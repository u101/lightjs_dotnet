namespace LightJS.Ast;

public sealed class LjsAstConditionalExpression
{
    public ILjsAstNode Condition { get; }
    public ILjsAstNode Expression { get; }

    public LjsAstConditionalExpression(ILjsAstNode condition, ILjsAstNode expression)
    {
        Condition = condition;
        Expression = expression;
    }
}