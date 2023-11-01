namespace App16.ALang.Ast;

public sealed class AstIfBlock : AstNode
{
    public AstConditionalExpression If { get; }

    public List<AstConditionalExpression> ElseIfs { get; } = new();

    public IAstNode Else { get; set; } = AstEmptyNode.Instance;

    public AstIfBlock(AstConditionalExpression mainBlock)
    {
        If = mainBlock;
    }
}