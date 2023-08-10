namespace LightJS.Ast;

public class LjsAstUnaryOperation : ILjsAstNode
{
    public ILjsAstNode Operand { get; }
    public LjsAstUnaryOperationType OperatorType { get; }

    
    public LjsAstUnaryOperation(ILjsAstNode operand, LjsAstUnaryOperationType operatorType)
    {
        Operand = operand;
        OperatorType = operatorType;
    }

    public IEnumerable<ILjsAstNode> ChildNodes => new[] { Operand };
    
    public bool HasChildNodes => true;
}