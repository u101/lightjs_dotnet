namespace LightJS.Program;

public readonly struct LjsInstruction
{
    public LjsInstructionCode Code { get; }
    public short Index { get; }

    public LjsInstruction(LjsInstructionCode code)
    {
        Code = code;
        Index = 0;
    }
    
    public LjsInstruction(LjsInstructionCode code, short index)
    {
        Code = code;
        Index = index;
    }
    
}