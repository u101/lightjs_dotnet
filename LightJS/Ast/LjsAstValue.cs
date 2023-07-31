namespace LightJS.Ast;

public abstract class LjsAstValue : LjsAstLeafNode
{
    
    public abstract object ObjectValue { get; }
    
}

public class LjsAstValue<TValue> : LjsAstValue
{
    public TValue Value { get; }

    public LjsAstValue(TValue value)
    {
        Value = value;
    }

    public override object ObjectValue => (object) Value;
    
}

public class LjsAstNull : LjsAstValue
{

    public static readonly LjsAstNull Instance = new();
    
    private LjsAstNull() {}
    
    public override object ObjectValue => null;
}

public class LjsAstUndefined : LjsAstValue
{

    public static readonly LjsAstUndefined Instance = new();
    
    private LjsAstUndefined() {}
    
    public override object ObjectValue => null;
}