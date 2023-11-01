namespace App16.ALang.Ast;

public sealed class AstConditionalExpression
{
    public IAstNode Condition { get; }
    public IAstNode Expression { get; }

    public AstConditionalExpression(IAstNode condition, IAstNode expression)
    {
        Condition = condition;
        Expression = expression;
    }
}