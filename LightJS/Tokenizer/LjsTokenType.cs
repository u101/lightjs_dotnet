namespace LightJS.Tokenizer;

public enum LjsTokenType
{
    None,
    
    IntDecimal,
    IntHex,
    IntBinary,
    
    Float,
    FloatE,
    
    StringLiteral,
    
    OpGreater,
    OpLess,
    OpAssign,
    OpPlus,
    OpMinus,
    OpMultiply,
    OpDiv,
    OpBitAnd,
    OpBitOr,
    
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
    OpPlusAssign,
    OpMinusAssign,
    OpEquals,
    OpEqualsStrict,
    OpGreaterOrEqual,
    OpLessOrEqual,
    OpNotEqual,
    OpNotEqualStrict,
    OpLogicalAnd,
    OpLogicalOr,

    Null,
    Undefined,
    This,
    
    True,
    False,
    
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