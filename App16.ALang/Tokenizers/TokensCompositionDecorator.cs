namespace App16.ALang.Tokenizers;

public class TokensCompositionDecorator : ITokensProcessor
{
    private readonly ITokensProcessor _decoratee;
    private readonly TokensCompositionMap _compositionMap;
    private readonly bool _tokensRequireToBeAdjacent;

    public TokensCompositionDecorator(
        ITokensProcessor decoratee,
        TokensCompositionMap compositionMap,
        bool tokensRequireToBeAdjacent)
    {
        _decoratee = decoratee;
        _compositionMap = compositionMap;
        _tokensRequireToBeAdjacent = tokensRequireToBeAdjacent;
    }
    
    public bool Process(TokenizerContext ctx)
    {
        if (!_decoratee.Process(ctx)) return false;

        if (ctx.Tokens.Count < 2) return true;
        
        var token0 = ctx.Tokens[^2];
        var token1 = ctx.Tokens[^1];

        if (
            (!_tokensRequireToBeAdjacent || token0.Position.IsAdjacentTo(token1.Position, token0.StringLength)) &&
            _compositionMap.TryGetComposition(
                token0.TokenType, 
                token1.TokenType, out var result))
        {
            ctx.RemoveLastToken();
            ctx.RemoveLastToken();
                
            ctx.AddToken(new Token(
                result, token0.Position, (token1.Position.CharIndex - token0.Position.CharIndex) + token1.StringLength));
        }

        return true;
    }
}