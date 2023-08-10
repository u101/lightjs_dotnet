namespace LightJS.Ast;

public class LjsAstGetNamedProperty : ILjsAstNode
{
    public string PropertyName { get; }
    public ILjsAstNode PropertySource { get; }

    public LjsAstGetNamedProperty(string propertyName, ILjsAstNode propertySource)
    {
        PropertyName = propertyName;
        PropertySource = propertySource;
    }

    public IEnumerable<ILjsAstNode> ChildNodes => new[] { PropertySource };
    public bool HasChildNodes => true;
}