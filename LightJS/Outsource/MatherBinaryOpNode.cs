namespace LightJS.Outsource;

public class MatherBinaryOpNode : IMatherNode
{
    public IMatherNode LeftOperand { get; }
    public IMatherNode RightOperand { get; }
    public MatherTokenType OperationType { get; }

    public MatherBinaryOpNode(IMatherNode leftOperand, IMatherNode rightOperand, MatherTokenType operationType)
    {
        LeftOperand = leftOperand;
        RightOperand = rightOperand;
        OperationType = operationType;
    }
}