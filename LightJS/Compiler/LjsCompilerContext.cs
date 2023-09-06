using LightJS.Ast;
using LightJS.Errors;
using LightJS.Program;

namespace LightJS.Compiler;

internal sealed class LjsCompilerContext
{
   
    private readonly List<LjsCompilerContext> _functionsContextList;
    private readonly LjsCompilerContext? _parentContext;

    public LjsCompilerFunctionContext FunctionContext { get; }
    
    internal LjsCompilerLocals Locals { get; }
    
    private readonly Dictionary<string, int> _namedFunctionsMap;

    internal Dictionary<string, int> NamedFunctionsMap => _namedFunctionsMap;

    internal LjsCompilerContext(
        LjsCompilerFunctionContext functionContext, 
        List<LjsCompilerContext> functionsContextList)
    {
        FunctionContext = functionContext;
        
        _functionsContextList = functionsContextList;
        _parentContext = null;
        
        Locals = new LjsCompilerLocals();
        _namedFunctionsMap = new Dictionary<string, int>();
    }

    private LjsCompilerContext(
        LjsCompilerFunctionContext functionContext,
        List<LjsCompilerContext> functionsContextList,
        LjsCompilerContext parentContext)
    {
        FunctionContext = functionContext;
        
        _functionsContextList = functionsContextList;
        _parentContext = parentContext;
        
        Locals = new LjsCompilerLocals();
        _namedFunctionsMap = new Dictionary<string, int>();
    }
    
    private LjsCompilerContext(
        LjsCompilerFunctionContext functionContext,
        List<LjsCompilerContext> functionsContextList,
        LjsCompilerContext parentContext,
        LjsCompilerLocals locals,
        Dictionary<string, int> namedFunctionsMap)
    {
        FunctionContext = functionContext;
        
        _functionsContextList = functionsContextList;
        _parentContext = parentContext;
        
        Locals = locals;
        _namedFunctionsMap = namedFunctionsMap;
    }

    internal bool HasLocalInHierarchy(string name) => Locals.Has(name) ||
                                                      (_parentContext != null &&
                                                       _parentContext.HasLocalInHierarchy(name));

    internal (LjsLocalVarPointer, int) GetLocalInHierarchy(string name)
    {
        if (Locals.Has(name))
        {
            var pointer = Locals.GetPointer(name);
            return (pointer, FunctionContext.FunctionIndex);
        }

        return _parentContext?.GetLocalInHierarchy(name) ?? 
               throw new Exception($"local with name:{name} not found in hierarchy");
    }

    private LjsCompilerContext CreateChildFunctionContext(int functionIndex) =>
        new(new LjsCompilerFunctionContext(functionIndex), _functionsContextList, this);

    internal LjsCompilerContext CreateNamedFunctionContext(
        LjsAstNamedFunctionDeclaration namedFunctionDeclaration)
    {
        var funcName = namedFunctionDeclaration.Name;

        if (_namedFunctionsMap.ContainsKey(funcName))
            throw new LjsCompilerError($"duplicate function names {funcName}");

        var namedFunctionIndex = _functionsContextList.Count;
        var namedFunc = CreateChildFunctionContext(namedFunctionIndex);

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
    
    internal LjsCompilerContext CreateAnonFunctionContext()
    {
        var functionIndex = _functionsContextList.Count;
        var func = CreateChildFunctionContext(functionIndex);

        _functionsContextList.Add(func);
        return func;
    }

    internal LjsCompilerContext CreateLocalContext() => new(
        FunctionContext, _functionsContextList,
        this, Locals.CreateChild(), _namedFunctionsMap);
}