using App16.ALang.Tokenizers;

namespace App16.ALang.Ast;

public sealed class AstReturn : AstNode
{
    public IAstNode Expression { get; }

    public AstReturn(IAstNode expression, Token token = default):base(token)
    {
        Expression = expression;
    }

    public AstReturn(Token token = default):base(token)
    {
        Expression = AstEmptyNode.Instance;
    }
    
}