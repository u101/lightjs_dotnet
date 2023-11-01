using App16.ALang.Tokenizers;

namespace App16.ALang.Ast;

public sealed class AstTernaryIfOperation : AstNode,IAstValueNode
{
    public IAstNode Condition { get; }
    public IAstNode TrueExpression { get; }
    public IAstNode FalseExpression { get; }


    public AstTernaryIfOperation(
        IAstNode condition, 
        IAstNode trueExpression, 
        IAstNode falseExpression, 
        Token token = default) : base(token)
    {
        Condition = condition;
        TrueExpression = trueExpression;
        FalseExpression = falseExpression;
    }
    
}