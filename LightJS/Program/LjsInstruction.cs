namespace LightJS.Program;

public readonly struct LjsInstruction
{
    public byte Code { get; }
    public short Index { get; }

    public LjsInstruction(byte code)
    {
        Code = code;
        Index = 0;
    }
    
    public LjsInstruction(byte code, short index)
    {
        Code = code;
        Index = index;
    }
    
}