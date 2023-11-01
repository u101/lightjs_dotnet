namespace App16.ALang.Ast;

public class AstBinaryOperationInfo : IEquatable<AstBinaryOperationInfo>
{
    public int OperatorId { get; }
    public int OperatorTokenType { get; }
    public int OperationOrder { get; }
    public string OperatorStringValue { get; }
    public AstOperationAssociativity OperationAssociativity { get; }

    public AstBinaryOperationInfo(
        int operatorId,
        int operatorTokenType, 
        int operationOrder,
        string operatorStringValue,
        AstOperationAssociativity operationAssociativity)
    {
        OperatorId = operatorId;
        OperatorTokenType = operatorTokenType;
        OperationOrder = operationOrder;
        OperatorStringValue = operatorStringValue;
        OperationAssociativity = operationAssociativity;
    }

    public bool Equals(AstBinaryOperationInfo? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return OperatorId == other.OperatorId && 
               OperatorTokenType == other.OperatorTokenType && 
               OperationOrder == other.OperationOrder && 
               OperationAssociativity == other.OperationAssociativity;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((AstBinaryOperationInfo)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            OperatorId, OperatorTokenType, OperationOrder, (int)OperationAssociativity);
    }
}