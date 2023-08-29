namespace LightJS.Runtime;

public sealed class LjsFunctionPointer : LjsObject
{
    public int FunctionIndex { get; }

    public LjsFunctionPointer(int functionIndex)
    {
        FunctionIndex = functionIndex;
    }

    public override string ToString()
    {
        return "Function";
    }

    public override bool Equals(LjsObject? other)
    {
        return other is LjsFunctionPointer b && FunctionIndex== b.FunctionIndex;
    }

    private bool Equals(LjsFunctionPointer other)
    {
        return FunctionIndex == other.FunctionIndex;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((LjsFunctionPointer)obj);
    }

    public override int GetHashCode()
    {
        return FunctionIndex;
    }
}