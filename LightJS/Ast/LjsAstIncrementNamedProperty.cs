namespace LightJS.Ast;

public sealed class LjsAstIncrementNamedProperty : ILjsAstNode
{
    public string PropertyName { get; }
    public ILjsAstNode PropertySource { get; }
    public LjsAstIncrementSign Sign { get; }
    public LjsAstIncrementOrder Order { get; }

    public LjsAstIncrementNamedProperty(
        string propertyName, 
        ILjsAstNode propertySource,
        LjsAstIncrementSign sign, 
        LjsAstIncrementOrder order)
    {
        PropertyName = propertyName;
        PropertySource = propertySource;
        Sign = sign;
        Order = order;
    }
}