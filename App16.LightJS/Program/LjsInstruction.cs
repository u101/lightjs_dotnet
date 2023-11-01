namespace App16.LightJS.Program;

public readonly struct LjsInstruction
{
    public LjsInstructionCode Code { get; }
    public int Argument { get; }

    public LjsInstruction(LjsInstructionCode code)
    {
        Code = code;
        Argument = 0;
    }
    
    public LjsInstruction(LjsInstructionCode code, int argument)
    {
        Code = code;
        Argument = argument;
    }
    
}