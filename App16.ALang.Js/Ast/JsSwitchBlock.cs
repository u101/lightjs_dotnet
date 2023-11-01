using App16.ALang.Ast;
using App16.ALang.Tokenizers;

namespace App16.ALang.Js.Ast;

public sealed class JsSwitchBlock : AstNode
{
    
    public IAstNode Expression { get; }
    public AstSequence Body { get; }

    public JsSwitchBlock(IAstNode expression, AstSequence body, Token token = default) : base(token)
    {
        Expression = expression;
        Body = body;
    }
    
}

public sealed class JsSwitchCase : AstNode
{
    public IAstNode Value { get; }

    public JsSwitchCase(IAstNode value, Token token = default) : base(token)
    {
        Value = value;
    }
}

public sealed class JsSwitchDefault : AstNode
{
    public JsSwitchDefault(Token token = default):base(token) {}
}