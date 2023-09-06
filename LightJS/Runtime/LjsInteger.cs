namespace LightJS.Runtime;

public sealed class LjsInteger : LjsNumber
{
    public static readonly LjsInteger Zero = new(0);
    public static readonly LjsInteger One = new(1);
    public static readonly LjsInteger MinusOne = new(-1);
    
    public int Value { get; }

    public override double NumericValue => Value;

    public override int IntegerValue => Value;

    public LjsInteger(int value)
    {
        Value = value;
    }

    public override bool Equals(LjsObject? other)
    {
        return other is LjsInteger d && d.Value == Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}