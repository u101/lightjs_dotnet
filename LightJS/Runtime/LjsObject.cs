namespace LightJS.Runtime;

/// <summary>
/// base class for any runtime type
/// </summary>
public class LjsObject : IEquatable<LjsObject>
{
    public static readonly LjsTypeInfo TypeInfo = new(new Dictionary<string, LjsObject>()
    {
        { "toString", new ToStringFunction() }
    });
    
    public static readonly LjsObject Null = LjsNull.Instance;
    public static readonly LjsObject Undefined = LjsUndefined.Instance;

    public virtual LjsTypeInfo GetTypeInfo() => TypeInfo;

    public override string ToString() =>  $"{{{GetType().Name}}}";

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
    public static implicit operator LjsObject(int[] v) => new LjsArray(v.Select(i => new LjsInteger(i)));

    #endregion

    private sealed class ToStringFunction : LjsFunction
    {
        public override LjsMemberType MemberType => LjsMemberType.InstanceMember;
        public override int ArgumentsCount => 1;
        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            return arguments[0].ToString();
        }
    }
    
}