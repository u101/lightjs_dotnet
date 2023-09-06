namespace LightJS.Tokenizer;

public enum LjsTokenType
{
    None,
    
    True,
    False,
    
    IntDecimal,
    IntHex,
    IntBinary,
    
    Float,
    FloatE,
    FloatNaN,
    
    StringLiteral,
    
    Null,
    Undefined,
    
    // binary operators ->
    // todo Exponentiation ** operator returns the result of raising the first operand to the power of the second operand
    OpGreater,
    OpLess,
    OpPlus,
    OpMinus,
    OpMultiply,
    OpDiv,
    OpModulo,
    OpBitAnd,
    OpBitOr,
    OpBitLeftShift,
    OpBitRightShift,
    OpBitUnsignedRightShift,
    
    OpEquals,
    OpEqualsStrict,
    OpGreaterOrEqual,
    OpLessOrEqual,
    OpNotEqual,
    OpNotEqualStrict,
    OpLogicalAnd,
    OpLogicalOr,
    
    OpAssign,
    OpPlusAssign,
    OpMinusAssign,
    OpMultAssign,
    OpDivAssign,
    OpBitOrAssign,
    OpBitAndAssign,
    OpLogicalOrAssign,
    OpLogicalAndAssign,
    
    // end of binary operators;
    
    // unary operators ->
    
    OpLogicalNot,
    OpBitNot,
    OpIncrement,
    OpDecrement,
    // end of unary operators;
    
    OpQuestionMark,
    
    OpComma,
    OpDot,
    OpColon,
    OpSemicolon,
    
    OpBracketOpen,
    OpBracketClose,
    OpParenthesesOpen,
    OpParenthesesClose,
    OpSquareBracketsOpen,
    OpSquareBracketsClose,
    
    This,
    
    Var,
    Const,
    Let,
    Function,
    
    Return,
    Break,
    Continue,
    
    If,
    ElseIf,
    Else,
    
    Switch,
    Case,
    Default,
    
    While,
    Do,
    
    For,
    In,
    Identifier
}