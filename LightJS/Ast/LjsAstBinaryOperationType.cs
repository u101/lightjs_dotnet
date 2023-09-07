namespace LightJS.Ast;

public enum LjsAstBinaryOperationType
{
    Plus,
    Minus,
    Multiply,
    Div,
    Modulo,
    Exponent,
    
    BitAnd,
    BitOr,
    BitLeftShift,
    BitRightShift,
    BitUnsignedRightShift,
    
    Greater,
    GreaterOrEqual,
    Less,
    LessOrEqual,
    
    Equals,
    EqualsStrict,
    NotEqual,
    NotEqualStrict,
    
    LogicalAnd,
    LogicalOr,
}