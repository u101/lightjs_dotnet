namespace LightJS.Ast;

public sealed class LjsAstBinaryOperation : ILjsAstNode
{
    public ILjsAstNode LeftOperand { get; }
    public ILjsAstNode RightOperand { get; }
    public LjsAstBinaryOperationType OperatorType { get; }
    
    public LjsAstBinaryOperation(
        ILjsAstNode leftOperand, 
        ILjsAstNode rightOperand, 
        LjsAstBinaryOperationType operatorType)
    {
        LeftOperand = leftOperand;
        RightOperand = rightOperand;
        OperatorType = operatorType;
    }
}