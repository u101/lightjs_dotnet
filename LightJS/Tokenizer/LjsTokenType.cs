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
    OpEqual,
    OpPlus,
    OpMinus,
    OpMultiply,
    OpSlash,
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