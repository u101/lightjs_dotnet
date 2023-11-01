namespace App16.LightJS.Runtime;

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
        return other is LjsFunctionPointer b && FunctionIndex == b.FunctionIndex;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FunctionIndex);
    }
}