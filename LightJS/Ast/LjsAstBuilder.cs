using LightJS.Tokenizer;
using LightJS.Utils;

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

    private ILjsAstNode ReadMain()
    {
        var token = _tokensReader.CurrentToken;
        var tokenPosition = token.Position;

        switch (token.TokenType)
        {
            case LjsTokenType.Word:

                var word = _sourceCodeString.Substring(
                    tokenPosition.CharIndex, token.StringLength);

                switch (word)
                {
                    case LjsKeywords.Var:

                        if (!_tokensReader.HasNextToken)
                            throw new LjsSyntaxError("expected identifier after 'var'", tokenPosition);
                        
                        return null;
                    default:
                        throw new NotImplementedException();
                }
                break;
            
            case LjsTokenType.Int:
                
                return new LjsAstValue<int>(
                    _sourceCodeString.ReadInt(tokenPosition.CharIndex, token.StringLength));
            
            case LjsTokenType.Float:
                
                return new LjsAstValue<double>(
                    _sourceCodeString.ReadDouble(tokenPosition.CharIndex, token.StringLength));
            
            case LjsTokenType.String:
                
                return new LjsAstValue<string>(
                    _sourceCodeString.Substring(tokenPosition.CharIndex, token.StringLength));
                
            case LjsTokenType.Operator:
                
                throw new NotImplementedException();
                break;
            
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