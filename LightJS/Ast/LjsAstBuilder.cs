using LightJS.Tokenizer;

namespace LightJS.Ast;

/// <summary>
/// Abstract syntax tree builder
/// </summary>
public class LjsAstBuilder
{
    private readonly string _sourceCodeString;
    private readonly TokensReader _tokensReader;

    /// <summary>
    /// save nodes positions in source code (line number, col number)
    /// </summary>
    private readonly Dictionary<ILjsAstNode, LjsTokenPosition> _tokenPositionsMap = new();

    public LjsAstBuilder(string sourceCodeString)
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
    
    public LjsAstBuilder(string sourceCodeString, List<LjsToken> tokens)
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
        
        var firstExpression = ParseExpression(StopTokenType.NextExpression);

        if (!_tokensReader.HasNextToken) 
            return new LjsAstModel(firstExpression, _tokenPositionsMap);

        var sq = new LjsAstSequence();
        sq.AddNode(firstExpression);

        while (_tokensReader.HasNextToken)
        {
            sq.AddNode(ParseExpression(StopTokenType.NextExpression));
        }

        return new LjsAstModel(sq, _tokenPositionsMap);
    }

    
    private readonly List<ILjsAstNode> _postfixExpression = new();
    private readonly Stack<OperatorNode> _operatorsStack = new();
    private readonly Stack<ILjsAstNode> _locals = new();

    private readonly List<OperatorNode> _opNodesPool = new();

    private static readonly Dictionary<LjsTokenType, int> OperatorsPriorityMap = new()
    {
        { LjsTokenType.OpParenthesesOpen, 0},
        { LjsTokenType.OpParenthesesClose, 0},
        
        { LjsTokenType.OpEquals, 50},
        { LjsTokenType.OpNotEqual, 50},
        { LjsTokenType.OpEqualsStrict, 50},
        { LjsTokenType.OpGreater, 50},
        { LjsTokenType.OpGreaterOrEqual, 50},
        { LjsTokenType.OpLess, 50},
        { LjsTokenType.OpLessOrEqual, 50},
        
        { LjsTokenType.OpLogicalAnd, 80},
        { LjsTokenType.OpLogicalOr, 80},
        
        { LjsTokenType.OpPlus, 100},
        { LjsTokenType.OpMinus, 100},
        
        { LjsTokenType.OpMultiply, 200},
        { LjsTokenType.OpDiv, 200},
        { LjsTokenType.OpModulo, 200},
        
        { LjsTokenType.OpLogicalNot, 1000},
        { LjsTokenType.OpIncrement, 1000},
        { LjsTokenType.OpDecrement, 1000},
    };

    private OperatorNode GetOrCreateBinaryOperatorNode(LjsTokenType tokenType, LjsTokenPosition tokenPosition)
    {
        if (_opNodesPool.Count > 0)
        {
            var operatorNode = _opNodesPool[^1];
            _opNodesPool.RemoveAt(_opNodesPool.Count - 1);
            operatorNode.OperatorType = tokenType;
            operatorNode.TokenPosition = tokenPosition;
            return operatorNode;
        }

        return new OperatorNode()
        {
            OperatorType = tokenType,
            TokenPosition = tokenPosition
        };
    }

    private void ReleaseOperatorNode(OperatorNode operatorNode)
    {
        _opNodesPool.Add(operatorNode);
    }

    private class OperatorNode : ILjsAstNode
    {
        public LjsTokenType OperatorType { get; set; } = LjsTokenType.None;

        public LjsTokenPosition TokenPosition { get; set; } = default;
        
        public IEnumerable<ILjsAstNode> ChildNodes => Array.Empty<ILjsAstNode>();
        public bool HasChildNodes => false;
    }
    
    [Flags]
    private enum StopTokenType
    {
        None = 0,
        // semi colon or new expression on a new line
        NextExpression = 1 << 0,
        ParenthesesClose = 1 << 1,
        SquareBracketsClose = 1 << 2,
        Colon = 1 << 3,
        Comma = 1 << 4
    }
    
    private enum ExpressionMemberType
    {
        None,
        Operand,
        Operator
    }
    
    private enum OperatorType
    {
        None,
        Binary,
        Unary,
        Polymorphic // unary or binary
    }

    private static OperatorType GetOperatorType(LjsTokenType tokenType) => tokenType switch
        {
            LjsTokenType.OpPlus => OperatorType.Polymorphic,
            LjsTokenType.OpMinus => OperatorType.Polymorphic,
            
            LjsTokenType.OpLogicalNot => OperatorType.Unary,
            LjsTokenType.OpIncrement => OperatorType.Unary,
            LjsTokenType.OpDecrement => OperatorType.Unary,
            
            LjsTokenType.OpGreater => OperatorType.Binary,
            LjsTokenType.OpLess => OperatorType.Binary,
            LjsTokenType.OpMultiply => OperatorType.Binary,
            LjsTokenType.OpDiv => OperatorType.Binary,
            LjsTokenType.OpModulo => OperatorType.Binary,
            LjsTokenType.OpBitAnd => OperatorType.Binary,
            LjsTokenType.OpBitOr => OperatorType.Binary,
            
            LjsTokenType.OpEquals => OperatorType.Binary,
            LjsTokenType.OpEqualsStrict => OperatorType.Binary,
            LjsTokenType.OpGreaterOrEqual => OperatorType.Binary,
            LjsTokenType.OpLessOrEqual => OperatorType.Binary,
            LjsTokenType.OpNotEqual => OperatorType.Binary,
            LjsTokenType.OpNotEqualStrict => OperatorType.Binary,
            LjsTokenType.OpLogicalAnd => OperatorType.Binary,
            LjsTokenType.OpLogicalOr => OperatorType.Binary,
            
            _ => OperatorType.None
        };

    private ILjsAstNode ParseExpression(StopTokenType stopTokenType = StopTokenType.None)
    {

        var operatorsStackStartingLn = _operatorsStack.Count;
        var postfixExpressionStartingLn = _postfixExpression.Count;

        var prevOperatorType = OperatorType.None;
        var prevMemberType = ExpressionMemberType.None;
        var prefixUnaryOperatorToken = default(LjsToken);

        while (_tokensReader.HasNextToken)
        {
            
            if ((stopTokenType & StopTokenType.NextExpression) != 0 && 
                prevMemberType != ExpressionMemberType.None)
            {
                var nextToken = _tokensReader.NextToken;
                var nextTokenType = nextToken.TokenType;
                
                if (nextTokenType == LjsTokenType.OpSemicolon)
                {
                    _tokensReader.MoveForward();
                    break;
                }

                var nextOpType = GetOperatorType(nextTokenType);

                if (_tokensReader.CurrentToken.Position.Line < nextToken.Position.Line &&
                    prevMemberType != ExpressionMemberType.Operator && 
                     nextOpType != OperatorType.Binary && nextOpType != OperatorType.Polymorphic)
                {
                    break;
                }
            }
            
            _tokensReader.MoveForward();

            var token = _tokensReader.CurrentToken;
            var tokenType = token.TokenType;
            var tokenPosition = token.Position;

            if (LjsAstBuilderUtils.IsLiteral(tokenType))
            {
                if (prevMemberType == ExpressionMemberType.Operand)
                {
                    throw new LjsSyntaxError("unexpected token", tokenPosition);
                }

                var literalNode = LjsAstBuilderUtils.CreateLiteralNode(token, _sourceCodeString);
                
                _tokenPositionsMap[literalNode] = tokenPosition;

                if (prevMemberType == ExpressionMemberType.Operator &&
                    prevOperatorType == OperatorType.Unary)
                {
                    literalNode = new LjsAstUnaryOperation(literalNode,
                        LjsAstBuilderUtils.GetUnaryPrefixOperationType(prefixUnaryOperatorToken.TokenType));
                    
                    _tokenPositionsMap[literalNode] = tokenPosition;
                }
                
                _postfixExpression.Add(literalNode);

                prevOperatorType = OperatorType.None;
                prevMemberType = ExpressionMemberType.Operand;
            }

            else if (tokenType == LjsTokenType.Identifier)
            {
                if (prevMemberType == ExpressionMemberType.Operand)
                {
                    throw new LjsSyntaxError($"unexpected token {tokenType}", tokenPosition);
                }
                
                // 1) simple var here : x
                // 2) func call : x(..)
                // 3) prop access : x.foo
                // 4) square brackets prop access : x[..]
                // 5) var assign : x = 0

                var node = ParseIdentifier(
                    stopTokenType, prevMemberType == ExpressionMemberType.None);
                
                if (prevMemberType == ExpressionMemberType.Operator &&
                    prevOperatorType == OperatorType.Unary)
                {
                    node = new LjsAstUnaryOperation(node,
                        LjsAstBuilderUtils.GetUnaryPrefixOperationType(prefixUnaryOperatorToken.TokenType));
                    
                    _tokenPositionsMap[node] = tokenPosition;
                }
                
                _postfixExpression.Add(node);

                prevOperatorType = OperatorType.None;
                prevMemberType = ExpressionMemberType.Operand;
                
                if (node is ILjsAstSetterNode)
                {
                    break;
                }
            }

            else if (tokenType == LjsTokenType.OpQuestionMark)
            {
                if (_postfixExpression.Count - postfixExpressionStartingLn == 0 ||
                    prevMemberType == ExpressionMemberType.Operator)
                {
                    throw new LjsSyntaxError("unexpected token", tokenPosition);
                }

                var condition =
                    BuildExpression(operatorsStackStartingLn, postfixExpressionStartingLn);

                var trueExpression = ParseExpression(StopTokenType.Colon);
                var falseExpression = ParseExpression(StopTokenType.Colon);

                return new LjsAstTernaryIfOperation(condition, trueExpression, falseExpression);
            }

            else if (tokenType == LjsTokenType.OpColon)
            {
                if (prevMemberType == ExpressionMemberType.Operator)
                {
                    throw new LjsSyntaxError("unexpected token", tokenPosition);
                }
                
                if ((stopTokenType & StopTokenType.Colon) != 0)
                {
                    break;
                }

                throw new LjsSyntaxError("unexpected ':'", tokenPosition);
            }
            
            else if (tokenType == LjsTokenType.OpComma)
            {
                if (prevMemberType == ExpressionMemberType.Operator)
                {
                    throw new LjsSyntaxError("unexpected token", tokenPosition);
                }
                
                if ((stopTokenType & StopTokenType.Comma) != 0)
                {
                    break;
                }

                throw new LjsSyntaxError("unexpected ','", tokenPosition);
            }

            else if (tokenType == LjsTokenType.OpParenthesesOpen)
            {
                if (prevMemberType == ExpressionMemberType.Operand)
                    throw new LjsSyntaxError("unexpected token", tokenPosition);

                var exp = ParseExpression(StopTokenType.ParenthesesClose);

                if (_tokensReader.CurrentToken.TokenType != LjsTokenType.OpParenthesesClose)
                {
                    throw new LjsSyntaxError("unclosed parentheses", tokenPosition);
                }
                
                //_tokenPositionsMap[exp] = tokenPosition; skip, already saved
                
                if (prevMemberType == ExpressionMemberType.Operator &&
                    prevOperatorType == OperatorType.Unary)
                {
                    exp = new LjsAstUnaryOperation(exp,
                        LjsAstBuilderUtils.GetUnaryPrefixOperationType(prefixUnaryOperatorToken.TokenType));
                    _tokenPositionsMap[exp] = tokenPosition;
                }

                _postfixExpression.Add(exp);

                prevOperatorType = OperatorType.None;
                prevMemberType = ExpressionMemberType.Operand;
            }

            else if (tokenType == LjsTokenType.OpParenthesesClose)
            {
                if (prevMemberType == ExpressionMemberType.Operator)
                {
                    throw new LjsSyntaxError("unexpected token", tokenPosition);
                }
                
                if ((stopTokenType & StopTokenType.ParenthesesClose) != 0)
                {
                    break;
                }

                throw new LjsSyntaxError("unexpected parentheses", tokenPosition);
            }
            
            else if (tokenType == LjsTokenType.OpSquareBracketsClose)
            {
                if (prevMemberType == ExpressionMemberType.Operator)
                {
                    throw new LjsSyntaxError("unexpected token", tokenPosition);
                }
                
                if ((stopTokenType & StopTokenType.SquareBracketsClose) != 0)
                {
                    break;
                }

                throw new LjsSyntaxError("unexpected square bracket", tokenPosition);
            }

            else if (OperatorsPriorityMap.ContainsKey(tokenType))
            {
                var operatorType = GetOperatorType(tokenType);

                if (prevMemberType == ExpressionMemberType.Operator &&
                    operatorType == OperatorType.Binary &&
                    prevOperatorType == OperatorType.Binary)
                {
                    // two binary operators in the line
                    throw new LjsSyntaxError("unexpected token", tokenPosition);
                }

                if (operatorType == OperatorType.Polymorphic)
                {
                    operatorType = prevMemberType == ExpressionMemberType.Operand ? 
                        OperatorType.Binary : OperatorType.Unary;
                }

                if (operatorType == OperatorType.Unary)
                {
                    if (prevMemberType == ExpressionMemberType.Operator &&
                        prevOperatorType == OperatorType.Unary)
                    {
                        throw new LjsSyntaxError("unary operators sequence not supported", tokenPosition);
                    }
                    
                    // process postfix increment / decrement
                    if ((tokenType == LjsTokenType.OpIncrement || tokenType == LjsTokenType.OpDecrement) &&
                        prevMemberType == ExpressionMemberType.Operand)
                    {
                        var lastOperandIndex = GetLastOperandIndex();

                        if (lastOperandIndex == -1)
                            throw new Exception("last operand not found");

                        var operand = _postfixExpression[lastOperandIndex];

                        var unaryOpType = tokenType == LjsTokenType.OpIncrement
                            ? LjsAstUnaryOperationType.PostfixIncrement
                            : LjsAstUnaryOperationType.PostfixDecrement;

                        var unaryOperation = new LjsAstUnaryOperation(operand, unaryOpType);

                        _tokenPositionsMap[unaryOperation] = tokenPosition;
                        
                        _postfixExpression[lastOperandIndex] = unaryOperation;
                        
                        prevMemberType = ExpressionMemberType.Operand;
                        prevOperatorType = OperatorType.None;
                    }
                    else
                    {
                        prefixUnaryOperatorToken = token;
                        prevMemberType = ExpressionMemberType.Operator;
                        prevOperatorType = OperatorType.Unary;
                    }
                }
                else
                {
                    
                    while (_operatorsStack.Count > operatorsStackStartingLn &&
                           (OperatorsPriorityMap[_operatorsStack.Peek().OperatorType] >=
                            OperatorsPriorityMap[tokenType]))
                        _postfixExpression.Add(_operatorsStack.Pop());


                    var operatorNode = GetOrCreateBinaryOperatorNode(tokenType, token.Position);
                    _operatorsStack.Push(operatorNode);
                    
                    prevMemberType = ExpressionMemberType.Operator;
                    prevOperatorType = operatorType;
                }
                
            }
            else
            {
                throw new LjsSyntaxError($"unexpected token {tokenType}", tokenPosition);
            }
        }

        return BuildExpression(operatorsStackStartingLn, postfixExpressionStartingLn);
    }

    private ILjsAstNode ParseDotPropertyOperation(
        ILjsAstNode propertySource, 
        StopTokenType stopTokenType, 
        bool assignmentOperationAllowed)
    {
        if (!_tokensReader.HasNextToken || _tokensReader.NextToken.TokenType != LjsTokenType.Identifier)
        {
            throw new LjsSyntaxError("expected identifier", _tokensReader.CurrentToken.Position);
        }
        
        _tokensReader.MoveForward();
        
        var token = _tokensReader.CurrentToken;
        var tokenPosition = token.Position;

        var id = _sourceCodeString.Substring(tokenPosition.CharIndex, token.StringLength);
        
        if (_tokensReader.HasNextToken)
        {
            var nextToken = _tokensReader.NextToken;

            if (LjsAstBuilderUtils.IsAssignOperator(nextToken.TokenType))
            {
                if (!assignmentOperationAllowed)
                    throw new LjsSyntaxError($"invalid assignment {nextToken.TokenType}", nextToken.Position);

                _tokensReader.MoveForward();
                    
                var exp = ParseExpression(stopTokenType);

                var setVar = new LjsAstSetNamedProperty(
                    id, propertySource, exp, 
                    LjsAstBuilderUtils.GetAssignMode(nextToken.TokenType));
                        
                _tokenPositionsMap[setVar] = nextToken.Position;

                return setVar;
            }

            switch (nextToken.TokenType)
            {
                case LjsTokenType.OpDot:
                
                    _tokensReader.MoveForward();
                
                    var varDot = new LjsAstGetNamedProperty(id, propertySource);

                    _tokenPositionsMap[varDot] = tokenPosition;

                    return ParseDotPropertyOperation(varDot, stopTokenType, assignmentOperationAllowed);
            
                case LjsTokenType.OpParenthesesOpen:
                
                    _tokensReader.MoveForward();
                
                    var getFuncNode = new LjsAstGetNamedProperty(id, propertySource);

                    _tokenPositionsMap[getFuncNode] = tokenPosition;

                    return ParseFunctionCall(
                        getFuncNode, tokenPosition, 
                        stopTokenType, assignmentOperationAllowed);
            
                case LjsTokenType.OpSquareBracketsOpen:
                
                    _tokensReader.MoveForward();

                    var n = new LjsAstGetNamedProperty(id, propertySource);
                
                    _tokenPositionsMap[n] = tokenPosition;

                    return ParseSquareBracketsPropertyOperation(n, stopTokenType, assignmentOperationAllowed);
            }
        }
        
        var getProp = new LjsAstGetNamedProperty(id, propertySource);
                
        _tokenPositionsMap[getProp] = tokenPosition;

        return getProp;
    }

    private ILjsAstNode ParseSquareBracketsPropertyOperation(
        ILjsAstNode propertySource, 
        StopTokenType stopTokenType, 
        bool assignmentOperationAllowed)
    {
        if (!_tokensReader.HasNextToken)
        {
            throw new LjsSyntaxError("expected square brackets close", _tokensReader.CurrentToken.Position);
        }

        var tokenPosition = _tokensReader.CurrentToken.Position;

        var getPropNameExpression = ParseExpression(StopTokenType.SquareBracketsClose);
        
        _tokenPositionsMap[getPropNameExpression] = tokenPosition;

        if (_tokensReader.CurrentToken.TokenType != LjsTokenType.OpSquareBracketsClose)
        {
            throw new LjsSyntaxError("expected square brackets close", _tokensReader.CurrentToken.Position);
        }

        if (_tokensReader.HasNextToken)
        {
            var nextToken = _tokensReader.NextToken;

            if (LjsAstBuilderUtils.IsAssignOperator(nextToken.TokenType))
            {
                if (!assignmentOperationAllowed)
                    throw new LjsSyntaxError($"invalid assignment {nextToken.TokenType}", nextToken.Position);

                _tokensReader.MoveForward();
                    
                var exp = ParseExpression(stopTokenType);

                var setVar = new LjsAstSetProperty(
                    getPropNameExpression, 
                    propertySource, 
                    exp, 
                    LjsAstBuilderUtils.GetAssignMode(nextToken.TokenType));
                        
                _tokenPositionsMap[setVar] = nextToken.Position;

                return setVar;
            }

            switch (nextToken.TokenType)
            {
                case LjsTokenType.OpDot:
                
                    _tokensReader.MoveForward();
                
                    var varDot = new LjsAstGetProperty(getPropNameExpression, propertySource);

                    _tokenPositionsMap[varDot] = tokenPosition;

                    return ParseDotPropertyOperation(varDot, stopTokenType, assignmentOperationAllowed);
            
                case LjsTokenType.OpParenthesesOpen:
                
                    _tokensReader.MoveForward();
                
                    var getFuncNode = new LjsAstGetProperty(getPropNameExpression, propertySource);

                    _tokenPositionsMap[getFuncNode] = tokenPosition;

                    return ParseFunctionCall(
                        getFuncNode, _tokensReader.CurrentToken.Position, 
                        stopTokenType, assignmentOperationAllowed);
            
                case LjsTokenType.OpSquareBracketsOpen:
                
                    _tokensReader.MoveForward();

                    var n = new LjsAstGetProperty(getPropNameExpression, propertySource);
                
                    _tokenPositionsMap[n] = tokenPosition;

                    return ParseSquareBracketsPropertyOperation(n, stopTokenType, assignmentOperationAllowed);
            }
        }
        
        var getProp = new LjsAstGetProperty(getPropNameExpression, propertySource);
                
        _tokenPositionsMap[getProp] = tokenPosition;

        return getProp;
    }

    private ILjsAstNode ParseFunctionCall(
        ILjsAstNode functionGetter, LjsTokenPosition functionTokenPosition, 
        StopTokenType stopTokenType, bool assignmentOperationAllowed)
    {
        if (!_tokensReader.HasNextToken)
        {
            throw new LjsSyntaxError("expected parentheses close", _tokensReader.CurrentToken.Position);
        }

        var funcCall = new LjsAstFunctionCall(functionGetter);
        
        _tokenPositionsMap[funcCall] = functionTokenPosition;

        if (_tokensReader.NextToken.TokenType != LjsTokenType.OpParenthesesClose)
        {
            while (_tokensReader.CurrentToken.TokenType != LjsTokenType.OpParenthesesClose)
            {
                var argumentExpression = ParseExpression(
                    StopTokenType.Comma | StopTokenType.ParenthesesClose);
            
                funcCall.Arguments.Add(argumentExpression);
            }
        }
        else
        {
            _tokensReader.MoveForward();
        }
        
        if (_tokensReader.HasNextToken)
        {
            var nextToken = _tokensReader.NextToken;

            if (LjsAstBuilderUtils.IsAssignOperator(nextToken.TokenType))
            {
                throw new LjsSyntaxError($"invalid assignment {nextToken.TokenType}", nextToken.Position);
            }

            switch (nextToken.TokenType)
            {
                case LjsTokenType.OpDot:
                
                    _tokensReader.MoveForward();

                    return ParseDotPropertyOperation(funcCall, stopTokenType, assignmentOperationAllowed);
            
                case LjsTokenType.OpParenthesesOpen:
                
                    _tokensReader.MoveForward();

                    return ParseFunctionCall(
                        funcCall, _tokensReader.CurrentToken.Position, 
                        stopTokenType, assignmentOperationAllowed);
            
                case LjsTokenType.OpSquareBracketsOpen:
                
                    _tokensReader.MoveForward();

                    return ParseSquareBracketsPropertyOperation(
                        funcCall, stopTokenType, assignmentOperationAllowed);
            }
        }

        return funcCall;
    }

    private ILjsAstNode ParseIdentifier(StopTokenType stopTokenType, bool assignmentOperationAllowed)
    {
        var token = _tokensReader.CurrentToken;
        var tokenPosition = token.Position;

        var id = _sourceCodeString.Substring(tokenPosition.CharIndex, token.StringLength);

        // 1) simple var here : x
        // 2) func call : x(..)
        // 3) prop access : x.foo
        // 4) square brackets prop access : x[..]
        // 5) var assign : x = 0

        if (_tokensReader.HasNextToken)
        {
            var nextToken = _tokensReader.NextToken;

            if (LjsAstBuilderUtils.IsAssignOperator(nextToken.TokenType))
            {
                if (!assignmentOperationAllowed)
                    throw new LjsSyntaxError($"invalid assignment {nextToken.TokenType}", nextToken.Position);

                _tokensReader.MoveForward();
                    
                var exp = ParseExpression(stopTokenType);

                var setVar = new LjsAstSetVar(id, exp, LjsAstBuilderUtils.GetAssignMode(nextToken.TokenType));
                        
                _tokenPositionsMap[setVar] = nextToken.Position;

                return setVar;
            }

            switch (nextToken.TokenType)
            {
                case LjsTokenType.OpDot:
                
                    _tokensReader.MoveForward();
                
                    var varDor = new LjsAstGetVar(id);

                    _tokenPositionsMap[varDor] = tokenPosition;

                    return ParseDotPropertyOperation(varDor, stopTokenType, assignmentOperationAllowed);
            
                case LjsTokenType.OpParenthesesOpen:
                
                    _tokensReader.MoveForward();
                
                    var getFuncNode = new LjsAstGetVar(id);

                    _tokenPositionsMap[getFuncNode] = tokenPosition;

                    return ParseFunctionCall(
                        getFuncNode, tokenPosition, stopTokenType, assignmentOperationAllowed);
            
                case LjsTokenType.OpSquareBracketsOpen:
                
                    _tokensReader.MoveForward();

                    var n = new LjsAstGetVar(id);
                
                    _tokenPositionsMap[n] = tokenPosition;

                    return ParseSquareBracketsPropertyOperation(n, stopTokenType, assignmentOperationAllowed);
            }
        }
        
        var getVarNode = new LjsAstGetVar(id);

        _tokenPositionsMap[getVarNode] = tokenPosition;

        return getVarNode;
    }

    private int GetLastOperandIndex()
    {
        for (var i = _postfixExpression.Count - 1; i >= 0; --i)
        {
            var n = _postfixExpression[i];
            if (n is not OperatorNode) return i;
        }

        return -1;
    }

    private ILjsAstNode BuildExpression(int operatorsStackStartingLn, int postfixExpressionStartingLn)
    {
        while (_operatorsStack.Count > operatorsStackStartingLn)
        {
            var op = _operatorsStack.Pop();
            _postfixExpression.Add(op);
        }
        
        _locals.Clear();
        
        for (var i = postfixExpressionStartingLn; i < _postfixExpression.Count; i++)
        {
            var n = _postfixExpression[i];

            if (n is OperatorNode operatorNode)
            {
                //	Получаем значения из стека в обратном порядке
                var rightOperand = _locals.Pop();
                var leftOperand = _locals.Pop();

                var binaryOperation = new LjsAstBinaryOperation(
                    leftOperand, rightOperand, LjsAstBuilderUtils.GetBinaryOperationType(operatorNode.OperatorType));
                
                _tokenPositionsMap[binaryOperation] = operatorNode.TokenPosition;
                _locals.Push(binaryOperation);
                
                ReleaseOperatorNode(operatorNode);
            }
            else
            {
                _locals.Push(n);
            }
        }
        
        _postfixExpression.RemoveRange(
            postfixExpressionStartingLn, _postfixExpression.Count - postfixExpressionStartingLn);

        return _locals.Pop();
    }
    
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
                _tokens[_currentIndex] : throw new IndexOutOfRangeException();

        public LjsToken NextToken => 
            _currentIndex + 1 < _tokens.Count ? 
                _tokens[_currentIndex + 1] : throw new IndexOutOfRangeException();

        public LjsToken PrevToken => 
            _currentIndex > 0 ?
                _tokens[_currentIndex - 1] : throw new IndexOutOfRangeException();

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