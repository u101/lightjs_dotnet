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
        FuncCall = 1 << 5
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
        FuncCall // stop at comma or parentheses close
    }

    private ILjsAstNode Convert(ParsingMode parsingMode)
    {
        // TODO ternary opertaor .. ? .. : ..
        // TODO dot prop access

        var parenthesesCount = 0;
        
        var sourceCodeString = _sourceCodeString;
        var expressionStackPosition = new ExpressionStackPosition(_operatorsStack.Count, _postfixExpression.Count);


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
                    break;
                }
                if (parenthesesCount == 0 && token.TokenType == LjsTokenType.OpParenthesesClose)
                {
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
            
            else if (token.TokenType == LjsTokenType.OpParenthesesOpen)
            {
                if (IsFunctionCall(prevToken.TokenType))
                {
                    if (nextToken.TokenType == LjsTokenType.OpParenthesesClose)
                    {
                        // func call without arguments
                        _operatorsStack.Push(new Op(
                            token, OpType.FuncCall | OpType.UnaryPostfix, 
                            LjsAstBuilderUtils.FuncCallOperatorPriority));
                        
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
                        
                        _operatorsStack.Push(new Op(
                            token, OpType.FuncCall | OpType.UnaryPostfix, 
                            LjsAstBuilderUtils.FuncCallOperatorPriority, argumentsCount));
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
                while (_operatorsStack.Count > 0 && !IsParenthesesOpenOperator(_operatorsStack.Peek()))
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
                
                while (_operatorsStack.Count > 0 && (_operatorsStack.Peek().Priority >= opPriority))
                    _postfixExpression.Add(_operatorsStack.Pop());
                
                _operatorsStack.Push(new Op(token, opType, opPriority));
            }
        }

        // check unclosed groups
        
        if (parenthesesCount > 0)
        {
            while (_operatorsStack.Count > expressionStackPosition.OperatorsStackStartingLn)
            {
                var op = _operatorsStack.Pop();
                if (op.TokenType == LjsTokenType.OpParenthesesOpen)
                {
                    throw new LjsSyntaxError("unclosed parentheses", op.Token.Position);
                }
            }

            throw new LjsSyntaxError("unclosed parentheses");
        }
        
        while (_operatorsStack.Count > expressionStackPosition.OperatorsStackStartingLn)
        {
            _postfixExpression.Add(_operatorsStack.Pop());
        }
        
        // --------- CREATE NODES
        
        _locals.Clear();

        var postfixExprStart = expressionStackPosition.PostfixExpressionStartingLn;
        
        for (var i = postfixExprStart; i < _postfixExpression.Count; i++)
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
                // unary operaton
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
                        switch (left)
                        {
                            case LjsAstGetVar getVar:
                                _locals.Push(new LjsAstSetVar(getVar.VarName, right, LjsAstBuilderUtils.GetAssignMode(op.TokenType)));
                                break;
                            case LjsAstGetNamedProperty getProp:
                                _locals.Push(new LjsAstSetNamedProperty(
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
            postfixExprStart, _postfixExpression.Count - postfixExprStart);
        
        return _locals.Pop();
    }

    private static bool IsParenthesesOpenOperator(Op op) => (op.OpType & OpType.Parentheses) != 0 &&
                                                             op.TokenType == LjsTokenType.OpParenthesesOpen;
    
    private static bool IsFunctionCall(LjsTokenType prevTokenType) => prevTokenType is
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