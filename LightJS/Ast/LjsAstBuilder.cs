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
        
        _tokensReader.MoveForward();
        
        return ReadMain();
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

    private ILjsAstNode ParseExpression()
    {
        return null;
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