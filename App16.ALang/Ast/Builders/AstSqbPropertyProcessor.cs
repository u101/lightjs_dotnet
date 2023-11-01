namespace App16.ALang.Ast.Builders;

public sealed class AstSqbPropertyProcessor : IAstDecoratorProcessor
{
    private readonly int _openTokenType;
    private readonly int _closeTokenType;
    private readonly IAstNodeProcessor _expressionProcessor;

    private readonly AstStopPointBeforeToken _stopPoint;

    public AstSqbPropertyProcessor(
        int openTokenType,
        int closeTokenType,
        IAstNodeProcessor expressionProcessor)
    {
        _openTokenType = openTokenType;
        _closeTokenType = closeTokenType;
        _expressionProcessor = expressionProcessor;
        _stopPoint = new AstStopPointBeforeToken(closeTokenType, false);
    }

    public IAstNode ProcessNext(IAstNode decoratee, AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;
        
        tokensIterator.CheckExpectedNextAndMoveForward(_openTokenType);

        var token = tokensIterator.CurrentToken;
        
        context.PushStopPoint(_stopPoint);

        var result = _expressionProcessor.ProcessNext(context);
        
        context.PopStopPoint();

        tokensIterator.CheckExpectedNextAndMoveForward(_closeTokenType);

        return new AstGetSquareBracketsProp(decoratee, result, token);
    }
}

public sealed class AstSqbPropertyLookup : IDecoratorForwardLookup
{
    private readonly int _openTokenType;

    public AstSqbPropertyLookup(int openTokenType)
    {
        _openTokenType = openTokenType;
    }

    public bool LookForward(IAstNode decoratee, AstTokensIterator tokensIterator)
    {
        return decoratee is IAstValueNode &&
               tokensIterator.LookForward(1).TokenType == _openTokenType;
    }
}