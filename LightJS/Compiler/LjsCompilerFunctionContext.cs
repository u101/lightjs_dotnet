using LightJS.Program;

namespace LightJS.Compiler;

internal sealed class LjsCompilerFunctionContext
{
    internal int FunctionIndex { get; }
    
    internal List<LjsFunctionArgument> FunctionArgs { get; } = new();
    
    internal LjsInstructionsList Instructions { get; } = new();
    
    internal LjsCompilerFunctionContext(int functionIndex)
    {
        FunctionIndex = functionIndex;
    }
}