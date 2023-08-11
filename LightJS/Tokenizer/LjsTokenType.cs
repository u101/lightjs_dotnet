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
    
    StringLiteral,
    
    Null,
    Undefined,
    
    // binary operators ->
    
    OpGreater,
    OpLess,
    OpPlus,
    OpMinus,
    OpMultiply,
    OpDiv,
    OpBitAnd,
    OpBitOr,
    
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
    
    OpNegate,
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
    
    OpIncrement,
    OpDecrement,
    
    This,
    
    Var,
    Const,
    Function,
    
    Return,
    Break,
    Continue,
    
    If,
    Else,
    While,
    Do,
    For,
    
    Identifier
}