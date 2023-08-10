namespace LightJS.Ast;

public class LjsAstGetProperty : ILjsAstNode
{
    public ILjsAstNode PropertyName { get; }
    public ILjsAstNode PropertySource { get; }

    public LjsAstGetProperty(ILjsAstNode propertyName, ILjsAstNode propertySource)
    {
        PropertyName = propertyName;
        PropertySource = propertySource;
    }

    public IEnumerable<ILjsAstNode> ChildNodes => new[] { PropertyName, PropertySource };
    public bool HasChildNodes => true;
}