namespace App16.LightJS.Runtime;

public sealed class LjsBoolean : LjsObject
{
    public static readonly LjsBoolean True = new(true);
    public static readonly LjsBoolean False = new(false);
    
    public bool Value { get; }

    private LjsBoolean(bool value)
    {
        Value = value;
    }
    
    public override bool Equals(LjsObject? other)
    {
        return other is LjsBoolean b && b.Value == Value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }

    public override string ToString()
    {
        return Value ? "true" : "false";
    }
    
}