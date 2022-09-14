namespace LightJS;

/// <summary>
/// Abstract syntax tree builder
/// </summary>
public class LjsAstBuilder
{
    private readonly LjsSourceCode _sourceCode;

    public LjsAstBuilder(LjsSourceCode sourceCode)
    {
        _sourceCode = sourceCode;
    }

    public LjsAstModel Build(List<LjsToken> tokens)
    {
        return new LjsAstModel();
    }
    
}