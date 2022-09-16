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