namespace LightJS.Program;

public static class LjsInstructions
{

    public const byte VarDef = (byte) 0x01; 
    // load local var, followed by var index byte
    public const byte VarLoad = (byte) 0x02; 
    // load local var, followed by var index byte
    public const byte VarStore = (byte) 0x03; 
    
    // load from stack value b, value a, push a + b
    public const byte Add = (byte) 0x10;
    // load from stack value b, value a, push a - b
    public const byte Sub = (byte) 0x11;
    // load from stack value b, value a, push a * b
    public const byte Mul = (byte) 0x12;
    // load from stack value b, value a, push a / b
    public const byte Div = (byte) 0x13;
    // load from stack value b, value a, push a / b
    public const byte Mod = (byte) 0x14;
    
    

}