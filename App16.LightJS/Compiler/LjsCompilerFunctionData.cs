using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsCompilerFunctionData
{
    internal int FunctionIndex { get; }
    
    internal List<LjsFunctionArgument> FunctionArgs { get; } = new();
    
    internal LjsInstructionsList Instructions { get; } = new();
    
    internal LjsCompilerFunctionData(int functionIndex)
    {
        FunctionIndex = functionIndex;
    }
}