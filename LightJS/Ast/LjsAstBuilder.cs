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

                return new LjsAstValue<int>(
                    LjsTokenizerUtils.GetTokenIntValue(_sourceCodeString, token));
                    
            case LjsTokenType.Float:
            case LjsTokenType.FloatE:

                return new LjsAstValue<double>(
                    LjsTokenizerUtils.GetTokenFloatValue(_sourceCodeString, token));
                    
            case LjsTokenType.StringLiteral:
                return new LjsAstValue<string>(
                    _sourceCodeString.Substring(token.Position.CharIndex, token.StringLength));
                    
            case LjsTokenType.True:
                        
                return new LjsAstValue<bool>(true);
                    
            case LjsTokenType.False:
                        
                return new LjsAstValue<bool>(false);
                    
            case LjsTokenType.Null:
                return LjsAstNull.Instance;
                    
            case LjsTokenType.Undefined:
                return LjsAstUndefined.Instance;
                    
                    
            default:
                throw new Exception($"unsupported value token type {token.TokenType}");
        }
    }

    
    private readonly List<ILjsAstNode> _postfixExpression = new();
    private readonly Stack<OperatorNode> _operatorsStack = new();
    private readonly Stack<ILjsAstNode> _locals = new();

    private readonly Dictionary<LjsTokenType, OperatorNode> _operatorNodesMap = new()
    {
        { LjsTokenType.OpParenthesesOpen, new OperatorNode(LjsTokenType.OpParenthesesOpen, 0)},
        { LjsTokenType.OpParenthesesClose, new OperatorNode(LjsTokenType.OpParenthesesClose, 0)},
        
        { LjsTokenType.OpAssign, new OperatorNode(LjsTokenType.OpAssign, 10)},
        { LjsTokenType.OpPlusAssign, new OperatorNode(LjsTokenType.OpPlusAssign, 10)},
        { LjsTokenType.OpMinusAssign, new OperatorNode(LjsTokenType.OpMinusAssign, 10)},
        
        { LjsTokenType.OpEquals, new OperatorNode(LjsTokenType.OpEquals, 50)},
        { LjsTokenType.OpNotEqual, new OperatorNode(LjsTokenType.OpNotEqual, 50)},
        { LjsTokenType.OpEqualsStrict, new OperatorNode(LjsTokenType.OpEqualsStrict, 50)},
        { LjsTokenType.OpGreater, new OperatorNode(LjsTokenType.OpGreater, 50)},
        { LjsTokenType.OpGreaterOrEqual, new OperatorNode(LjsTokenType.OpGreaterOrEqual, 50)},
        { LjsTokenType.OpLess, new OperatorNode(LjsTokenType.OpLess, 50)},
        { LjsTokenType.OpLessOrEqual, new OperatorNode(LjsTokenType.OpLessOrEqual, 50)},
        
        { LjsTokenType.OpLogicalAnd, new OperatorNode(LjsTokenType.OpLogicalAnd, 80)},
        { LjsTokenType.OpLogicalOr, new OperatorNode(LjsTokenType.OpLogicalOr, 80)},
        
        { LjsTokenType.OpPlus, new OperatorNode(LjsTokenType.OpPlus, 100)},
        { LjsTokenType.OpMinus, new OperatorNode(LjsTokenType.OpMinus, 100)},
        
        { LjsTokenType.OpMultiply, new OperatorNode(LjsTokenType.OpMultiply, 200)},
        { LjsTokenType.OpDiv, new OperatorNode(LjsTokenType.OpDiv, 200)},
        
        { LjsTokenType.OpNegate, new OperatorNode(LjsTokenType.OpNegate, 1000)},
        { LjsTokenType.OpIncrement, new OperatorNode(LjsTokenType.OpIncrement, 1000)},
        { LjsTokenType.OpDecrement, new OperatorNode(LjsTokenType.OpDecrement, 1000)},
    };

    private class OperatorNode : ILjsAstNode
    {
        public LjsTokenType OperatorType { get; }
        public int Priority { get; }

        public OperatorNode(LjsTokenType operatorType, int priority)
        {
            OperatorType = operatorType;
            Priority = priority;
        }
        
        public IEnumerable<ILjsAstNode> ChildNodes => Array.Empty<ILjsAstNode>();
        public bool HasChildNodes => false;
    }
    
    private enum OperationType
    {
        Binary,
        Unary,
        Ternary,
        Both
    }

    private static OperationType GetOperationType(LjsTokenType tokenType)
    {
        if (tokenType == LjsTokenType.OpQuestionMark) 
            return OperationType.Ternary;
        
        if (tokenType == LjsTokenType.OpPlus || 
            tokenType == LjsTokenType.OpMinus) return OperationType.Both;

        if (tokenType == LjsTokenType.OpNegate ||
            tokenType == LjsTokenType.OpIncrement ||
            tokenType == LjsTokenType.OpDecrement) return OperationType.Unary;

        return OperationType.Binary;
    }
    
    [Flags]
    private enum StopTokenType
    {
        None,
        Parentheses
    }

    private ILjsAstNode ParseExpression(StopTokenType stopTokenType = StopTokenType.None)
    {
        // TODO parentheses recursion
        // TODO unary operators
        // TODO ternary operators

        var operatorsStackStartingLn = _operatorsStack.Count;
        var postfixExpressionStartingLn = _postfixExpression.Count;

        var prevOperand = false;
        
        while (_tokensReader.HasNextToken)
        {
            _tokensReader.MoveForward();

            var token = _tokensReader.CurrentToken;
            var tokenType = token.TokenType;
            var tokenPosition = token.Position;


            if (token.TokenClass == LjsTokenClass.Value)
            {
                if (prevOperand) 
                    throw new LjsSyntaxError("unexpected token", tokenPosition);
                
                var valueNode = GetValueNode(token);
                _postfixExpression.Add(valueNode);
                
                prevOperand = true;
            }
            
            else
            {
                
                if (tokenType == LjsTokenType.Identifier)
                {
                    if (prevOperand) 
                        throw new LjsSyntaxError("unexpected token", tokenPosition);
                
                    var id = _sourceCodeString.Substring(tokenPosition.CharIndex, token.StringLength);
                    _postfixExpression.Add(new LjsAstGetVar(id));
                
                    prevOperand = true;
                }
            
                else if (tokenType == LjsTokenType.OpParenthesesOpen)
                {
                    if (prevOperand) 
                        throw new LjsSyntaxError("unexpected token", tokenPosition);

                    prevOperand = true;

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
                        break;
                    }
                    
                    throw new LjsSyntaxError("unexpected parentheses", tokenPosition);
                }
            
                else if (token.TokenClass == LjsTokenClass.Operator && 
                         _operatorNodesMap.ContainsKey(tokenType))
                {
                    if (!prevOperand)
                    {
                        throw new LjsSyntaxError("unexpected token", tokenPosition);
                    }

                    prevOperand = false;
                    

                    //	Заносим в выходную строку все операторы из стека, имеющие более высокий приоритет
                    while (_operatorsStack.Count > operatorsStackStartingLn && 
                           (_operatorNodesMap[_operatorsStack.Peek().OperatorType].Priority >= _operatorNodesMap[tokenType].Priority))
                        _postfixExpression.Add(_operatorsStack.Pop());
                
                    //	Заносим в стек оператор
                    _operatorsStack.Push(_operatorNodesMap[tokenType]);
                }
                else
                {
                    throw new Exception("unexpected token");
                }
            }
        }

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