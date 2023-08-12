using LightJS.Tokenizer;

namespace LightJS.Outsource;

public class MatherBinaryOpNode : IMatherNode
{
    public IMatherNode LeftOperand { get; }
    public IMatherNode RightOperand { get; }
    public LjsTokenType OperationType { get; }

    public MatherBinaryOpNode(IMatherNode leftOperand, IMatherNode rightOperand, LjsTokenType operationType)
    {
        LeftOperand = leftOperand;
        RightOperand = rightOperand;
        OperationType = operationType;
    }
}