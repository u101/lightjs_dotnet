namespace App16.LightJS.Compiler;

public sealed class LjsCompilerFunctionContext
{
    public LjsCompilerFunctionData FunctionData { get; }
    public LjsCompilerLocals Locals { get; }

    public LjsCompilerFunctionContext(LjsCompilerFunctionData functionData)
    {
        FunctionData = functionData;
        Locals = new LjsCompilerLocals(functionData.FunctionIndex);
    }
}