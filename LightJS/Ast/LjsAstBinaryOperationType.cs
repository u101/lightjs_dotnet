namespace LightJS.Ast;

public enum LjsAstBinaryOperationType
{
    Greater,
    Less,
    Plus,
    Minus,
    Multiply,
    Div,
    Modulo,
    
    BitAnd,
    BitOr,
    
    BitLeftShift,
    BitRightShift,
    BitUnsignedRightShift,
    
    Equals,
    EqualsStrict,
    GreaterOrEqual,
    LessOrEqual,
    NotEqual,
    NotEqualStrict,
    LogicalAnd,
    LogicalOr,
}