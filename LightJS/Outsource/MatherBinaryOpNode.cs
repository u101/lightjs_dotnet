namespace LightJS.Outsource;

public class MatherBinaryOpNode : IMatherNode
{
    public IMatherNode LeftOperand { get; }
    public IMatherNode RightOperand { get; }
    public MatherBinaryOp OperationType { get; }

    public MatherBinaryOpNode(IMatherNode leftOperand, IMatherNode rightOperand, MatherBinaryOp operationType)
    {
        LeftOperand = leftOperand;
        RightOperand = rightOperand;
        OperationType = operationType;
    }
}