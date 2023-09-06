using LightJS.Ast;
using LightJS.Errors;
using LightJS.Program;

namespace LightJS.Compiler;

internal sealed class LjsCompilerContext
{
    private readonly List<LjsCompilerContext> _functionsContextList;
    private readonly LjsCompilerContext? _parentContext;
    internal LjsInstructionsList Instructions { get; } = new();
    internal int FunctionIndex { get; }

    internal List<LjsFunctionArgument> FunctionArgs { get; } = new();

    internal LjsCompilerLocals Locals { get; } = new();
    
    private readonly Dictionary<string, int> _namedFunctionsMap = new();

    internal Dictionary<string, int> NamedFunctionsMap => _namedFunctionsMap;

    internal LjsCompilerContext(int functionIndex, List<LjsCompilerContext> functionsList)
    {
        FunctionIndex = functionIndex;
        _functionsContextList = functionsList;
        _parentContext = null;
    }

    private LjsCompilerContext(int functionIndex, List<LjsCompilerContext> functionsList,
        LjsCompilerContext parentData)
    {
        FunctionIndex = functionIndex;
        _functionsContextList = functionsList;
        _parentContext = parentData;
    }

    internal bool HasLocalInHierarchy(string name) => Locals.Has(name) ||
                                                      (_parentContext != null &&
                                                       _parentContext.HasLocalInHierarchy(name));

    internal (LjsLocalVarPointer, int) GetLocalInHierarchy(string name)
    {
        if (Locals.Has(name))
        {
            var localIndex = Locals.GetIndex(name);
            var pointer = Locals.GetPointer(localIndex);
            return (pointer, FunctionIndex);
        }

        return _parentContext?.GetLocalInHierarchy(name) ?? 
               throw new Exception($"local with name:{name} not found in hierarchy");
    }

    internal LjsCompilerContext CreateChild(int functionIndex) =>
        new(functionIndex, _functionsContextList, this);

    internal LjsCompilerContext CreateNamedFunctionContext(
        LjsAstNamedFunctionDeclaration namedFunctionDeclaration)
    {
        var funcName = namedFunctionDeclaration.Name;

        if (_namedFunctionsMap.ContainsKey(funcName))
            throw new LjsCompilerError($"duplicate function names {funcName}");

        var namedFunctionIndex = _functionsContextList.Count;
        var namedFunc = CreateChild(namedFunctionIndex);

        _functionsContextList.Add(namedFunc);
        _namedFunctionsMap[funcName] = namedFunctionIndex;
        return namedFunc;
    }

    internal bool HasFunctionWithName(string name) => _namedFunctionsMap.ContainsKey(name) ||
                                                      (_parentContext != null && _parentContext.HasFunctionWithName(name));

    internal int GetFunctionIndex(string name)
    {
        if (_namedFunctionsMap.ContainsKey(name))
        {
            return _namedFunctionsMap[name];
        }

        if (_parentContext != null)
            return _parentContext.GetFunctionIndex(name);

        throw new LjsCompilerError($"function with name {name} not found");
    }

    internal (LjsCompilerContext, int) GetOrCreateNamedFunctionContext(
        LjsAstNamedFunctionDeclaration namedFunctionDeclaration)
    {
        if (HasFunctionWithName(namedFunctionDeclaration.Name))
        {
            var functionIndex = GetFunctionIndex(namedFunctionDeclaration.Name);
            var functionContext = _functionsContextList[functionIndex];
            return (functionContext, functionIndex);
        }
        else
        {
            var functionIndex = _functionsContextList.Count;
            var functionContext = CreateNamedFunctionContext(namedFunctionDeclaration);
            return (functionContext, functionIndex);
        }
    }
}