using LightJS.Errors;
using LightJS.Program;

namespace LightJS.Compiler;

internal sealed class LjsCompilerLocals
{
    
    private readonly Dictionary<string, int> _localVarIndices = new();
    private readonly List<LjsLocalVarPointer> _localVars = new();
    
    internal List<LjsLocalVarPointer> LocalVars => _localVars;
    
    internal int AddLocal(string name, LjsLocalVarKind varKind)
    {
        if (_localVarIndices.ContainsKey(name))
            throw new LjsCompilerError($"duplicate var name {name}");

        var index = _localVars.Count;
        _localVars.Add(new LjsLocalVarPointer(index, name, varKind));
        _localVarIndices[name] = index;
        return index;
    }

    internal bool HasLocal(string name) => _localVarIndices.ContainsKey(name);

    internal int GetLocal(string name) => _localVarIndices.TryGetValue(name, out var i) ? i : -1;
}