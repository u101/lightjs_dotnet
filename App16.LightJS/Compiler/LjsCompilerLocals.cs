using App16.LightJS.Errors;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsCompilerLocals
{
    public int FunctionIndex { get; }
    public List<LjsLocalVarPointer> Pointers { get; }
    
    private readonly Dictionary<string, int> _indices = new();
    
    public bool IsChild { get; }

    public LjsCompilerLocals(int functionIndex)
    {
        FunctionIndex = functionIndex;
        IsChild = false;
        Pointers = new List<LjsLocalVarPointer>();
    }

    private LjsCompilerLocals(LjsCompilerLocals parent)
    {
        FunctionIndex = parent.FunctionIndex;
        Pointers = parent.Pointers;
        IsChild = true;
        _indices = new Dictionary<string, int>(parent._indices);
    }
    
    public int Add(string name, LjsLocalVarKind varKind)
    {
        if (_indices.ContainsKey(name))
            throw new LjsCompilerError($"duplicate var name {name}");

        var index = Pointers.Count;
        Pointers.Add(new LjsLocalVarPointer(index, name, varKind));
        _indices[name] = index;
        return index;
    }
    
    public bool Has(string name) => _indices.ContainsKey(name);

    public LjsLocalVarPointer GetPointer(string name)
    {
        if (_indices.TryGetValue(name, out var localIndex))
        {
            return Pointers[localIndex];
        }

        throw new Exception($"var {name} not found");
    }

    public LjsCompilerLocals CreateChild() => new(this);
    
}