namespace App16.ALang.Ast.Builders;

public interface IAstProcessorStopPoint
{
    bool CheckPreviousPoint { get; }
    
    bool ShouldStop(AstModelBuilderContext context);
}

public sealed class AstStopPointEof : IAstProcessorStopPoint
{
    public static readonly AstStopPointEof Instance = new();
    
    private AstStopPointEof() {}

    public bool CheckPreviousPoint => false;

    public bool ShouldStop(AstModelBuilderContext context)
    {
        return !context.TokensIterator.HasNextToken;
        
    }
}

public sealed class AstStopPointBeforeToken : IAstProcessorStopPoint
{
    public bool CheckPreviousPoint { get; }
    private readonly int _tokenType;

    public AstStopPointBeforeToken(int tokenType, bool checkPreviousPoint)
    {
        CheckPreviousPoint = checkPreviousPoint;
        _tokenType = tokenType;
    }
    
    public bool ShouldStop(AstModelBuilderContext context)
    {
        return context.TokensIterator.NextToken.TokenType == _tokenType;
    }
}