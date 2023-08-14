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

        var node = Convert(ParsingMode.None);

        return new LjsAstModel(node, _tokenPositionsMap);

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
    
    private readonly struct ExpressionStackPosition
    {
        public int OperatorsStackStartingLn { get; }
        public int PostfixExpressionStartingLn { get; }

        public ExpressionStackPosition(int operatorsStackStartingLn, int postfixExpressionStartingLn)
        {
            OperatorsStackStartingLn = operatorsStackStartingLn;
            PostfixExpressionStartingLn = postfixExpressionStartingLn;
        }
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
        public bool IsAssign => (this.OpType & OpType.Assign) != 0;
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
    
    private enum ParsingMode
    {
        None,
        FuncCall, // stop at comma or parentheses close
        PropAccess, // property access with square brackets
        UpToColon
    }

    private ILjsAstNode Convert(ParsingMode parsingMode)
    {
        // TODO ternary opertaor .. ? .. : ..

        var parenthesesCount = 0;
        
        var sourceCodeString = _sourceCodeString;
        var expressionStackPosition = new ExpressionStackPosition(
            _operatorsStack.Count, _postfixExpression.Count);

        var operatorsStackStartingLn = _operatorsStack.Count;
        var postfixExpressionStartingLn = _postfixExpression.Count;

        var startingToken = _tokensReader.CurrentToken;
        var parsingModeFinished = false;

        while (_tokensReader.HasNextToken)
        {
            _tokensReader.MoveForward();

            var token = _tokensReader.CurrentToken;
            var prevToken = _tokensReader.PrevToken;
            var nextToken = _tokensReader.NextToken;

            if (parsingMode == ParsingMode.FuncCall)
            {
                if (token.TokenType == LjsTokenType.OpComma)
                {
                    parsingModeFinished = true;
                    break;
                }
                if (parenthesesCount == 0 && token.TokenType == LjsTokenType.OpParenthesesClose)
                {
                    parsingModeFinished = true;
                    break;
                }
            }
            else if (parsingMode == ParsingMode.PropAccess)
            {
                if (token.TokenType == LjsTokenType.OpSquareBracketsClose)
                {
                    parsingModeFinished = true;
                    break;
                }
            }
            else if (parsingMode == ParsingMode.UpToColon)
            {
                if (token.TokenType == LjsTokenType.OpColon)
                {
                    parsingModeFinished = true;
                    break;
                }
            }
            
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
                var trueExpressionNode = Convert(ParsingMode.UpToColon);
                var falseExpressionNode = Convert(ParsingMode.None);

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
                    var propAccessNode = Convert(ParsingMode.PropAccess);
                    
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
                        var funcArg = Convert(ParsingMode.FuncCall);
                        
                        _postfixExpression.Add(funcArg);
                        
                        while (_tokensReader.CurrentToken.TokenType != LjsTokenType.OpParenthesesClose)
                        {
                            funcArg = Convert(ParsingMode.FuncCall);
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
                    _operatorsStack.Push(new Op(token, OpType.Parentheses, 0));
                    ++parenthesesCount;
                }
            }
            
            else if (token.TokenType == LjsTokenType.OpParenthesesClose)
            {
                while (_operatorsStack.Count > operatorsStackStartingLn && 
                       !IsParenthesesOpenOperator(_operatorsStack.Peek()))
                {
                    _postfixExpression.Add(_operatorsStack.Pop()); 
                }
                
                _operatorsStack.Pop(); // remove opening parentheses from stack
                --parenthesesCount;
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
                    if (LjsAstBuilderUtils.IsAssignOperator(token.TokenType))
                    {
                        opType |= OpType.Assign;
                    }
                }
                
                // todo check isUnary equals isDefinitelyUnary() method and throw error if needed

                var opPriority = LjsAstBuilderUtils.GetOperatorPriority(token.TokenType, isUnary);
                
                PushOperatorToStack(
                    new Op(token, opType, opPriority), 
                    operatorsStackStartingLn);
            }
            else
            {
                throw new LjsSyntaxError("unexpected token", token.Position);
            }
        }
        
        // check correct exit in modes
        if (!parsingModeFinished)
        {
            switch (parsingMode)
            {
                case ParsingMode.FuncCall:
                    throw new LjsSyntaxError("invalid function call", startingToken.Position);
                case ParsingMode.PropAccess:
                    throw new LjsSyntaxError("invalid [] prop access", startingToken.Position);
            }
        }

        // check unclosed groups
        
        if (parenthesesCount > 0)
        {
            while (_operatorsStack.Count > operatorsStackStartingLn)
            {
                var op = _operatorsStack.Pop();
                if (op.TokenType == LjsTokenType.OpParenthesesOpen)
                {
                    throw new LjsSyntaxError("unclosed parentheses", op.Token.Position);
                }
            }

            throw new LjsSyntaxError("unclosed parentheses");
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

                    else if (op.IsAssign)
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

    private static bool IsParenthesesOpenOperator(Op op) => (op.OpType & OpType.Parentheses) != 0 &&
                                                             op.TokenType == LjsTokenType.OpParenthesesOpen;
    
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

        public int CurrentIndex => _currentIndex;

        public LjsToken this[int index] => _tokens[index];

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

        public bool HasCurrentToken => _currentIndex >= 0 && _currentIndex < _tokens.Count;

        public bool HasPrevToken => _currentIndex > 0;

        public void MoveForward()
        {
            if (!HasNextToken)
            {
                throw new IndexOutOfRangeException();
            }
        
            ++_currentIndex;
        }

        public void MoveTo(int index)
        {
            if (index < 0 || index >= _tokens.Count)
                throw new ArgumentException($"token index {index} out of range [0 .. {_tokens.Count}]");
            _currentIndex = index;
        }
    }
    
}