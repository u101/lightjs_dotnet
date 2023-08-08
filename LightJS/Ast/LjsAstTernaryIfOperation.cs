namespace LightJS.Ast;

public class LjsAstTernaryIfOperation : ILjsAstNode
{
    public ILjsAstNode Condition { get; }
    public ILjsAstNode TrueExpression { get; }
    public ILjsAstNode FalseExpression { get; }


    public LjsAstTernaryIfOperation(ILjsAstNode condition, ILjsAstNode trueExpression, ILjsAstNode falseExpression)
    {
        Condition = condition;
        TrueExpression = trueExpression;
        FalseExpression = falseExpression;
    }

    public IEnumerable<ILjsAstNode> ChildNodes => new[] { Condition, TrueExpression, FalseExpression };
    
    public bool HasChildNodes => true;
}