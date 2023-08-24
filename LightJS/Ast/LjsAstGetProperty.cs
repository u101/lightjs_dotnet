namespace LightJS.Ast;

public sealed class LjsAstGetProperty : ILjsAstNode
{
    public ILjsAstNode PropertyName { get; }
    public ILjsAstNode PropertySource { get; }

    public LjsAstGetProperty(ILjsAstNode propertyName, ILjsAstNode propertySource)
    {
        PropertyName = propertyName;
        PropertySource = propertySource;
    }
}