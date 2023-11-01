using App16.ALang.Js.Tokenizers;

namespace App16.ALang.Js.Ast.Builders;

public static class JsAstBuilderUtils
{
    public static bool IsLiteral(int tokenType) =>
        tokenType == JsTokenTypes.True ||
        tokenType == JsTokenTypes.False ||
        tokenType == JsTokenTypes.IntBinary ||
        tokenType == JsTokenTypes.IntDecimal ||
        tokenType == JsTokenTypes.IntHex ||
        tokenType == JsTokenTypes.Float ||
        tokenType == JsTokenTypes.FloatE ||
        tokenType == JsTokenTypes.FloatNaN ||
        tokenType == JsTokenTypes.Null ||
        tokenType == JsTokenTypes.Undefined ||
        tokenType == JsTokenTypes.StringLiteral;
    
    public static bool IsDefinitelyPrefixUnaryOperator(int tokenType) => tokenType is
        JsTokenTypes.OpLogicalNot or
        JsTokenTypes.OpBitNot or
        JsTokenTypes.OpIncrement or
        JsTokenTypes.OpDecrement;

    public static bool IsKeyword(int tokenType) =>
        tokenType == JsTokenTypes.Var ||
        tokenType == JsTokenTypes.Let ||
        tokenType == JsTokenTypes.Const ||
        tokenType == JsTokenTypes.Function ||

        tokenType == JsTokenTypes.Return ||
        tokenType == JsTokenTypes.Break ||
        tokenType == JsTokenTypes.Continue ||

        tokenType == JsTokenTypes.If ||
        tokenType == JsTokenTypes.ElseIf ||
        tokenType == JsTokenTypes.Else ||
        tokenType == JsTokenTypes.Switch ||
        tokenType == JsTokenTypes.Case ||
        tokenType == JsTokenTypes.Default ||
        tokenType == JsTokenTypes.While ||
        tokenType == JsTokenTypes.Do ||
        tokenType == JsTokenTypes.For ||
        tokenType == JsTokenTypes.In;
    
    public static bool IsPossiblyPrefixUnaryOperator(int tokenType) =>
        IsDefinitelyPrefixUnaryOperator(tokenType) || 
        tokenType == JsTokenTypes.OpPlus ||
        tokenType == JsTokenTypes.OpMinus;
}