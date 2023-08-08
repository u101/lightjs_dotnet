using LightJS.Tokenizer;

namespace LightJS.Ast;

public class LjsAstUnaryOperation : ILjsAstNode
{
    public ILjsAstNode Operand { get; }
    public LjsTokenType OperatorType { get; }


    public LjsAstUnaryOperation(ILjsAstNode operand, LjsTokenType operatorType)
    {
        Operand = operand;
        OperatorType = operatorType;
    }

    public IEnumerable<ILjsAstNode> ChildNodes => new[] { Operand };
    
    public bool HasChildNodes => true;
}