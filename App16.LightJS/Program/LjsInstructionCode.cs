namespace App16.LightJS.Program;

public enum LjsInstructionCode
{
    Halt = 0x00,
    
    // remove last value on stack
    Discard = 0x1,
    // copy last value on stack and push to stack
    Copy = 0x2,
    
    // load local var and push to the stack, argument = local index
    VarLoad = 0x03, 
    // store last value from the stack in local var, argument = local index
    VarStore = 0x04, 
    
    // load int const, followed by const index byte
    ConstInt = 0x05,
    // push specified value on the stack, no arguments 
    ConstIntZero = 0x06,
    ConstIntOne = 0x07,
    ConstIntMinusOne = 0x08,
    
    // load double const, followed by const index byte
    ConstDouble = 0x09,
    // push specified value on the stack, no arguments 
    ConstDoubleZero = 0x0a,
    ConstDoubleNaN = 0x0b,
    
    
    // load double const, followed by const index byte
    ConstString = 0x0d,
    // push specified value on the stack, no arguments 
    ConstStringEmpty = 0x0e,
    
    // push const to stack
    ConstTrue = 0x0f, 
    // push const to stack
    ConstFalse = 0x10, 
    // push null to stack
    ConstNull = 0x11,
    // push undef to stack
    ConstUndef = 0x12,
    
    

    // load from stack value b, value a, push a + b
    Add = 0x15,
    // load from stack value b, value a, push a - b
    Sub = 0x16,
    // load from stack value b, value a, push a * b
    Mul = 0x17,
    // load from stack value b, value a, push a / b
    Div = 0x18,
    // load from stack value b, value a, push a / b
    Mod = 0x19,
    // load from stack value b, value a, push a & b
    BitAnd = 0x1a,
    // load from stack value b, value a, push a | b
    BitOr = 0x1b,
    // load from stack value b, value a, push a ^ b
    BitXor = 0x1c,
    // load from stack value b, value a, push a << b
    BitShiftLeft = 0x1d,
    // load from stack value b, value a, push a >> b
    BitSShiftRight = 0x1e,
    // load from stack value b, value a, push a >>> b
    BitUShiftRight = 0x1f,
    
    // load from stack value b, value a, push a > b ? 1:0
    Gt = 0x20,
    // load from stack value b, value a, push a >= b ? 1:0
    Gte = 0x21,
    // load from stack value b, value a, push a < b ? 1:0
    Lt = 0x22,
    // load from stack value b, value a, push a <= b ? 1:0
    Lte = 0x23,
    // load from stack value b, value a, push a == b ? 1:0
    Eq = 0x24,
    // load from stack value b, value a, push a === b ? 1:0
    Eqs = 0x25,
    // load from stack value b, value a, push a != b ? 1:0
    Neq = 0x26,
    // load from stack value b, value a, push a !== b ? 1:0
    Neqs = 0x27,
    // load from stack value b, value a, push a && b ? 1:0
    And = 0x28,
    // load from stack value b, value a, push a || b ? 1:0
    Or = 0x29,
    
    // load from stack value a, push !a
    Not = 0x2a,
    // load from stack value a, push ~a
    BitNot = 0x2b,
    // load from stack value a, push a + 1
    Incr = 0x2c,
    // load from stack value a, push a - 1
    Decr = 0x2d,
    // load from stack value a, push -a
    Minus = 0x2e,
    Pow = 0x2f,

    Jump = 0x30,
    JumpIfTrue = 0x31,
    JumpIfFalse = 0x32,

    Return = 0x40,
    FuncCall = 0x41,
    // load function reference to stack, argument = function index
    FuncRef = 0x42,
    
    // load local var from parent function context and push to the stack,
    // argument = 2 bytes local var index, 2 bytes function index
    ParentVarLoad = 0x46, 
    // store last value from the stack in parent function context local var,
    // argument = 2 bytes local var index, 2 bytes function index
    ParentVarStore = 0x47, 
    
    // load var and push to the stack, argument = string const index
    ExtLoad = 0x50,
    // store last value from the stack in var, argument = string const index
    ExtStore = 0x51,
    
    NewArray = 0x55,
    NewDictionary = 0x56,
    
    GetProp = 0x60,
    SetProp = 0x61,

    GetThis = 0x62
}