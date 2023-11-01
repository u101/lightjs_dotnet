using App16.ALang.Tokenizers;

namespace App16.ALang.Ast;

public sealed class AstForLoop : AstNode
{
    public IAstNode InitExpression { get; }
    public IAstNode Condition { get; }
    public IAstNode IterationExpression { get; }
    public IAstNode Body { get; }

    public AstForLoop(
        IAstNode initExpression,
        IAstNode condition, 
        IAstNode iterationExpression,
        IAstNode body,
        Token token = default) : base(token)
    {
        InitExpression = initExpression;
        Condition = condition;
        IterationExpression = iterationExpression;
        Body = body;
    }
}