using System.Globalization;

namespace LightJS.Runtime;

public sealed class LjsDouble : LjsNumber
{
    public double Value { get; }

    public override double NumericValue => Value;

    public LjsDouble(double value)
    {
        Value = value;
    }

    public override bool Equals(LjsObject? other)
    {
        return other is LjsDouble d && 
               EqualityComparer<double>.Default.Equals(Value, d.Value);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}