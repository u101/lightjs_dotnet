namespace App16.ALang.Ast.Builders;

public sealed class AstModelBuilderContext
{
    public string SourceCodeString { get; }
    public AstTokensIterator TokensIterator { get; }

    private readonly List<IAstProcessorStopPoint> _stopPoints = new()
    {
        AstStopPointEof.Instance
    };

    public AstModelBuilderContext(
        string sourceCodeString,
        AstTokensIterator tokensIterator)
    {
        SourceCodeString = sourceCodeString;
        TokensIterator = tokensIterator;
    }

    public bool ShouldStopAtCurrentPoint()
    {
        for (var i = _stopPoints.Count - 1; i >= 0; --i)
        {
            var p = _stopPoints[i];
            
            if (p.ShouldStop(this)) return true;
            
            if (!p.CheckPreviousPoint) return false;
        }

        return false;
    }

    public void PushStopPoint(IAstProcessorStopPoint stopPoint)
    {
        _stopPoints.Add(stopPoint);
    }

    public void PopStopPoint()
    {
        _stopPoints.RemoveAt(_stopPoints.Count - 1);
    }
}