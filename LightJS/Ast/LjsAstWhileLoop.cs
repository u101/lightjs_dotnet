namespace LightJS.Ast;

public sealed class LjsAstWhileLoop : ILjsAstNode
{
    public ILjsAstNode Condition { get; }
    public ILjsAstNode Body { get; }

    public LjsAstWhileLoop(ILjsAstNode condition, ILjsAstNode body)
    {
        Condition = condition;
        Body = body;
    }
    
}