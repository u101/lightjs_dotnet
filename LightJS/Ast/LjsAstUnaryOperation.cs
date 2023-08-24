namespace LightJS.Ast;

public sealed class LjsAstUnaryOperation : ILjsAstNode
{
    public ILjsAstNode Operand { get; }
    public LjsAstUnaryOperationType OperatorType { get; }

    
    public LjsAstUnaryOperation(ILjsAstNode operand, LjsAstUnaryOperationType operatorType)
    {
        Operand = operand;
        OperatorType = operatorType;
    }
    
}