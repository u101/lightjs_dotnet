using App16.ALang.Tokenizers;

namespace App16.ALang.Ast;

public sealed class AstGetSquareBracketsProp : AstNode, IAstValueNode
{
    public IAstNode PropertySource { get; }
    public IAstNode Expression { get; }

    public AstGetSquareBracketsProp(
        IAstNode propertySource, 
        IAstNode expression, 
        Token token = default) : base(token)
    {
        PropertySource = propertySource;
        Expression = expression;
    }
}