namespace App16.ALang.Ast;

public sealed class AstUnaryOperationInfo : IEquatable<AstUnaryOperationInfo>
{
    public int OperatorId { get; }
    public int OperatorTokenType { get; }
    public string OperatorStringValue { get; }
    public AstUnaryOperationAlign Align { get; }
    public bool IsVarModifier { get; }

    public AstUnaryOperationInfo(
        int operatorId,
        int operatorTokenType,
        string operatorStringValue,
        AstUnaryOperationAlign align,
        bool isVarModifier)
    {
        OperatorId = operatorId;
        OperatorTokenType = operatorTokenType;
        OperatorStringValue = operatorStringValue;
        Align = align;
        IsVarModifier = isVarModifier;
    }

    public bool Equals(AstUnaryOperationInfo? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        
        return OperatorId == other.OperatorId && 
               OperatorTokenType == other.OperatorTokenType && 
               OperatorStringValue == other.OperatorStringValue;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((AstUnaryOperationInfo)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(OperatorId, OperatorTokenType, OperatorStringValue);
    }
}

public enum AstUnaryOperationAlign
{
    Prefix,
    Postfix,
    Any
}