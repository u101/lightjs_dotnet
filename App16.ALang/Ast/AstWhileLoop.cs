using App16.ALang.Tokenizers;

namespace App16.ALang.Ast;

public sealed class AstWhileLoop : AstNode
{
    public IAstNode Condition { get; }
    public IAstNode Body { get; }

    public AstWhileLoop(IAstNode condition, IAstNode body, Token token = default) : base(token)
    {
        Condition = condition;
        Body = body;
    }
    
}