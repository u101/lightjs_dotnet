namespace LightJS.Program;

public sealed class LjsLocalVarPointer
{
    public int Index { get; }
    public string Name { get; }
    public LjsLocalVarKind VarKind { get; }

    public LjsLocalVarPointer(int index, string name, LjsLocalVarKind varKind)
    {
        Index = index;
        Name = name;
        VarKind = varKind;
    }
}