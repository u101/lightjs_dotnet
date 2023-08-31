namespace LightJS.Runtime;

public sealed class LjsString : LjsObject
{
    public string Value { get; }

    public LjsString(string value)
    {
        Value = value ?? 
                throw new ArgumentNullException(nameof(value), "null string value is not supported");
    }

    public override bool Equals(LjsObject? other)
    {
        return other is LjsString s && s.Value == Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }
    
}