namespace LightJS.Runtime;

public sealed class LjsNull : LjsObject
{
    public static readonly LjsNull Instance = new();
    
    private LjsNull() {}
    
    public override string ToString() => "null";
    
    public override bool Equals(LjsObject? other)
    {
        return ReferenceEquals(other, Instance);
    }
}