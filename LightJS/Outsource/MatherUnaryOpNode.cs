using LightJS.Tokenizer;

namespace LightJS.Outsource;

public class MatherUnaryOpNode : IMatherNode
{
    public IMatherNode Operand { get; }
    public LjsTokenType OperatorType { get; }

    public MatherUnaryOpNode(IMatherNode operand, LjsTokenType operatorType)
    {
        Operand = operand;
        OperatorType = operatorType;
    }
}