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

        switch (token.TokenType)
        {
            case LjsTokenType.Null:
                break;
            case LjsTokenType.Word:
                break;
            case LjsTokenType.Int:
                break;
            case LjsTokenType.Float:
                break;
            case LjsTokenType.String:
                break;
            case LjsTokenType.Operator:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        throw new NotImplementedException();
    }
    
}