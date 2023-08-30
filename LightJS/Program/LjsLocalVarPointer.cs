namespace LightJS.Program;

public sealed class LjsLocalVarPointer
{
    public int Index { get; }
    public string Name { get; }

    public LjsLocalVarPointer(int index, string name)
    {
        Index = index;
        Name = name;
    }
}