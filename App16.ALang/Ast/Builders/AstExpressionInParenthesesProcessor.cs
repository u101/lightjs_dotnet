namespace App16.ALang.Ast.Builders;

public class AstExpressionInParenthesesProcessor : IAstNodeProcessor
{
    private readonly int _parenthesesOpenTokenType;
    private readonly int _parenthesesCloseTokenType;
    private readonly IAstNodeProcessor _expressionProcessor;

    private readonly AstStopPointBeforeToken _stopPoint;

    public AstExpressionInParenthesesProcessor(
        int parenthesesOpenTokenType,
        int parenthesesCloseTokenType,
        IAstNodeProcessor expressionProcessor
        )
    {
        _parenthesesOpenTokenType = parenthesesOpenTokenType;
        _parenthesesCloseTokenType = parenthesesCloseTokenType;
        _expressionProcessor = expressionProcessor;
        _stopPoint = new AstStopPointBeforeToken(_parenthesesCloseTokenType, false);
    }
    
    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;
        
        tokensIterator.CheckExpectedNextAndMoveForward(_parenthesesOpenTokenType);
        
        context.PushStopPoint(_stopPoint);

        var result = _expressionProcessor.ProcessNext(context);
        
        context.PopStopPoint();

        tokensIterator.CheckExpectedNextAndMoveForward(_parenthesesCloseTokenType);

        return result;
    }
}

public sealed class AstExpressionInParenthesesLookup : IForwardLookup
{
    private readonly int _parenthesesOpenTokenType;

    public AstExpressionInParenthesesLookup(
        int parenthesesOpenTokenType
    )
    {
        _parenthesesOpenTokenType = parenthesesOpenTokenType;
    }
    
    public bool LookForward(AstTokensIterator tokensIterator)
    {
        return tokensIterator.NextToken.TokenType == _parenthesesOpenTokenType;
    }
}