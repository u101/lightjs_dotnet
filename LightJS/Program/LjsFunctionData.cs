namespace LightJS.Program;

public sealed class LjsFunctionData 
{
    public LjsInstruction[] Instructions { get; }
    public LjsFunctionArgument[] Arguments { get; }
    public LjsLocalVarPointer[] Locals { get; }

    public int LocalsCount => Locals.Length;

    public LjsFunctionData(
        LjsInstruction[] instructions, 
        LjsFunctionArgument[] arguments, 
        LjsLocalVarPointer[] locals)
    {
        Instructions = instructions;
        Arguments = arguments;
        Locals = locals;
    }
}