using LightJS.Tokenizer;

namespace LightJS.Ast;

public static class LjsAstBuilderUtils
{
    
    public static LjsAstBinaryOperationType GetBinaryOperationType(LjsTokenType tokenType) => tokenType switch
    {
        LjsTokenType.OpPlus => LjsAstBinaryOperationType.Plus,
        LjsTokenType.OpMinus => LjsAstBinaryOperationType.Minus,
        
        LjsTokenType.OpGreater => LjsAstBinaryOperationType.Greater,
        LjsTokenType.OpLess => LjsAstBinaryOperationType.Less,
        LjsTokenType.OpMultiply => LjsAstBinaryOperationType.Multiply,
        LjsTokenType.OpDiv => LjsAstBinaryOperationType.Div,
        LjsTokenType.OpBitAnd => LjsAstBinaryOperationType.BitAnd,
        LjsTokenType.OpBitOr => LjsAstBinaryOperationType.BitOr,
        
        LjsTokenType.OpEquals => LjsAstBinaryOperationType.Equals,
        LjsTokenType.OpEqualsStrict => LjsAstBinaryOperationType.EqualsStrict,
        LjsTokenType.OpGreaterOrEqual => LjsAstBinaryOperationType.GreaterOrEqual,
        LjsTokenType.OpLessOrEqual => LjsAstBinaryOperationType.LessOrEqual,
        LjsTokenType.OpNotEqual => LjsAstBinaryOperationType.NotEqual,
        LjsTokenType.OpNotEqualStrict => LjsAstBinaryOperationType.NotEqualStrict,
        LjsTokenType.OpLogicalAnd => LjsAstBinaryOperationType.LogicalAnd,
        LjsTokenType.OpLogicalOr => LjsAstBinaryOperationType.LogicalOr,
        
        _ => throw new Exception($"unsupported binary operator token {tokenType}")
    };
    
}