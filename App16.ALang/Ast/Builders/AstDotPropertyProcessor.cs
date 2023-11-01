using App16.ALang.Tokenizers;

namespace App16.ALang.Ast.Builders;

public sealed class AstDotPropertyProcessor : IAstDecoratorProcessor
{
    private readonly int _dotTokenType;
    private readonly int _identifierTokenType;

    public AstDotPropertyProcessor(
        int dotTokenType,
        int identifierTokenType)
    {
        _dotTokenType = dotTokenType;
        _identifierTokenType = identifierTokenType;
    }

    public IAstNode ProcessNext(IAstNode decoratee, AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;
        
        tokensIterator.CheckExpectedNextAndMoveForward(_dotTokenType);
        tokensIterator.CheckExpectedNextAndMoveForward(_identifierTokenType);

        var identifierToken = tokensIterator.CurrentToken;

        var propertyId = TokenizerUtils.GetTokenStringValue(context.SourceCodeString, identifierToken);

        return new AstGetDotProperty(decoratee, propertyId, identifierToken);
    }
}

public sealed class AstDotPropertyLookup : IDecoratorForwardLookup
{
    
    private readonly int _dotTokenType;
    private readonly int _identifierTokenType;

    public AstDotPropertyLookup(
        int dotTokenType,
        int identifierTokenType)
    {
        _dotTokenType = dotTokenType;
        _identifierTokenType = identifierTokenType;
    }
    
    public bool LookForward(IAstNode decoratee, AstTokensIterator tokensIterator)
    {
        return decoratee is IAstValueNode &&
               tokensIterator.LookForward(1).TokenType == _dotTokenType && 
               tokensIterator.LookForward(2).TokenType == _identifierTokenType;
    }
}

