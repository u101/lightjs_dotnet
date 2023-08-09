using LightJS.Tokenizer;

namespace LightJS.Ast;

/// <summary>
/// Abstract syntax tree builder
/// </summary>
public class LjsAstBuilder
{
    private readonly string _sourceCodeString;
    private readonly TokensReader _tokensReader;

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

    public ILjsAstNode Build()
    {
        if (!_tokensReader.HasNextToken)
        {
            throw new Exception("no tokens");
        }
        
        return ParseExpression();
    }

    private ILjsAstNode GetValueNode(LjsToken token)
    {
        switch (token.TokenType)
        {
            case LjsTokenType.IntDecimal:
            case LjsTokenType.IntHex:
            case LjsTokenType.IntBinary:

                return new LjsAstLiteral<int>(
                    LjsTokenizerUtils.GetTokenIntValue(_sourceCodeString, token));
                    
            case LjsTokenType.Float:
            case LjsTokenType.FloatE:

                return new LjsAstLiteral<double>(
                    LjsTokenizerUtils.GetTokenFloatValue(_sourceCodeString, token));
                    
            case LjsTokenType.StringLiteral:
                return new LjsAstLiteral<string>(
                    _sourceCodeString.Substring(token.Position.CharIndex, token.StringLength));
                    
            case LjsTokenType.True:
                        
                return new LjsAstLiteral<bool>(true);
                    
            case LjsTokenType.False:
                        
                return new LjsAstLiteral<bool>(false);
                    
            case LjsTokenType.Null:
                return new LjsAstNull();
                    
            case LjsTokenType.Undefined:
                return new LjsAstUndefined();
                    
                    
            default:
                throw new Exception($"unsupported value token type {token.TokenType}");
        }
    }

    
    private readonly List<ILjsAstNode> _postfixExpression = new();
    private readonly Stack<OperatorNode> _operatorsStack = new();
    private readonly Stack<ILjsAstNode> _locals = new();

    private readonly List<OperatorNode> _opNodesPool = new();

    private readonly Dictionary<LjsTokenType, int> _operatorsPriorityMap = new()
    {
        { LjsTokenType.OpParenthesesOpen, 0},
        { LjsTokenType.OpParenthesesClose, 0},
        
        { LjsTokenType.OpAssign, 10},
        { LjsTokenType.OpPlusAssign, 10},
        { LjsTokenType.OpMinusAssign, 10},
        
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
        
        { LjsTokenType.OpNegate, 1000},
        { LjsTokenType.OpIncrement, 1000},
        { LjsTokenType.OpDecrement, 1000},
    };

    private OperatorNode GetOrCreateOperatorNode(LjsTokenType tokenType, LjsTokenPosition tokenPosition)
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
        None,
        Parentheses,
        Colon
    }

    private ILjsAstNode ParseExpression(StopTokenType stopTokenType = StopTokenType.None)
    {
        // TODO unary operators
        // TODO stop expression advanced

        var operatorsStackStartingLn = _operatorsStack.Count;
        var postfixExpressionStartingLn = _postfixExpression.Count;

        var prevTokenClass = LjsTokenClass.None;
        
        while (_tokensReader.HasNextToken)
        {
            _tokensReader.MoveForward();

            var token = _tokensReader.CurrentToken;
            var tokenType = token.TokenType;
            var tokenPosition = token.Position;


            if ((token.TokenClass & LjsTokenClass.Literal) != 0)
            {
                if (prevTokenClass != LjsTokenClass.None &&
                    (prevTokenClass & LjsTokenClass.Operator) == 0)
                    throw new LjsSyntaxError("unexpected token", tokenPosition);
                
                
                var valueNode = GetValueNode(token);
                _postfixExpression.Add(valueNode);
                
                prevTokenClass = token.TokenClass;
            }
            
            else
            {
                
                if (tokenType == LjsTokenType.Identifier)
                {
                    if (prevTokenClass != LjsTokenClass.None &&
                        (prevTokenClass & LjsTokenClass.Operator) == 0)
                        throw new LjsSyntaxError("unexpected token", tokenPosition);
                
                    var id = _sourceCodeString.Substring(tokenPosition.CharIndex, token.StringLength);
                    _postfixExpression.Add(new LjsAstGetVar(id));
                
                    prevTokenClass = token.TokenClass;
                }
                
                else if (tokenType == LjsTokenType.OpQuestionMark)
                {
                    if (_postfixExpression.Count - postfixExpressionStartingLn == 0)
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
                    if ((stopTokenType & StopTokenType.Colon) != 0)
                    {
                        return BuildExpression(operatorsStackStartingLn, postfixExpressionStartingLn);
                    }
                    
                    throw new LjsSyntaxError("unexpected colon", tokenPosition);
                }
            
                else if (tokenType == LjsTokenType.OpParenthesesOpen)
                {
                    if (prevTokenClass != LjsTokenClass.None &&
                        (prevTokenClass & LjsTokenClass.Operator) == 0)
                        throw new LjsSyntaxError("unexpected token", tokenPosition);

                    prevTokenClass = LjsTokenClass.Word;

                    var exp = ParseExpression(StopTokenType.Parentheses);

                    if (_tokensReader.CurrentToken.TokenType != LjsTokenType.OpParenthesesClose)
                    {
                        throw new LjsSyntaxError("unclosed parentheses", tokenPosition);
                    }
                    
                    _postfixExpression.Add(exp);
                }
            
                else if (tokenType == LjsTokenType.OpParenthesesClose)
                {
                    if ((stopTokenType & StopTokenType.Parentheses) != 0)
                    {
                        return BuildExpression(operatorsStackStartingLn, postfixExpressionStartingLn);
                    }
                    
                    throw new LjsSyntaxError("unexpected parentheses", tokenPosition);
                }
            
                else if ((token.TokenClass & LjsTokenClass.Operator) != 0 && 
                         _operatorsPriorityMap.ContainsKey(tokenType))
                {
                    if ((prevTokenClass == LjsTokenClass.None || 
                         (prevTokenClass & LjsTokenClass.BinaryOperator) != 0) &&
                        (token.TokenClass & LjsTokenClass.BinaryOperator) != 0)
                    {
                        throw new LjsSyntaxError("unexpected token", tokenPosition);
                    }

                    prevTokenClass = token.TokenClass;
                    

                    //	Заносим в выходную строку все операторы из стека, имеющие более высокий приоритет
                    while (_operatorsStack.Count > operatorsStackStartingLn && 
                           (_operatorsPriorityMap[_operatorsStack.Peek().OperatorType] >= _operatorsPriorityMap[tokenType]))
                        _postfixExpression.Add(_operatorsStack.Pop());
                
                    //	Заносим в стек оператор
                    _operatorsStack.Push(GetOrCreateOperatorNode(tokenType, token.Position));
                }
                else
                {
                    throw new Exception("unexpected token");
                }
            }
        }

        return BuildExpression(operatorsStackStartingLn, postfixExpressionStartingLn);
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
                
                _locals.Push(new LjsAstBinaryOperation(leftOperand, rightOperand, operatorNode.OperatorType));
                
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