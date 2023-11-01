using App16.ALang.Ast;

namespace App16.ALang.Tests.DefaultLang;

public static class DefOperationInfos
{

    public static readonly List<AstUnaryOperationInfo> UnaryOperationInfos = new()
    {
        new AstUnaryOperationInfo(
            DefUnaryOperationTypes.LogicalNot, DefTokenTypes.OpLogicalNot, "!", 
            AstUnaryOperationAlign.Prefix, false),
        
        new AstUnaryOperationInfo(
            DefUnaryOperationTypes.BitNot, DefTokenTypes.OpBitNot, "~",
            AstUnaryOperationAlign.Prefix, false),
        
        new AstUnaryOperationInfo(
            DefUnaryOperationTypes.UnaryPlus, DefTokenTypes.OpPlus, "+",
            AstUnaryOperationAlign.Prefix, false),
        
        new AstUnaryOperationInfo(
            DefUnaryOperationTypes.UnaryMinus, DefTokenTypes.OpMinus, "-",
            AstUnaryOperationAlign.Prefix, false),
        
        new AstUnaryOperationInfo(
            DefUnaryOperationTypes.Increment, DefTokenTypes.OpIncrement, "++",
            AstUnaryOperationAlign.Any, true),
        
        new AstUnaryOperationInfo(
            DefUnaryOperationTypes.Decrement, DefTokenTypes.OpDecrement, "--",
            AstUnaryOperationAlign.Any, true),
    };

    public static readonly List<AstBinaryOperationInfo> BinaryOperationInfos = new()
    {
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.Plus, DefTokenTypes.OpPlus,
            DefOperationPriorityGroups.Addition, "+", AstOperationAssociativity.LeftToRight),

        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.Minus, DefTokenTypes.OpMinus,
            DefOperationPriorityGroups.Addition, "-", AstOperationAssociativity.LeftToRight),

        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.Multiply, DefTokenTypes.OpMultiply,
            DefOperationPriorityGroups.Multiplication, "*", AstOperationAssociativity.LeftToRight),

        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.Div, DefTokenTypes.OpDiv,
            DefOperationPriorityGroups.Multiplication, "/", AstOperationAssociativity.LeftToRight),

        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.Modulo, DefTokenTypes.OpModulo,
            DefOperationPriorityGroups.Multiplication, "%", AstOperationAssociativity.LeftToRight),

        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.Pow, DefTokenTypes.OpExponent,
            DefOperationPriorityGroups.Multiplication, "**", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.BitAnd, DefTokenTypes.OpBitAnd,
            DefOperationPriorityGroups.BitAnd, "&", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.BitOr, DefTokenTypes.OpBitOr,
            DefOperationPriorityGroups.BitOr, "|", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.BitXor, DefTokenTypes.OpBitXor,
            DefOperationPriorityGroups.BitXor, "^", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.BitLeftShift, DefTokenTypes.OpBitLeftShift,
            DefOperationPriorityGroups.BitShift, "<<", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.BitRightShift, DefTokenTypes.OpBitRightShift,
            DefOperationPriorityGroups.BitShift, ">>", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.BitUnsignedRightShift, DefTokenTypes.OpBitUnsignedRightShift,
            DefOperationPriorityGroups.BitShift, ">>>", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.Greater, DefTokenTypes.OpGreater,
            DefOperationPriorityGroups.Compare, ">", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.GreaterOrEqual, DefTokenTypes.OpGreaterOrEqual,
            DefOperationPriorityGroups.Compare, ">=", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.Less, DefTokenTypes.OpLess,
            DefOperationPriorityGroups.Compare, "<", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.LessOrEqual, DefTokenTypes.OpLessOrEqual,
            DefOperationPriorityGroups.Compare, "<=", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.Equals, DefTokenTypes.OpEquals,
            DefOperationPriorityGroups.EqualityCheck, "==", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.EqualsStrict, DefTokenTypes.OpEqualsStrict,
            DefOperationPriorityGroups.EqualityCheck, "===", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.NotEqual, DefTokenTypes.OpNotEqual,
            DefOperationPriorityGroups.EqualityCheck, "!=", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.NotEqualStrict, DefTokenTypes.OpNotEqualStrict,
            DefOperationPriorityGroups.EqualityCheck, "!==", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.LogicalAnd, DefTokenTypes.OpLogicalAnd,
            DefOperationPriorityGroups.LogicalAnd, "&&", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.LogicalOr, DefTokenTypes.OpLogicalOr,
            DefOperationPriorityGroups.LogicalOr, "||", AstOperationAssociativity.LeftToRight),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.Assign, DefTokenTypes.OpAssign,
            DefOperationPriorityGroups.Assign, "=", AstOperationAssociativity.RightToLeft),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.PlusAssign, DefTokenTypes.OpPlusAssign,
            DefOperationPriorityGroups.Assign, "+=", AstOperationAssociativity.RightToLeft),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.MinusAssign, DefTokenTypes.OpMinusAssign,
            DefOperationPriorityGroups.Assign, "-=", AstOperationAssociativity.RightToLeft),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.MulAssign, DefTokenTypes.OpMultAssign,
            DefOperationPriorityGroups.Assign, "*=", AstOperationAssociativity.RightToLeft),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.DivAssign, DefTokenTypes.OpDivAssign,
            DefOperationPriorityGroups.Assign, "/=", AstOperationAssociativity.RightToLeft),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.BitOrAssign, DefTokenTypes.OpBitOrAssign,
            DefOperationPriorityGroups.Assign, "|=", AstOperationAssociativity.RightToLeft),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.BitAndAssign, DefTokenTypes.OpBitAndAssign,
            DefOperationPriorityGroups.Assign, "&=", AstOperationAssociativity.RightToLeft),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.LogicalOrAssign, DefTokenTypes.OpLogicalOrAssign,
            DefOperationPriorityGroups.Assign, "||=", AstOperationAssociativity.RightToLeft),
        
        new AstBinaryOperationInfo(
            DefBinaryOperationTypes.LogicalAndAssign, DefTokenTypes.OpLogicalAndAssign,
            DefOperationPriorityGroups.Assign, "&&=", AstOperationAssociativity.RightToLeft),
    };

}