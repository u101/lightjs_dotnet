namespace LightJS.Runtime;

/// <summary>
/// base class for any runtime type
/// </summary>
public class LjsObject : IEquatable<LjsObject>
{

    public static readonly LjsObject Null = new LjsNull();
    public static readonly LjsObject Undefined = new LjsUndefined();

    public override string ToString()
    {
        return "{LjsObject}";
    }

    public virtual bool Equals(LjsObject? other)
    {
        return other != null && other == this;
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
    
    #region type coercing
    
    public static implicit operator LjsObject(string s) => new LjsValue<string>(s);
    public static implicit operator LjsObject(int v) => new LjsValue<int>(v);
    public static implicit operator LjsObject(double v) => new LjsValue<double>(v);
    public static implicit operator LjsObject(bool v) => v ? LjsValue.True : LjsValue.False;

    #endregion

}