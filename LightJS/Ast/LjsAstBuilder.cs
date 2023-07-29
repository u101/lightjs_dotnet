using LightJS.Tokenizer;

namespace LightJS.Ast;

/// <summary>
/// Abstract syntax tree builder
/// </summary>
public class LjsAstBuilder
{
    private readonly LjsSourceCode _sourceCode;

    public LjsAstBuilder(LjsSourceCode sourceCode)
    {
        _sourceCode = sourceCode ?? throw new ArgumentNullException(nameof(sourceCode));
    }

    public LjsAstModel Build(List<LjsToken> tokens)
    {
        if (tokens == null)
            throw new ArgumentNullException(nameof(tokens));

        if (tokens.Count == 0)
            throw new ArgumentException("empty tokens list");
        
        var topLevelNodes = new List<ILjsAstNode>();

        var tokenIndex = 0;

        while (tokenIndex < tokens.Count)
        {
            var node = ReadMain(tokens, ref tokenIndex);
            topLevelNodes.Add(node);
        }
        
        return new LjsAstModel(topLevelNodes);
    }

    private ILjsAstNode ReadMain(List<LjsToken> tokens, ref int currentIndex)
    {
        var token = tokens[currentIndex];
        var tokenPosition = token.Position;

        switch (token.TokenType)
        {
            case LjsTokenType.Word:
                throw new NotImplementedException();
                break;
            
            case LjsTokenType.Int:
                ++currentIndex;
                return new LjsAstValue<int>(
                    _sourceCode.ReadInt(tokenPosition.CharIndex, token.StringLength));
            
            case LjsTokenType.Float:
                ++currentIndex;
                return new LjsAstValue<double>(
                    _sourceCode.ReadDouble(tokenPosition.CharIndex, token.StringLength));
            
            case LjsTokenType.String:
                ++currentIndex;
                return new LjsAstValue<string>(
                    _sourceCode.Substring(tokenPosition.CharIndex, token.StringLength));
                
            case LjsTokenType.Operator:
                
                throw new NotImplementedException();
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }

        
    }
    
}