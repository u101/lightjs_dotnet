namespace LightJS.Runtime;

/// <summary>
/// base class for any runtime type
/// </summary>
public class LjsObject : IEquatable<LjsObject>
{
    public static readonly LjsObject Null = LjsNull.Instance;
    public static readonly LjsObject Undefined = LjsUndefined.Instance;

    public override string ToString()
    {
        return "{LjsObject}";
    }

    public virtual bool Equals(LjsObject? other)
    {
        return other != null && ReferenceEquals(other, this);
    }
    
    public override bool Equals(object? obj)
    {
        return obj is LjsObject o && Equals(o);
    }
    
    #region type coercing
    
    public static implicit operator LjsObject(string s) => new LjsString(s);
    public static implicit operator LjsObject(int v) => new LjsInteger(v);
    public static implicit operator LjsObject(double v) => new LjsDouble(v);
    public static implicit operator LjsObject(bool v) => v ? LjsBoolean.True : LjsBoolean.False;

    

    #endregion

}