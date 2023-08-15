using LightJS.Tokenizer;

namespace LightJS.Ast;

public class LjsAstBuilder2
{
    private readonly string _sourceCodeString;
    private readonly TokensReader _tokensReader;
    
    private readonly List<ILjsAstNode> _postfixExpression = new();
    private readonly Stack<Op> _operatorsStack = new();
    private readonly Stack<ILjsAstNode> _locals = new();

    /// <summary>
    /// save nodes positions in source code (line number, col number)
    /// </summary>
    private readonly Dictionary<ILjsAstNode, LjsTokenPosition> _tokenPositionsMap = new();

    public LjsAstBuilder2(string sourceCodeString)
    {
        if (string.IsNullOrEmpty(sourceCodeString))
        {
            throw new ArgumentException("input string is null or empty");
        }
        
        var ljsTokenizer = new LjsTokenizer(sourceCodeString);
        var tokens = ljsTokenizer.ReadTokens();
        
        _sourceCodeString = sourceCodeString;
        _tokensReader = new TokensReader(tokens);
    }
    
    public LjsAstBuilder2(string sourceCodeString, List<LjsToken> tokens)
    {
        if (string.IsNullOrEmpty(sourceCodeString))
        {
            throw new ArgumentException("input string is null or empty");
        }
        
        if (tokens == null)
            throw new ArgumentNullException(nameof(tokens));

        if (tokens.Count == 0)
            throw new ArgumentException("empty tokens list");
        
        _sourceCodeString = sourceCodeString;
        _tokensReader = new TokensReader(tokens);
    }

    public LjsAstModel Build()
    {
        if (!_tokensReader.HasNextToken)
        {
            throw new Exception("no tokens");
        }

        const ExpressionTerminationType expressionTerminationType = 
            ExpressionTerminationType.Eof | ExpressionTerminationType.Semicolon;

        var firstExpression = Process(expressionTerminationType);
        
        if (!_tokensReader.HasNextToken)
            return new LjsAstModel(firstExpression, _tokenPositionsMap);
        
        var sq = new LjsAstSequence();
        sq.AddNode(firstExpression);

        while (_tokensReader.HasNextToken)
        {
            sq.AddNode(Process(expressionTerminationType));
        }

        return new LjsAstModel(sq, _tokenPositionsMap);

    }
    
    [Flags]
    private enum OpType
    {
        None = 0,
        Binary = 1 << 0,
        UnaryPrefix = 1 << 1,
        UnaryPostfix = 1 << 2,
        Assign = 1 << 3,
        Parentheses = 1 << 4,
        FuncCall = 1 << 5,
        PropAccess = 1 << 6,
        TernaryIf = 1 << 7
    }

    private class Op : ILjsAstNode
    {
        public LjsTokenType TokenType => Token.TokenType;
        public LjsToken Token { get; }
        public OpType OpType { get; }
        public bool IsUnary => 
            ((this.OpType & OpType.UnaryPrefix) | (this.OpType & OpType.UnaryPostfix)) != 0;
        
        public bool IsUnaryPrefix => (this.OpType & OpType.UnaryPrefix) != 0;
        public bool IsUnaryPostfix => (this.OpType & OpType.UnaryPostfix) != 0;
        public bool IsBinary => (this.OpType & OpType.Binary) != 0;
        public int Priority { get; }
        public int InnerMembersCount { get; }

        public Op(LjsToken token, OpType opType, int priority, int innerMembersCount = 0)
        {
            Token = token;
            OpType = opType;
            Priority = priority;
            InnerMembersCount = innerMembersCount;
        }
    }
    
    [Flags]
    private enum ExpressionTerminationType
    {
        None = 0,
        Eof = 1 << 0,
        Semicolon = 1 << 1,
        ParenthesesClose = 1 << 2,
        Comma = 1 << 3,
        Colon = 1 << 4,
        SquareBracketsClose = 1 << 5,
        
        FuncCall = ParenthesesClose | Comma,
    }

    private static bool ShouldStopExpressionParsing(
        LjsToken currentToken,
        LjsToken prevToken,
        ExpressionTerminationType terminationType)
    {
        return
            ((terminationType & ExpressionTerminationType.ParenthesesClose) != 0 &&
             currentToken.TokenType == LjsTokenType.OpParenthesesClose) ||

            ((terminationType & ExpressionTerminationType.Semicolon) != 0 &&
             currentToken.TokenType == LjsTokenType.OpSemicolon) ||

            ((terminationType & ExpressionTerminationType.Comma) != 0 &&
             currentToken.TokenType == LjsTokenType.OpComma) ||
            
            ((terminationType & ExpressionTerminationType.SquareBracketsClose) != 0 &&
             currentToken.TokenType == LjsTokenType.OpSquareBracketsClose) ||

            ((terminationType & ExpressionTerminationType.Colon) != 0 &&
             currentToken.TokenType == LjsTokenType.OpColon)
            ;
    }

    private ILjsAstNode ProcessBlock(ExpressionTerminationType terminationType)
    {
        // three scenarios of termination : eof, semicolon, auto-semicolon
        
        var terminator = 
            terminationType | ExpressionTerminationType.Semicolon;
        
        var firstExpression = Process(terminator);

        if (!_tokensReader.HasNextToken)
        {
            if ((terminationType & ExpressionTerminationType.Eof) != 0)
                return firstExpression;

            throw new LjsSyntaxError("unexpected EOF", _tokensReader.CurrentToken.Position);
        }
        
        var sq = new LjsAstSequence();
        sq.AddNode(firstExpression);

        while (_tokensReader.HasNextToken)
        {
            sq.AddNode(Process(terminator));
        }

        return sq;
    }
    
    private ILjsAstNode Process(ExpressionTerminationType terminationType)
    {
        
        var sourceCodeString = _sourceCodeString;

        var operatorsStackStartingLn = _operatorsStack.Count;
        var postfixExpressionStartingLn = _postfixExpression.Count;

        var startingToken = _tokensReader.CurrentToken;
        var lastProcessedToken = startingToken;
        
        var parsingModeFinished = false;

        while (_tokensReader.HasNextToken)
        {
            _tokensReader.MoveForward();

            var token = _tokensReader.CurrentToken;
            var prevToken = _tokensReader.PrevToken;
            var nextToken = _tokensReader.NextToken;

            if (ShouldStopExpressionParsing(token, prevToken, terminationType))
            {
                parsingModeFinished = true;
                break;
            }

            lastProcessedToken = token;
            
            if (token.TokenType == LjsTokenType.Identifier)
            {
                _postfixExpression.Add(new LjsAstGetVar(
                    sourceCodeString.Substring(token.Position.CharIndex, token.StringLength)));
            }
            else if (LjsAstBuilderUtils.IsLiteral(token.TokenType))
            {
                _postfixExpression.Add( LjsAstBuilderUtils.CreateLiteralNode(token, sourceCodeString));
            }
            
            else if (token.TokenType == LjsTokenType.OpQuestionMark)
            {
                var trueExpressionNode = Process(ExpressionTerminationType.Colon);
                var falseExpressionNode = Process(terminationType);

                parsingModeFinished = true;

                _postfixExpression.Add(trueExpressionNode);
                _postfixExpression.Add(falseExpressionNode);
                
                PushOperatorToStack(
                    new Op(token, OpType.TernaryIf,
                        LjsAstBuilderUtils.GetOperatorPriority(token.TokenType, false)),
                    operatorsStackStartingLn);
                
                break;
            }
            
            else if (token.TokenType == LjsTokenType.OpSquareBracketsOpen)
            {
                if (IsPropertyAccess(prevToken.TokenType))
                {
                    var propAccessNode = Process(
                        ExpressionTerminationType.SquareBracketsClose);
                    
                    _postfixExpression.Add(propAccessNode);
                    
                    _operatorsStack.Push(new Op(
                        token, OpType.PropAccess | OpType.UnaryPostfix, 
                        LjsAstBuilderUtils.FuncCallOperatorPriority, 1));
                }
                else
                {
                    throw new NotImplementedException();
                    // todo check if array initialization
                }
            }
            
            else if (token.TokenType == LjsTokenType.OpParenthesesOpen)
            {
                if (IsFunctionCall(prevToken.TokenType))
                {
                    if (nextToken.TokenType == LjsTokenType.OpParenthesesClose)
                    {
                        // func call without arguments
                        PushOperatorToStack(
                            new Op(token, OpType.FuncCall | OpType.UnaryPostfix,
                                LjsAstBuilderUtils.FuncCallOperatorPriority), 
                            
                            operatorsStackStartingLn);
                        
                        _tokensReader.MoveForward(); // skip closing parentheses
                    }
                    else
                    {
                        var argumentsCount = 1;
                        
                        var funcArg = Process(ExpressionTerminationType.FuncCall);
                        
                        _postfixExpression.Add(funcArg);
                        
                        while (_tokensReader.CurrentToken.TokenType != LjsTokenType.OpParenthesesClose)
                        {
                            funcArg = Process(ExpressionTerminationType.FuncCall);
                            _postfixExpression.Add(funcArg);
                            ++argumentsCount;
                        }
                        
                        PushOperatorToStack(
                            new Op(token, OpType.FuncCall | OpType.UnaryPostfix, 
                                LjsAstBuilderUtils.FuncCallOperatorPriority, argumentsCount),
                            operatorsStackStartingLn);
                    }
                    
                }
                else
                {
                    var enclosedOperation = Process(ExpressionTerminationType.ParenthesesClose);
                    _postfixExpression.Add(enclosedOperation);
                }
            }
            else if (LjsAstBuilderUtils.IsAssignOperator(token.TokenType))
            {
                // we do recursive assignment operations processing for preserving right to left order of operations in assignment chain
                var assignExpression = Process(terminationType);
                
                while (_operatorsStack.Count > operatorsStackStartingLn)
                {
                    _postfixExpression.Add(_operatorsStack.Pop());
                }
                
                _postfixExpression.Add(assignExpression);
                _postfixExpression.Add(new Op(token, OpType.Assign, 0));
                

                parsingModeFinished = true;
                
                break;
                
            }
            
            else if (LjsAstBuilderUtils.IsOrdinaryOperator(token.TokenType))
            {
                var isUnaryPrefix = LjsAstBuilderUtils.CanBeUnaryPrefixOp(token.TokenType) &&
                                    (prevToken.TokenType == LjsTokenType.None ||
                                     LjsAstBuilderUtils.IsBinaryOp(prevToken.TokenType) ||
                                     prevToken.TokenType == LjsTokenType.OpParenthesesOpen);
                var isUnaryPostfix = !isUnaryPrefix &&
                                     LjsAstBuilderUtils.CanBeUnaryPostfixOp(token.TokenType) && 
                              (prevToken.TokenType == LjsTokenType.OpParenthesesClose ||
                               prevToken.TokenType == LjsTokenType.OpSquareBracketsClose ||
                               prevToken.TokenType == LjsTokenType.Identifier);

                var isUnary = isUnaryPrefix || isUnaryPostfix;

                var opType = OpType.None;
                if (isUnaryPrefix) opType |= OpType.UnaryPrefix;
                if (isUnaryPostfix) opType |= OpType.UnaryPostfix;
                if (!isUnary)
                {
                    opType |= OpType.Binary;
                }
                
                // todo check isUnary equals isDefinitelyUnary() method and throw error if needed

                var opPriority = LjsAstBuilderUtils.GetOperatorPriority(token.TokenType, isUnary);
                
                PushOperatorToStack(
                    new Op(token, opType, opPriority), 
                    operatorsStackStartingLn);
            }
            else
            {
                throw new LjsSyntaxError($"unexpected token {token.TokenType}", token.Position);
            }
        }
        
        // check correct exit in modes
        if (!parsingModeFinished)
        {
            var eofTermination = 
                (terminationType & ExpressionTerminationType.Eof) != 0 && !_tokensReader.HasNextToken;

            if (!eofTermination)
            {
                throw new LjsSyntaxError("unterminated expression", lastProcessedToken.Position);
            }
        }
        
        while (_operatorsStack.Count > operatorsStackStartingLn)
        {
            _postfixExpression.Add(_operatorsStack.Pop());
        }
        
        // --------- CREATE NODES
        
        _locals.Clear();
        
        for (var i = postfixExpressionStartingLn; i < _postfixExpression.Count; i++)
        {
            var node = _postfixExpression[i];
            
            if (node is Op op)
            {

                // function call operation
                if ((op.OpType & OpType.FuncCall) != 0)
                {
                    var argumentsCount = op.InnerMembersCount;

                    if (argumentsCount == 0)
                    {
                        var operand = _locals.Pop();
                    
                        _locals.Push(new LjsAstFunctionCall(operand));
                    }
                    else
                    {
                        var args = new ILjsAstNode[argumentsCount];
                        
                        for (var j = 0; j < argumentsCount; j++)
                        {
                            args[j] = _locals.Pop();
                        }
                        
                        var operand = _locals.Pop();
                        
                        _locals.Push(new LjsAstFunctionCall(operand, args));
                    }
                }
                // property access
                else if ((op.OpType & OpType.PropAccess) != 0)
                {
                    var propNameNode =  _locals.Pop();
                    var operand = _locals.Pop();
                    
                    _locals.Push(new LjsAstGetProperty(propNameNode, operand));
                }
                else if ((op.OpType & OpType.TernaryIf) != 0)
                {
                    var falseExpression = _locals.Pop();
                    var trueExpression = _locals.Pop();
                    var condition = _locals.Pop();
                    _locals.Push(new LjsAstTernaryIfOperation(condition, trueExpression, falseExpression));
                }
                
                // unary operation
                else if (op.IsUnary)
                {
                    var operand = _locals.Pop();
                    _locals.Push(new LjsAstUnaryOperation(
                        operand, LjsAstBuilderUtils.GetUnaryOperationType(op.TokenType, op.IsUnaryPrefix)));
                }
                else
                {
                    // binary operation
                    var right = _locals.Pop();
                    var left = _locals.Pop();

                    if (op.TokenType == LjsTokenType.OpDot)
                    {
                        // convert get var node into get named property
                        if (right is LjsAstGetVar getVar)
                        {
                            _locals.Push(new LjsAstGetNamedProperty(getVar.VarName, left));
                        }
                        else
                        {
                            throw new LjsSyntaxError("invalid property access", op.Token.Position);
                        }
                    }

                    else if ((op.OpType & OpType.Assign) != 0)
                    {
                        // replace getter nodes by setter nodes
                        switch (left)
                        {
                            case LjsAstGetVar getVar:
                                _locals.Push(new LjsAstSetVar(getVar.VarName, right, LjsAstBuilderUtils.GetAssignMode(op.TokenType)));
                                break;
                            case LjsAstGetNamedProperty getNamedProp:
                                _locals.Push(new LjsAstSetNamedProperty(
                                    getNamedProp.PropertyName, getNamedProp.PropertySource, 
                                    right, LjsAstBuilderUtils.GetAssignMode(op.TokenType)));
                                break;
                            case LjsAstGetProperty getProp:
                                _locals.Push(new LjsAstSetProperty(
                                    getProp.PropertyName, getProp.PropertySource, 
                                    right, LjsAstBuilderUtils.GetAssignMode(op.TokenType)));
                                break;
                            default:
                                throw new LjsSyntaxError("invalid assign", op.Token.Position);
                        }
                    }
                    else
                    {
                        _locals.Push(new LjsAstBinaryOperation(
                            left, right, LjsAstBuilderUtils.GetBinaryOperationType(op.TokenType)));
                    }
                    
                    
                }
                
            }
            else
            {
                _locals.Push(node);
            }
        }
        
        _postfixExpression.RemoveRange(
            postfixExpressionStartingLn, _postfixExpression.Count - postfixExpressionStartingLn);
        
        return _locals.Pop();
    }

    private void PushOperatorToStack(Op op, int operatorsStackStartingLn)
    {
        while (_operatorsStack.Count > operatorsStackStartingLn && 
               (_operatorsStack.Peek().Priority >= op.Priority))
            _postfixExpression.Add(_operatorsStack.Pop());
                
        _operatorsStack.Push(op);
    }
    
    private static bool IsFunctionCall(LjsTokenType prevTokenType) => prevTokenType is
        LjsTokenType.Identifier or
        LjsTokenType.OpParenthesesClose or
        LjsTokenType.OpSquareBracketsClose;

    private static bool IsPropertyAccess(LjsTokenType prevTokenType) => prevTokenType is
        LjsTokenType.Identifier or
        LjsTokenType.OpParenthesesClose or
        LjsTokenType.OpSquareBracketsClose;

    private class TokensReader
    {
        private readonly List<LjsToken> _tokens;

        private int _currentIndex = -1;
    
        public TokensReader(List<LjsToken> tokens)
        {
            _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        }

        public LjsToken CurrentToken => 
            _currentIndex >= 0 && _currentIndex < _tokens.Count ? 
                _tokens[_currentIndex] : default;

        public LjsToken NextToken => 
            _currentIndex + 1 < _tokens.Count ? 
                _tokens[_currentIndex + 1] : default;

        public LjsToken PrevToken => 
            _currentIndex > 0 ?
                _tokens[_currentIndex - 1] : default;

        public bool HasNextToken => _currentIndex + 1 < _tokens.Count;

        public void MoveForward()
        {
            if (!HasNextToken)
            {
                throw new IndexOutOfRangeException();
            }
        
            ++_currentIndex;
        }
    }
    
}