using App16.LightJS.Errors;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsCompilerContext
{
    private readonly List<Dictionary<string, int>> _namedFunctionsMaps = new()
    {
        new Dictionary<string, int>()
    };

    private readonly List<LjsCompilerLocals> _localsStack = new();

    private readonly List<LjsCompilerFunctionContext> _functionContextsStack = new();
    private readonly List<LjsCompilerFunctionContext> _allFunctionContexts = new();

    private readonly Stack<List<int>> _jumpToTheStartPlaceholdersIndicesStack = new();
    private readonly Stack<List<int>> _jumpToTheEndPlaceholdersIndicesStack = new();

    public LjsProgramConstants Constants { get; } = new();

    public List<LjsCompilerFunctionContext> AllFunctions => _allFunctionContexts;

    public void StartFunction(LjsCompilerFunctionContext functionContext)
    {
        _functionContextsStack.Add(functionContext);
        _allFunctionContexts.Add(functionContext);
        _localsStack.Add(functionContext.Locals);
    }

    public void EndFunction()
    {
        _functionContextsStack.RemoveAt(_functionContextsStack.Count - 1);
        _localsStack.RemoveAt(_localsStack.Count - 1);
    }

    public LjsCompilerLocals CurrentLocals => _localsStack[^1];
    public LjsCompilerFunctionContext CurrentFunction => _functionContextsStack[^1];

    public bool CanJumpToStart => _jumpToTheStartPlaceholdersIndicesStack.Count != 0;
    public bool CanJumpToEnd => _jumpToTheEndPlaceholdersIndicesStack.Count != 0;

    public void AddJumpToEnd(int instructionIndex)
    {
        if (!CanJumpToEnd) throw new Exception("stack is empty");
        _jumpToTheEndPlaceholdersIndicesStack.Peek().Add(instructionIndex);
    }
    
    public void AddJumpToStart(int instructionIndex)
    {
        if (!CanJumpToStart) throw new Exception("stack is empty");
        _jumpToTheStartPlaceholdersIndicesStack.Peek().Add(instructionIndex);
    }

    public List<int> JumpToStartIndices => _jumpToTheStartPlaceholdersIndicesStack.Peek();
    public List<int> JumpToEndIndices => _jumpToTheEndPlaceholdersIndicesStack.Peek();

    public void PushPlaceholdersIndices()
    {
        _jumpToTheStartPlaceholdersIndicesStack.Push(GetTemporaryIntList());
        _jumpToTheEndPlaceholdersIndicesStack.Push(GetTemporaryIntList());
    }
    
    public void PopPlaceholdersIndices()
    {
        var list1 = _jumpToTheStartPlaceholdersIndicesStack.Pop();
        var list2 = _jumpToTheEndPlaceholdersIndicesStack.Pop();
        ReleaseTemporaryIntList(list1);
        ReleaseTemporaryIntList(list2);
    }
    
    public void StartLocalBlock()
    {
        _localsStack.Add(CurrentLocals.CreateChild());
    }

    public void EndLocalBlock()
    {
        _localsStack.RemoveAt(_localsStack.Count - 1);
    }

    public int AddLocal(string name, LjsLocalVarKind varKind)
    {
        if (varKind != LjsLocalVarKind.Var)
        {
            return CurrentLocals.Add(name, varKind);
        }
        
        var i = _localsStack.Count - 1;
        var loc = _localsStack[i];

        while (loc.IsChild && i > 0)
        {
            i--;
            loc = _localsStack[i];
        }

        return loc.Add(name, varKind);

    }

    public bool HasLocal(string name, bool parentSearch = true)
    {
        var i = _localsStack.Count - 1;
        var loc = _localsStack[i];
        
        if (loc.Has(name)) return true;
        
        if (!parentSearch || !loc.IsChild) return false;

        while (loc.IsChild && i > 0)
        {
            i--;
            loc = _localsStack[i];

            if (loc.Has(name)) return true;
        }

        return false;
    } 
    
    public LjsLocalVarPointer GetLocal(string name)
    {
        var i = _localsStack.Count - 1;
        var loc = _localsStack[i];

        if (loc.Has(name))
        {
            return loc.GetPointer(name);
        }
        
        while (loc.IsChild && i > 0)
        {
            i--;
            loc = _localsStack[i];

            if (loc.Has(name)) return loc.GetPointer(name);
        }

        throw new Exception($"var {name} not found");
    }
        

    public bool HasLocalInHierarchy(string name)
    {
        for (var i = _localsStack.Count - 1; i >= 0; --i)
        {
            var locals = _localsStack[i];
            if (locals.Has(name)) return true;
        }

        return false;
    }
    
    public (LjsLocalVarPointer, int) GetLocalInHierarchy(string name)
    {
        for (var i = _localsStack.Count - 1; i >= 0; --i)
        {
            var locals = _localsStack[i];
            if (locals.Has(name))
            {
                var pointer = locals.GetPointer(name);
                return (pointer, locals.FunctionIndex);
            }
        }

        throw new Exception($"local with name:{name} not found in hierarchy");
    }

    public LjsCompilerFunctionContext AddAnonymousFunctionDeclaration()
    {
        var functionIndex = _allFunctionContexts.Count;
        
        var funcData = new LjsCompilerFunctionData(functionIndex);
        
        var funcContext = new LjsCompilerFunctionContext(funcData);
        
        _allFunctionContexts.Add(funcContext);

        return funcContext;
    }

    public LjsCompilerFunctionContext AddNamedFunctionDeclaration(string name)
    {
        var functionIndex = _allFunctionContexts.Count;
        
        var funcData = new LjsCompilerFunctionData(functionIndex);
        
        var funcContext = new LjsCompilerFunctionContext(funcData);
        
        _allFunctionContexts.Add(funcContext);

        _namedFunctionsMaps[^1][name] = functionIndex;

        return funcContext;
    }

    public LjsCompilerFunctionContext GetFunctionContext(int functionIndex) => 
        _allFunctionContexts[functionIndex];

    public void PushNamedFunctionsMap()
    {
        _namedFunctionsMaps.Add(new Dictionary<string, int>());
    }

    public void PopNamedFunctionsMap()
    {
        if (_namedFunctionsMaps.Count == 1)
            throw new Exception();
        
        _namedFunctionsMaps.RemoveAt(_namedFunctionsMaps.Count - 1);
    }

    public bool HasFunctionWithName(string name)
    {
        if (_namedFunctionsMaps.Count == 0) return false;

        for (var i = _namedFunctionsMaps.Count - 1; i >= 0; --i)
        {
            var map = _namedFunctionsMaps[i];
            if (map.ContainsKey(name)) return true;
        }

        return false;
    }

    public int GetFunctionIndex(string name)
    {
        for (var i = _namedFunctionsMaps.Count - 1; i >= 0; --i)
        {
            var map = _namedFunctionsMaps[i];
            if (map.TryGetValue(name, out var index)) return index;
        }

        throw new LjsCompilerError($"function with name {name} not found");
    }

    public Dictionary<string, int> TopLevelNamedFunctions => _namedFunctionsMaps[0];
    
    
    private readonly List<List<int>> _intListsPool = new();

    private List<int> GetTemporaryIntList()
    {
        if (_intListsPool.Count > 0)
        {
            var list = _intListsPool[^1];
            _intListsPool.RemoveAt(_intListsPool.Count - 1);
            list.Clear();
            return list;
        }

        return new List<int>(8);
    }

    private void ReleaseTemporaryIntList(List<int> list)
    {
        list.Clear();
        _intListsPool.Add(list);
    }
}