using LightJS.Errors;
using LightJS.Program;

namespace LightJS.Compiler;

internal sealed class LjsCompilerLocals
{

    private readonly LjsCompilerLocals? _parent;
    
    private readonly Dictionary<string, int> _indices = new();
    private readonly List<LjsLocalVarPointer> _pointers;
    
    internal List<LjsLocalVarPointer> Pointers => _pointers;

    internal LjsCompilerLocals()
    {
        _parent = null;
        _pointers = new List<LjsLocalVarPointer>();
    }

    private LjsCompilerLocals(LjsCompilerLocals parent)
    {
        _parent = parent;
        _pointers = parent.Pointers;
        _indices = new Dictionary<string, int>(parent._indices);
    }
    
    internal int Add(string name, LjsLocalVarKind varKind)
    {
        if (_parent != null && (varKind == LjsLocalVarKind.Var))
        {
            return _parent.Add(name, varKind);
        }
        
        if (_indices.ContainsKey(name))
            throw new LjsCompilerError($"duplicate var name {name}");

        var index = _pointers.Count;
        _pointers.Add(new LjsLocalVarPointer(index, name, varKind));
        _indices[name] = index;
        return index;
    }
    
    internal bool Has(string name, bool parentSearch = true) => 
        _indices.ContainsKey(name) || (parentSearch && _parent != null && _parent.Has(name));

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

    public LjsCompilerLocals CreateChild() => new(this);
}