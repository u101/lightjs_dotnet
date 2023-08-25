using System.Diagnostics.CodeAnalysis;

namespace LightJS.Runtime;

/// <summary>
/// Value type wrap
/// </summary>
public class LjsValue<TValueType> : LjsObject
{
    public TValueType Value { get; }

    public LjsValue(
        [DisallowNull] TValueType value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value?.ToString() ?? "null";
    }

    protected bool Equals(LjsValue<TValueType> other)
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
        return EqualityComparer<TValueType>.Default.GetHashCode(Value!);
    }
}

public static class LjsValue
{
    public static readonly LjsValue<bool> True = new (true);
    public static readonly LjsValue<bool> False = new (false);
}