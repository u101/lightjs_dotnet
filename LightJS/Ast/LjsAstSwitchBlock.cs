namespace LightJS.Ast;

public sealed class LjsAstSwitchBlock : ILjsAstNode
{
    public ILjsAstNode Expression { get; }

    public LjsAstSwitchBlock(ILjsAstNode expression)
    {
        Expression = expression;
    }
    
}