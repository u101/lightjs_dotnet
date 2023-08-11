using LightJS.Tokenizer;

namespace LightJS.Ast;

public static class LjsAstBuilderUtils
{
    
    public static LjsAstAssignMode GetAssignMode(LjsTokenType tokenType) => tokenType switch
    {
        LjsTokenType.OpAssign => LjsAstAssignMode.Normal,
        LjsTokenType.OpPlusAssign => LjsAstAssignMode.PlusAssign,
        LjsTokenType.OpMinusAssign => LjsAstAssignMode.MinusAssign,
        LjsTokenType.OpMultAssign => LjsAstAssignMode.MulAssign,
        LjsTokenType.OpDivAssign => LjsAstAssignMode.DivAssign,
        LjsTokenType.OpBitOrAssign => LjsAstAssignMode.BitOrAssign,
        LjsTokenType.OpBitAndAssign => LjsAstAssignMode.BitAndAssign,
        LjsTokenType.OpLogicalOrAssign => LjsAstAssignMode.LogicalOrAssign,
        LjsTokenType.OpLogicalAndAssign => LjsAstAssignMode.LogicalAndAssign,
        _ => throw new Exception($"unsupported token type {tokenType}")
    };

    public static bool IsAssignOperator(LjsTokenType tokenType) => 
        tokenType is LjsTokenType.OpAssign 
            or LjsTokenType.OpPlusAssign
            or LjsTokenType.OpMinusAssign
            or LjsTokenType.OpMultAssign
            or LjsTokenType.OpDivAssign
            or LjsTokenType.OpBitOrAssign
            or LjsTokenType.OpBitAndAssign
            or LjsTokenType.OpLogicalOrAssign
            or LjsTokenType.OpLogicalAndAssign
    ;
    
    public static bool IsLiteral(LjsTokenType tokenType) =>
        tokenType == LjsTokenType.True ||
        tokenType == LjsTokenType.False ||
        tokenType == LjsTokenType.IntBinary ||
        tokenType == LjsTokenType.IntDecimal ||
        tokenType == LjsTokenType.IntHex ||
        tokenType == LjsTokenType.Float ||
        tokenType == LjsTokenType.FloatE ||
        tokenType == LjsTokenType.Null ||
        tokenType == LjsTokenType.Undefined ||
        tokenType == LjsTokenType.StringLiteral;
    
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
    
    /// <summary>
    /// for prefix operators only
    /// </summary>
    public static LjsAstUnaryOperationType GetUnaryOperationType(LjsTokenType tokenType) => tokenType switch
    {
        LjsTokenType.OpPlus => LjsAstUnaryOperationType.Plus,
        LjsTokenType.OpMinus => LjsAstUnaryOperationType.Minus,
            
        LjsTokenType.OpNegate => LjsAstUnaryOperationType.Negate,
        LjsTokenType.OpIncrement => LjsAstUnaryOperationType.PrefixIncrement,
        LjsTokenType.OpDecrement => LjsAstUnaryOperationType.PrefixDecrement,
        _ => throw new Exception($"unsupported unary operation token type {tokenType}")
    };

    public static ILjsAstNode CreateLiteralNode(LjsToken token, string sourceCodeString)
    {
        switch (token.TokenType)
        {
            case LjsTokenType.IntDecimal:
            case LjsTokenType.IntHex:
            case LjsTokenType.IntBinary:

                return new LjsAstLiteral<int>(
                    LjsTokenizerUtils.GetTokenIntValue(sourceCodeString, token));
                    
            case LjsTokenType.Float:
            case LjsTokenType.FloatE:

                return new LjsAstLiteral<double>(
                    LjsTokenizerUtils.GetTokenFloatValue(sourceCodeString, token));
                    
            case LjsTokenType.StringLiteral:
                return new LjsAstLiteral<string>(
                    sourceCodeString.Substring(token.Position.CharIndex, token.StringLength));
                    
            case LjsTokenType.True:
                        
                return new LjsAstLiteral<bool>(true);
                    
            case LjsTokenType.False:
                        
                return new LjsAstLiteral<bool>(false);
                    
            case LjsTokenType.Null:
                return new LjsAstNull();
                    
            case LjsTokenType.Undefined:
                return new LjsAstUndefined();
                    
                    
            default:
                throw new Exception($"unsupported literal token type {token.TokenType}");
        }
    }
    
}