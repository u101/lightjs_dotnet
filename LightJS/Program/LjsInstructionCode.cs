namespace LightJS.Program;

public enum LjsInstructionCode
{
    Halt = 0x00,
    
    // define local var, argument = string const index
    VarDef = 0x01, 
    // similar to VarStore except value is not pushed to the stack
    VarInit = 0x02, 
    // load local var and push to the stack, argument = string const index
    VarLoad = 0x03, 
    // store last value from the stack in var, argument = string const index
    VarStore = 0x04, 
    // load int const, followed by const index byte
    ConstInt = 0x05,
    // load double const, followed by const index byte
    ConstDouble = 0x06,
    // load double const, followed by const index byte
    ConstString = 0x07,
    // push const to stack
    ConstTrue = 0x08, 
    // push const to stack
    ConstFalse = 0x09, 
    // push null to stack
    ConstNull = 0x0a,
    // push undef to stack
    ConstUndef = 0x0b,
    
    JumpIfFalse = 0x0e,
    Jump = 0x0f,

    // load from stack value b, value a, push a + b
    Add = 0x10,
    // load from stack value b, value a, push a - b
    Sub = 0x11,
    // load from stack value b, value a, push a * b
    Mul = 0x12,
    // load from stack value b, value a, push a / b
    Div = 0x13,
    // load from stack value b, value a, push a / b
    Mod = 0x14,
    // load from stack value b, value a, push a & b
    BitAnd = 0x15,
    // load from stack value b, value a, push a | b
    BitOr = 0x16,
    // load from stack value b, value a, push a ^ b
    BitXor = 0x17,
    // load from stack value b, value a, push a << b
    BitShiftLeft = 0x18,
    // load from stack value b, value a, push a >> b
    BitSShiftRight = 0x19,
    // load from stack value b, value a, push a >>> b
    BitUShiftRight = 0x1a,
    
    // load from stack value b, value a, push a > b ? 1:0
    Gt = 0x1b,
    // load from stack value b, value a, push a >= b ? 1:0
    Gte = 0x1c,
    // load from stack value b, value a, push a < b ? 1:0
    Lt = 0x1d,
    // load from stack value b, value a, push a <= b ? 1:0
    Lte = 0x1e,
    // load from stack value b, value a, push a == b ? 1:0
    Eq = 0x1f,
    // load from stack value b, value a, push a === b ? 1:0
    Eqs = 0x20,
    // load from stack value b, value a, push a != b ? 1:0
    Neq = 0x21,
    // load from stack value b, value a, push a !== b ? 1:0
    Neqs = 0x22,
    // load from stack value b, value a, push a && b ? 1:0
    And = 0x23,
    // load from stack value b, value a, push a || b ? 1:0
    Or = 0x24,
    
    // load from stack value a, push !a
    Not = 0x25,
    // load from stack value a, push !a
    BitNot = 0x26,
    // load from stack value a, push a + 1
    Incr = 0x27,
    // load from stack value a, push a + 1
    Decr = 0x28,
    // load from stack value a, push -a
    Minus = 0x29,

    Return = 0x40,
    FuncCall = 0x41,

}