using App16.ALang.Ast;
using App16.ALang.Ast.Builders;
using App16.ALang.Js.Tokenizers;

namespace App16.ALang.Js.Ast.Builders;

public sealed class JsControlTransferOperatorProcessor : IAstNodeProcessor
{
    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;

        tokensIterator.MoveForward();
        
        var token = tokensIterator.CurrentToken;

        switch (token.TokenType)
        {
            case JsTokenTypes.Break: return new AstBreak(token);
            case JsTokenTypes.Continue: return new AstContinue(token);
            default:
                throw new Exception($"invalid token type {token.TokenType}");
        }
    }
}

public sealed class JsControlTransferOperatorLookup : IForwardLookup
{
    public bool LookForward(AstTokensIterator tokensIterator)
    {
        var nextTokenType = tokensIterator.NextToken.TokenType;
        
        return nextTokenType == JsTokenTypes.Break ||
               nextTokenType == JsTokenTypes.Continue;
    }
}