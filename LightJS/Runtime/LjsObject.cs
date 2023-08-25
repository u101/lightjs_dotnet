namespace LightJS.Runtime;

/// <summary>
/// base class for any runtime type
/// </summary>
public class LjsObject
{

    public static readonly LjsObject Null = new LjsNull();
    public static readonly LjsObject Undefined = new LjsUndefined();

    public override string ToString()
    {
        return "{LjsObject}";
    }
    
    private sealed class LjsNull : LjsObject
    {
        public LjsNull() {}
        public override string ToString() => "null";
    }
    
    private sealed class LjsUndefined : LjsObject
    {
        public LjsUndefined() {}
        public override string ToString() => "undefined";
    }
}