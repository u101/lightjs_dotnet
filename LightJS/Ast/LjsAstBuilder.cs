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
        
        { LjsTokenType.OpPlus, new OperatorNode(LjsTokenType.OpAssign, 10)},
        
        { LjsTokenType.OpMinus, new OperatorNode(LjsTokenType.OpEquals, 50)},
        { LjsTokenType.OpMinus, new OperatorNode(LjsTokenType.OpEqualsStrict, 50)},
        { LjsTokenType.OpMinus, new OperatorNode(LjsTokenType.OpGreater, 50)},
        { LjsTokenType.OpMinus, new OperatorNode(LjsTokenType.OpGreaterOrEqual, 50)},
        { LjsTokenType.OpMinus, new OperatorNode(LjsTokenType.OpLess, 50)},
        { LjsTokenType.OpMinus, new OperatorNode(LjsTokenType.OpLessOrEqual, 50)},
        
        { LjsTokenType.OpMinus, new OperatorNode(LjsTokenType.OpLogicalAnd, 80)},
        { LjsTokenType.OpMinus, new OperatorNode(LjsTokenType.OpLogicalOr, 80)},
        
        { LjsTokenType.OpPlus, new OperatorNode(LjsTokenType.OpPlus, 100)},
        { LjsTokenType.OpMinus, new OperatorNode(LjsTokenType.OpMinus, 100)},
        
        { LjsTokenType.OpMultiply, new OperatorNode(LjsTokenType.OpMultiply, 200)},
        { LjsTokenType.OpDiv, new OperatorNode(LjsTokenType.OpDiv, 200)},
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

    private ILjsAstNode ParseExpression()
    {
        while (_tokensReader.HasNextToken)
        {
            _tokensReader.MoveForward();

            var token = _tokensReader.CurrentToken;

            if (token.TokenClass == LjsTokenClass.Value)
            {
                var valueNode = GetValueNode(token);
                _postfixExpression.Add(valueNode);
            }
            else if (token.TokenType == LjsTokenType.OpParenthesesOpen)
            {
                _operatorsStack.Push(_operatorNodesMap[LjsTokenType.OpParenthesesOpen]);
            }
            else if (token.TokenType == LjsTokenType.OpParenthesesClose)
            {
                //	Заносим в выходную строку из стека всё вплоть до открывающей скобки
                while (_operatorsStack.Count > 0 &&
                       _operatorsStack.Peek().OperatorType != LjsTokenType.OpParenthesesOpen)
                {
                    _postfixExpression.Add(_operatorsStack.Pop());
                }

                _operatorsStack.Pop();
            }
            else if (token.TokenClass == LjsTokenClass.Operator && 
                     _operatorNodesMap.ContainsKey(token.TokenType))
            {
                //	Заносим в выходную строку все операторы из стека, имеющие более высокий приоритет
                while (_operatorsStack.Count > 0 && 
                       (_operatorNodesMap[_operatorsStack.Peek().OperatorType].Priority >= _operatorNodesMap[token.TokenType].Priority))
                    _postfixExpression.Add(_operatorsStack.Pop());
                
                //	Заносим в стек оператор
                _operatorsStack.Push(_operatorNodesMap[token.TokenType]);
            }

        }
        
        //	Заносим все оставшиеся операторы из стека в выходную строку
        foreach (var op in _operatorsStack)
        {
            _postfixExpression.Add(op);
        }

        //	Проходим по строке
        for (var i = 0; i < _postfixExpression.Count; i++)
        {
            //	Текущий символ
            var c = _postfixExpression[i];

            if (c is OperatorNode operatorNode)
            {

                //	Получаем значения из стека в обратном порядке
                var rightOperand = _locals.Pop();
                var leftOperand = _locals.Pop();

                //	Получаем результат операции и заносим в стек
                _locals.Push(new LsjAstBinaryOperation(leftOperand, rightOperand, operatorNode.OperatorType));
            }
            else
            {
                _locals.Push(c);
            }
        }

        return _locals.Pop();
    }
    
    

    private ILjsAstNode ReadMain()
    {
        var token = _tokensReader.CurrentToken;

        switch (token.TokenClass)
        {
            case LjsTokenClass.Word:

                switch (token.TokenType)
                {
                    
                    case LjsTokenType.Identifier:
                        
                        break;
                    
                }
                
                throw new NotImplementedException();
            
            case LjsTokenClass.Value:

                return GetValueNode(token);
            
            default:
                throw new ArgumentOutOfRangeException();
        }

        
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
    }
    
}