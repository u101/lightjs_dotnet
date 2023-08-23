namespace LightJS.Ast;

public class LjsAstForLoop : ILjsAstNode
{
    public ILjsAstNode InitExpression { get; }
    public ILjsAstNode Condition { get; }
    public ILjsAstNode IterationExpression { get; }
    public ILjsAstNode Body { get; }

    public LjsAstForLoop(
        ILjsAstNode initExpression,
        ILjsAstNode condition, 
        ILjsAstNode iterationExpression,
        ILjsAstNode body)
    {
        InitExpression = initExpression;
        Condition = condition;
        IterationExpression = iterationExpression;
        Body = body;
    }
}