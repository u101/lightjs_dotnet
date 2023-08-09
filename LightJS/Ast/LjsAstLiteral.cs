namespace LightJS.Ast;

public abstract class LjsAstLiteral : LjsAstLeafNode
{
    
    public abstract object ObjectValue { get; }
    
}

public class LjsAstLiteral<TValue> : LjsAstLiteral
{
    public TValue Value { get; }

    public LjsAstLiteral(TValue value)
    {
        Value = value;
    }

    public override object ObjectValue => (object) Value;
    
}

public class LjsAstNull : LjsAstLiteral
{
    public LjsAstNull() {}
    
    public override object ObjectValue => null;
}

public class LjsAstUndefined : LjsAstLiteral
{
    public LjsAstUndefined() {}
    
    public override object ObjectValue => null;
}