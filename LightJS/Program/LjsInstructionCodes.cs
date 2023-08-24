namespace LightJS.Program;

public static class LjsInstructionCodes
{

    // define local var
    public const byte VarDef = (byte) 0x01; 
    // load local var, followed by var index byte
    public const byte VarLoad = (byte) 0x02; 
    // load local var, followed by var index byte
    public const byte VarStore = (byte) 0x03; 
    // load int const, followed by const index byte
    public const byte ConstInt = (byte) 0x04;
    // load double const, followed by const index byte
    public const byte ConstDouble = (byte) 0x05;
    // load double const, followed by const index byte
    public const byte ConstString = (byte) 0x06;
    // push const to stack
    public const byte ConstTrue = (byte) 0x07; 
    // push const to stack
    public const byte ConstFalse = (byte) 0x08; 
    // push null to stack
    public const byte ConstNull = (byte) 0x09;
    // push undef to stack
    public const byte ConstUndef = (byte) 0x0a; 
    
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
    // load from stack value b, value a, push a & b
    public const byte BitAnd = (byte) 0x15;
    // load from stack value b, value a, push a | b
    public const byte BitOr = (byte) 0x16;
    // load from stack value b, value a, push a ^ b
    public const byte BitXor = (byte) 0x17;
    // load from stack value b, value a, push a << b
    public const byte BitShiftLeft = (byte) 0x18;
    // load from stack value b, value a, push a >> b
    public const byte BitSShiftRight = (byte) 0x19;
    // load from stack value b, value a, push a >>> b
    public const byte BitUShiftRight = (byte) 0x1a;
    
    // load from stack value b, value a, push a > b ? 1:0
    public const byte Gt = (byte) 0x1b;
    // load from stack value b, value a, push a >= b ? 1:0
    public const byte Gte = (byte) 0x1c;
    // load from stack value b, value a, push a < b ? 1:0
    public const byte Lt = (byte) 0x1d;
    // load from stack value b, value a, push a <= b ? 1:0
    public const byte Lte = (byte) 0x1e;
    // load from stack value b, value a, push a == b ? 1:0
    public const byte Eq = (byte) 0x1f;
    // load from stack value b, value a, push a === b ? 1:0
    public const byte Eqs = (byte) 0x20;
    // load from stack value b, value a, push a != b ? 1:0
    public const byte Neq = (byte) 0x21;
    // load from stack value b, value a, push a !== b ? 1:0
    public const byte Neqs = (byte) 0x22;
    // load from stack value b, value a, push a && b ? 1:0
    public const byte And = (byte) 0x23;
    // load from stack value b, value a, push a || b ? 1:0
    public const byte Or = (byte) 0x24;
    
    // load from stack value a, push !a
    public const byte Not = (byte) 0x25;
    // load from stack value a, push !a
    public const byte BitNot = (byte) 0x26;
    // load from stack value a, push a + 1
    public const byte Incr = (byte) 0x27;
    // load from stack value a, push a + 1
    public const byte Decr = (byte) 0x28;
    // load from stack value a, push -a
    public const byte Negate = (byte) 0x29;

}