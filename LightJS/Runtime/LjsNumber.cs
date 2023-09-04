namespace LightJS.Runtime;

public abstract class LjsNumber : LjsObject
{
    public abstract double NumericValue { get; }
    
    public abstract int IntegerValue { get; }
}