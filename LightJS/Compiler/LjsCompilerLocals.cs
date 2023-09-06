using LightJS.Errors;
using LightJS.Program;

namespace LightJS.Compiler;

internal sealed class LjsCompilerLocals
{

    private readonly LjsCompilerLocals? _parent;
    
    private readonly Dictionary<string, int> _indices = new();
    private readonly List<LjsLocalVarPointer> _pointers = new();
    
    internal List<LjsLocalVarPointer> Pointers => _pointers;

    internal LjsCompilerLocals()
    {
        _parent = null;
    }

    private LjsCompilerLocals(LjsCompilerLocals parent)
    {
        _parent = parent;
    }
    
    internal int Add(string name, LjsLocalVarKind varKind)
    {
        if (_indices.ContainsKey(name))
            throw new LjsCompilerError($"duplicate var name {name}");

        var index = _pointers.Count;
        _pointers.Add(new LjsLocalVarPointer(index, name, varKind));
        _indices[name] = index;
        return index;
    }

    internal bool Has(string name) => _indices.ContainsKey(name) || (_parent != null && _parent.Has(name));

    internal LjsLocalVarPointer GetPointer(string name)
    {
        if (_indices.TryGetValue(name, out var localIndex))
        {
            return _pointers[localIndex];
        }
        
        if (_parent != null)
        {
            return _parent.GetPointer(name);
        }

        throw new Exception($"var {name} not found");
    }
}