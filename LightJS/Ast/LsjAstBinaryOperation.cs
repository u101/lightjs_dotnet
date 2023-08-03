using LightJS.Tokenizer;

namespace LightJS.Ast;

public class LsjAstBinaryOperation : ILjsAstNode
{
    public ILjsAstNode LeftOperand { get; }
    public ILjsAstNode RightOperand { get; }
    public LjsTokenType OperatorType { get; }
    
    

    public LsjAstBinaryOperation(ILjsAstNode leftOperand, ILjsAstNode rightOperand, LjsTokenType operatorType)
    {
        LeftOperand = leftOperand;
        RightOperand = rightOperand;
        OperatorType = operatorType;
    }

    public IEnumerable<ILjsAstNode> ChildNodes => new[] { LeftOperand, RightOperand };
    
    public bool HasChildNodes => true;
}