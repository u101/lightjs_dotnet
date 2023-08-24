namespace LightJS.Ast;

public sealed class LjsAstIfBlock : ILjsAstNode
{
    public LjsAstConditionalExpression MainBlock { get; }

    public List<LjsAstConditionalExpression> ConditionalAlternatives { get; } = new();

    public ILjsAstNode? ElseBlock { get; set; } = null;

    public LjsAstIfBlock(LjsAstConditionalExpression mainBlock)
    {
        MainBlock = mainBlock;
    }
    
}