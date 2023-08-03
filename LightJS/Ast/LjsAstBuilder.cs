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

    public LjsAstModel Build()
    {
        
        var topLevelNodes = new List<ILjsAstNode>();

        while (_tokensReader.HasNextToken)
        {
            _tokensReader.MoveForward();
            
            var node = ReadMain();
            topLevelNodes.Add(node);
        }
        
        return new LjsAstModel(topLevelNodes);
    }

    public ILjsAstNode BuildExpression()
    {
        while (_tokensReader.HasNextToken)
        {
            _tokensReader.MoveForward();
            
            var node = ReadMain();

            return node;
        }

        throw new Exception("empty script");
    }

    private ILjsAstNode ReadMain()
    {
        var token = _tokensReader.CurrentToken;
        var tokenPosition = token.Position;

        switch (token.TokenClass)
        {
            case LjsTokenClass.Word:

                var word = LjsTokenizerUtils.GetTokenStringValue(
                    _sourceCodeString, token);

                if (IsKeyword(word))
                {
                    switch (word)
                    {
                        case LjsKeywords.Var:

                            if (!_tokensReader.HasNextToken || 
                                _tokensReader.NextToken.TokenClass != LjsTokenClass.Word)
                                throw new LjsSyntaxError("expected identifier after 'var'", tokenPosition);
                            
                            _tokensReader.MoveForward();

                            var v = LjsTokenizerUtils.GetTokenStringValue(
                                _sourceCodeString, _tokensReader.CurrentToken);

                            if (IsKeyword(v))
                                throw new LjsSyntaxError(
                                    $"unexpected {v} after 'var'", _tokensReader.CurrentToken.Position);

                            var varNode = new LjsAstVar();
                            
                            

                            return null;
                        default:
                            throw new NotImplementedException();
                    }
                }
                
                throw new NotImplementedException();
                break;
            
            case LjsTokenClass.Value:

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
                            _sourceCodeString.Substring(tokenPosition.CharIndex, token.StringLength));
                    
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
                
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }

        
    }

    private static bool IsKeyword(string word)
    {
        return LjsKeywords.AllKeywords.Contains(word);
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