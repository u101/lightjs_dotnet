using LightJS.Program;

namespace LightJS.Compiler;

internal sealed class LjsCompilerFunctionContext
{
    internal List<LjsFunctionArgument> FunctionArgs { get; } = new();
    
    private readonly Dictionary<string, int> _namedFunctionsMap = new();

    internal Dictionary<string, int> NamedFunctionsMap => _namedFunctionsMap;
    
    // internal LjsCompilerFunctionContext(int functionIndex, List<LjsCompilerContext> functionsList)
    // {
    //     FunctionIndex = functionIndex;
    //     _functionsContextList = functionsList;
    //     _parentContext = null;
    // }
}