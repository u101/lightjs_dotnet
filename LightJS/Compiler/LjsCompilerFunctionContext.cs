using LightJS.Program;

namespace LightJS.Compiler;

internal sealed class LjsCompilerFunctionContext
{
    private readonly List<LjsCompilerFunctionContext> _functionsContextList;
    private readonly LjsCompilerFunctionContext? _parentFunction;
    
    internal int FunctionIndex { get; }
    
    internal List<LjsFunctionArgument> FunctionArgs { get; } = new();
    
    private readonly Dictionary<string, int> _namedFunctionsMap = new();

    internal Dictionary<string, int> NamedFunctionsMap => _namedFunctionsMap;
    
    internal LjsInstructionsList Instructions { get; } = new();
    
    internal LjsCompilerFunctionContext(int functionIndex, List<LjsCompilerFunctionContext> functionsList)
    {
        FunctionIndex = functionIndex;
        _functionsContextList = functionsList;
        _parentFunction = null;
    }
}