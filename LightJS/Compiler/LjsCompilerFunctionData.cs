using LightJS.Ast;
using LightJS.Errors;
using LightJS.Program;

namespace LightJS.Compiler;

internal sealed class LjsCompilerFunctionData
{
    private readonly List<LjsCompilerFunctionData> _functionsList;
    private readonly LjsCompilerFunctionData? _parentData;
    internal LjsInstructionsList Instructions { get; } = new();
    internal int FunctionIndex { get; }

    internal List<LjsFunctionArgument> Args { get; } = new();
    internal List<LjsLocalVarPointer> LocalVars => _localVars;

    private readonly Dictionary<string, int> _localVarIndices = new();
    private readonly List<LjsLocalVarPointer> _localVars = new();
    private readonly Dictionary<string, int> _namedFunctionsMap = new();

    internal Dictionary<string, int> NamedFunctionsMap => _namedFunctionsMap;

    internal LjsCompilerFunctionData(int functionIndex, List<LjsCompilerFunctionData> functionsList)
    {
        FunctionIndex = functionIndex;
        _functionsList = functionsList;
        _parentData = null;
    }

    private LjsCompilerFunctionData(int functionIndex, List<LjsCompilerFunctionData> functionsList,
        LjsCompilerFunctionData parentData)
    {
        FunctionIndex = functionIndex;
        _functionsList = functionsList;
        _parentData = parentData;
    }

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

    internal bool HasLocalInHierarchy(string name) => _localVarIndices.ContainsKey(name) ||
                                                      (_parentData != null &&
                                                       _parentData.HasLocalInHierarchy(name));

    internal (int, int) GetLocalInHierarchy(string name)
    {
        if (_localVarIndices.TryGetValue(name, out var i))
        {
            return (i, FunctionIndex);
        }

        return _parentData?.GetLocalInHierarchy(name) ?? (-1, -1);
    }

    internal LjsCompilerFunctionData CreateChild(int functionIndex) =>
        new(functionIndex, _functionsList, this);

    internal LjsCompilerFunctionData CreateNamedFunction(
        LjsAstNamedFunctionDeclaration namedFunctionDeclaration)
    {
        var funcName = namedFunctionDeclaration.Name;

        if (_namedFunctionsMap.ContainsKey(funcName))
            throw new LjsCompilerError($"duplicate function names {funcName}");

        var namedFunctionIndex = _functionsList.Count;
        var namedFunc = CreateChild(namedFunctionIndex);

        _functionsList.Add(namedFunc);
        _namedFunctionsMap[funcName] = namedFunctionIndex;
        return namedFunc;
    }

    internal bool HasFunctionWithName(string name) => _namedFunctionsMap.ContainsKey(name) ||
                                                      (_parentData != null && _parentData.HasFunctionWithName(name));

    internal int GetFunctionIndex(string name)
    {
        if (_namedFunctionsMap.ContainsKey(name))
        {
            return _namedFunctionsMap[name];
        }

        if (_parentData != null)
            return _parentData.GetFunctionIndex(name);

        throw new LjsCompilerError($"function with name {name} not found");
    }

    internal (LjsCompilerFunctionData, int) GetOrCreateNamedFunctionData(
        LjsAstNamedFunctionDeclaration namedFunctionDeclaration)
    {
        if (HasFunctionWithName(namedFunctionDeclaration.Name))
        {
            var functionIndex = GetFunctionIndex(namedFunctionDeclaration.Name);
            var functionData = _functionsList[functionIndex];
            return (functionData, functionIndex);
        }
        else
        {
            var functionIndex = _functionsList.Count;
            var functionData = CreateNamedFunction(namedFunctionDeclaration);
            return (functionData, functionIndex);
        }
    }
}