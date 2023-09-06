using LightJS.Errors;
using LightJS.Program;

namespace LightJS.Compiler;

internal sealed class LjsCompilerLocals
{
    
    private readonly Dictionary<string, int> _indices = new();
    private readonly List<LjsLocalVarPointer> _pointers = new();
    
    internal List<LjsLocalVarPointer> Pointers => _pointers;
    
    internal int Add(string name, LjsLocalVarKind varKind)
    {
        if (_indices.ContainsKey(name))
            throw new LjsCompilerError($"duplicate var name {name}");

        var index = _pointers.Count;
        _pointers.Add(new LjsLocalVarPointer(index, name, varKind));
        _indices[name] = index;
        return index;
    }

    internal bool Has(string name) => _indices.ContainsKey(name);

    internal int GetIndex(string name) => _indices.TryGetValue(name, out var i) ? i : -1;

    internal LjsLocalVarPointer GetPointer(int localIndex) => _pointers[localIndex];
}