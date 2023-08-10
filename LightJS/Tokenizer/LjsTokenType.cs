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