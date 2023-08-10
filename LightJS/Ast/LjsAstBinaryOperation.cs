using LightJS.Tokenizer;

namespace LightJS.Ast;

public class LjsAstBinaryOperation : ILjsAstNode
{
    public ILjsAstNode LeftOperand { get; }
    public ILjsAstNode RightOperand { get; }
    public LjsTokenType OperatorType { get; }
    
    
    // todo replace LjsTokenType with LjsAstBinaryOperationType
    public LjsAstBinaryOperation(ILjsAstNode leftOperand, ILjsAstNode rightOperand, LjsTokenType operatorType)
    {
        LeftOperand = leftOperand;
        RightOperand = rightOperand;
        OperatorType = operatorType;
    }

    public IEnumerable<ILjsAstNode> ChildNodes => new[] { LeftOperand, RightOperand };
    
    public bool HasChildNodes => true;
}