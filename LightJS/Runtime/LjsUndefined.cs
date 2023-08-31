namespace LightJS.Runtime;

public sealed class LjsUndefined : LjsObject
{
    public static readonly LjsUndefined Instance = new();
    
    private LjsUndefined() {}
    
    public override string ToString() => "undefined";
    
    public override bool Equals(LjsObject? other)
    {
        return ReferenceEquals(other, Instance);
    }
}