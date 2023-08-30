namespace LightJS.Program;

public sealed class LjsFunctionData 
{
    public int FunctionIndex { get; }
    public LjsInstruction[] Instructions { get; }
    public LjsFunctionArgument[] Arguments { get; }
    public LjsLocalVarPointer[] Locals { get; }

    public int LocalsCount => Locals.Length;

    public LjsFunctionData(
        int functionIndex,
        LjsInstruction[] instructions, 
        LjsFunctionArgument[] arguments, 
        LjsLocalVarPointer[] locals)
    {
        FunctionIndex = functionIndex;
        Instructions = instructions;
        Arguments = arguments;
        Locals = locals;
    }
}