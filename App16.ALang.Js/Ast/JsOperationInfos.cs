using App16.ALang.Ast;
using App16.ALang.Js.Tokenizers;

namespace App16.ALang.Js.Ast;

public static class JsOperationInfos
{
    public static readonly List<AstUnaryOperationInfo> UnaryOperationInfos = new()
    {
        new AstUnaryOperationInfo(
            JsUnaryOperationTypes.LogicalNot, JsTokenTypes.OpLogicalNot, "!", 
            AstUnaryOperationAlign.Prefix, false),
        
        new AstUnaryOperationInfo(
            JsUnaryOperationTypes.BitNot, JsTokenTypes.OpBitNot, "~",
            AstUnaryOperationAlign.Prefix, false),
        
        new AstUnaryOperationInfo(
            JsUnaryOperationTypes.UnaryPlus, JsTokenTypes.OpPlus, "+",
            AstUnaryOperationAlign.Prefix, false),
        
        new AstUnaryOperationInfo(
            JsUnaryOperationTypes.UnaryMinus, JsTokenTypes.OpMinus, "-",
            AstUnaryOperationAlign.Prefix, false),
        
        new AstUnaryOperationInfo(
            JsUnaryOperationTypes.Increment, JsTokenTypes.OpIncrement, "++",
            AstUnaryOperationAlign.Any, true),
        
        new AstUnaryOperationInfo(
            JsUnaryOperationTypes.Decrement, JsTokenTypes.OpDecrement, "--",
            AstUnaryOperationAlign.Any, true),
    };

    public static readonly List<AstBinaryOperationInfo> BinaryOperationInfos = new()
    {
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.Plus, JsTokenTypes.OpPlus,
            JsOperationPriorityGroups.Addition, "+", AstOperationAssociativity.LeftToRight),

        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.Minus, JsTokenTypes.OpMinus,
            JsOperationPriorityGroups.Addition, "-", AstOperationAssociativity.LeftToRight),

        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.Multiply, JsTokenTypes.OpMultiply,
            JsOperationPriorityGroups.Multiplication, "*", AstOperationAssociativity.LeftToRight),

        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.Div, JsTokenTypes.OpDiv,
            JsOperationPriorityGroups.Multiplication, "/", AstOperationAssociativity.LeftToRight),

        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.Modulo, JsTokenTypes.OpModulo,
            JsOperationPriorityGroups.Multiplication, "%", AstOperationAssociativity.LeftToRight),

        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.Pow, JsTokenTypes.OpExponent,
            JsOperationPriorityGroups.Multiplication, "**", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.BitAnd, JsTokenTypes.OpBitAnd,
            JsOperationPriorityGroups.BitAnd, "&", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.BitOr, JsTokenTypes.OpBitOr,
            JsOperationPriorityGroups.BitOr, "|", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.BitXor, JsTokenTypes.OpBitXor,
            JsOperationPriorityGroups.BitXor, "^", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.BitLeftShift, JsTokenTypes.OpBitLeftShift,
            JsOperationPriorityGroups.BitShift, "<<", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.BitRightShift, JsTokenTypes.OpBitRightShift,
            JsOperationPriorityGroups.BitShift, ">>", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.BitUnsignedRightShift, JsTokenTypes.OpBitUnsignedRightShift,
            JsOperationPriorityGroups.BitShift, ">>>", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.Greater, JsTokenTypes.OpGreater,
            JsOperationPriorityGroups.Compare, ">", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.GreaterOrEqual, JsTokenTypes.OpGreaterOrEqual,
            JsOperationPriorityGroups.Compare, ">=", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.Less, JsTokenTypes.OpLess,
            JsOperationPriorityGroups.Compare, "<", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.LessOrEqual, JsTokenTypes.OpLessOrEqual,
            JsOperationPriorityGroups.Compare, "<=", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.Equals, JsTokenTypes.OpEquals,
            JsOperationPriorityGroups.EqualityCheck, "==", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.EqualsStrict, JsTokenTypes.OpEqualsStrict,
            JsOperationPriorityGroups.EqualityCheck, "===", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.NotEqual, JsTokenTypes.OpNotEqual,
            JsOperationPriorityGroups.EqualityCheck, "!=", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.NotEqualStrict, JsTokenTypes.OpNotEqualStrict,
            JsOperationPriorityGroups.EqualityCheck, "!==", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.LogicalAnd, JsTokenTypes.OpLogicalAnd,
            JsOperationPriorityGroups.LogicalAnd, "&&", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.LogicalOr, JsTokenTypes.OpLogicalOr,
            JsOperationPriorityGroups.LogicalOr, "||", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.Assign, JsTokenTypes.OpAssign,
            JsOperationPriorityGroups.Assign, "=", AstOperationAssociativity.RightToLeft),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.PlusAssign, JsTokenTypes.OpPlusAssign,
            JsOperationPriorityGroups.Assign, "+=", AstOperationAssociativity.RightToLeft),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.MinusAssign, JsTokenTypes.OpMinusAssign,
            JsOperationPriorityGroups.Assign, "-=", AstOperationAssociativity.RightToLeft),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.MulAssign, JsTokenTypes.OpMultAssign,
            JsOperationPriorityGroups.Assign, "*=", AstOperationAssociativity.RightToLeft),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.DivAssign, JsTokenTypes.OpDivAssign,
            JsOperationPriorityGroups.Assign, "/=", AstOperationAssociativity.RightToLeft),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.BitOrAssign, JsTokenTypes.OpBitOrAssign,
            JsOperationPriorityGroups.Assign, "|=", AstOperationAssociativity.RightToLeft),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.BitAndAssign, JsTokenTypes.OpBitAndAssign,
            JsOperationPriorityGroups.Assign, "&=", AstOperationAssociativity.RightToLeft),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.LogicalOrAssign, JsTokenTypes.OpLogicalOrAssign,
            JsOperationPriorityGroups.Assign, "||=", AstOperationAssociativity.RightToLeft),
        
        new AstBinaryOperationInfo(
            JsBinaryOperationTypes.LogicalAndAssign, JsTokenTypes.OpLogicalAndAssign,
            JsOperationPriorityGroups.Assign, "&&=", AstOperationAssociativity.RightToLeft),
    };
}