namespace LightJS.Ast;

public sealed class LjsAstSwitchBlock : ILjsAstNode
{
    public ILjsAstNode Expression { get; }
    public LjsAstSequence Body { get; }

    public LjsAstSwitchBlock(ILjsAstNode expression, LjsAstSequence body)
    {
        Expression = expression;
        Body = body;
    }
    
}

public sealed class LjsAstSwitchCase : ILjsAstNode
{
    public ILjsAstNode Value { get; }

    public LjsAstSwitchCase(ILjsAstNode value)
    {
        Value = value;
    }
}

public sealed class LjsAstSwitchDefault : ILjsAstNode
{
    public LjsAstSwitchDefault() {}
}