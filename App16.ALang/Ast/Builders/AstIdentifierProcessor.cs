using App16.ALang.Tokenizers;

namespace App16.ALang.Ast.Builders;

public sealed class AstIdentifierProcessor : IAstNodeProcessor
{
    private readonly int _identifierTokenType;

    public AstIdentifierProcessor(int identifierTokenType)
    {
        _identifierTokenType = identifierTokenType;
    }

    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;
        
        tokensIterator.CheckExpectedNextAndMoveForward(_identifierTokenType);

        var currentToken = tokensIterator.CurrentToken;
        
        var id = TokenizerUtils.GetTokenStringValue(
            context.SourceCodeString, currentToken);
        
        return new AstGetId(id, currentToken);
    }
}

public sealed class AstIdentifierLookup : IForwardLookup
{
    private readonly int _identifierTokenType;

    public AstIdentifierLookup(int identifierTokenType)
    {
        _identifierTokenType = identifierTokenType;
    }

    public bool LookForward(AstTokensIterator tokensIterator)
    {
        return tokensIterator.NextToken.TokenType == _identifierTokenType;
    }
}