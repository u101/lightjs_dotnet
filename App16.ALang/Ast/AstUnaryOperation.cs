using App16.ALang.Tokenizers;

namespace App16.ALang.Ast;

public sealed class AstUnaryOperation : AstNode, IAstValueNode
{
    public IAstNode Operand { get; }
    public AstUnaryOperationInfo OperationInfo { get; }
    public bool IsPrefix { get; }

    public AstUnaryOperation(
        IAstNode operand, 
        AstUnaryOperationInfo operationInfo,
        bool isPrefix = true, Token token = default) : base(token)
    {
        Operand = operand;
        OperationInfo = operationInfo;
        IsPrefix = isPrefix;
    }
    
}

/*
public static class AstUnaryOperationUtil
{
    public static string GetOp(AstUnaryOperationType operationType)
    {
        switch (operationType)
        {
            case AstUnaryOperationType.LogicalNot: return "!";
            case AstUnaryOperationType.BitNot: return "~";
            case AstUnaryOperationType.UnaryPlus: return "+";
            case AstUnaryOperationType.UnaryMinus: return "-";
            case AstUnaryOperationType.Increment: return "++";
            case AstUnaryOperationType.Decrement: return "--";
            default:
                throw new ArgumentOutOfRangeException(nameof(operationType));
        }
    }
}

public enum AstUnaryOperationType
{
    LogicalNot,
    BitNot,
    UnaryPlus,
    UnaryMinus,
    Increment,
    Decrement,
}
*/