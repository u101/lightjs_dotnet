namespace LightJS.Ast;

public sealed class LjsAstIncrementProperty:ILjsAstNode
{
    public ILjsAstNode PropertyName { get; }
    public ILjsAstNode PropertySource { get; }
    public LjsAstIncrementSign Sign { get; }
    public LjsAstIncrementOrder Order { get; }

    public LjsAstIncrementProperty(
        ILjsAstNode propertyName, 
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