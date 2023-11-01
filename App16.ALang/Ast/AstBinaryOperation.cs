using App16.ALang.Tokenizers;

namespace App16.ALang.Ast;

public sealed class AstBinaryOperation : AstNode, IAstValueNode
{
    public IAstNode LeftOperand { get; }
    public IAstNode RightOperand { get; }
    public AstBinaryOperationInfo OperationInfo { get; }

    public AstBinaryOperation(
        IAstNode leftOperand, 
        IAstNode rightOperand, 
        AstBinaryOperationInfo operationInfo,
        Token token = default) : base(token)
    {
        LeftOperand = leftOperand;
        RightOperand = rightOperand;
        OperationInfo = operationInfo;
    }
}

/*
public static class AstBinaryOperationUtil
{
    public static string GetOp(AstBinaryOperationType operationType)
    {
        switch (operationType)
        {
            case AstBinaryOperationType.Plus: return "+";
            case AstBinaryOperationType.Minus: return "-";
            case AstBinaryOperationType.Multiply: return "*";
            case AstBinaryOperationType.Div: return "/";
            case AstBinaryOperationType.Modulo: return "%";
            case AstBinaryOperationType.BitXor: return "^";

            case AstBinaryOperationType.BitAnd: return "&";
            case AstBinaryOperationType.BitOr: return "|";
            case AstBinaryOperationType.BitLeftShift: return "<<";
            case AstBinaryOperationType.BitRightShift: return ">>";
            case AstBinaryOperationType.BitUnsignedRightShift: return ">>>";

            case AstBinaryOperationType.Greater: return ">";
            case AstBinaryOperationType.GreaterOrEqual: return ">=";
            case AstBinaryOperationType.Less: return "<";
            case AstBinaryOperationType.LessOrEqual: return "<=";

            case AstBinaryOperationType.Equals: return "==";
            case AstBinaryOperationType.NotEqual: return "!=";

            case AstBinaryOperationType.LogicalAnd: return "&&";
            case AstBinaryOperationType.LogicalOr: return "||";

            case AstBinaryOperationType.Assign: return "=";
            case AstBinaryOperationType.PlusAssign: return "+=";
            case AstBinaryOperationType.MinusAssign: return "-=";
            case AstBinaryOperationType.MulAssign: return "*=";
            case AstBinaryOperationType.DivAssign: return "/=";
            case AstBinaryOperationType.BitOrAssign: return "|=";
            case AstBinaryOperationType.BitAndAssign: return "&=";
            case AstBinaryOperationType.LogicalOrAssign: return "||=";
            case AstBinaryOperationType.LogicalAndAssign: return "&&=";
            default:
                throw new ArgumentOutOfRangeException(nameof(operationType));
        }
    }
}

public enum AstBinaryOperationType
{
    Plus,
    Minus,
    Multiply,
    Div,
    Modulo,
    
    BitAnd,
    BitOr,
    BitXor,
    
    BitLeftShift,
    BitRightShift,
    BitUnsignedRightShift,
    
    Greater,
    GreaterOrEqual,
    Less,
    LessOrEqual,
    
    Equals,
    NotEqual,
    
    LogicalAnd,
    LogicalOr,
    
    Assign,
    PlusAssign,
    MinusAssign,
    MulAssign,
    DivAssign,
    BitOrAssign,
    BitAndAssign,
    LogicalOrAssign,
    LogicalAndAssign
}
*/