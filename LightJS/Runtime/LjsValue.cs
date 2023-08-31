namespace LightJS.Runtime;

/// <summary>
/// Value type wrap
/// </summary>
public class LjsValue<TValueType> : LjsObject where TValueType : struct
{
    public TValueType Value { get; }

    public LjsValue(TValueType value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString() ?? "null";
    }

    public override bool Equals(LjsObject? other)
    {
        return other is LjsValue<TValueType> b && Value.Equals(b.Value);
    }

    private bool Equals(LjsValue<TValueType> other)
    {
        return EqualityComparer<TValueType>.Default.Equals(Value, other.Value);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((LjsValue<TValueType>)obj);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<TValueType>.Default.GetHashCode(Value);
    }
}