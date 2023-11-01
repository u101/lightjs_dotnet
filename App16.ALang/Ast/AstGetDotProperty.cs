using App16.ALang.Tokenizers;

namespace App16.ALang.Ast;

public sealed class AstGetDotProperty : AstNode, IAstValueNode
{
    public IAstNode PropertySource { get; }
    public string PropertyName { get; }


    public AstGetDotProperty(
        IAstNode propertySource, 
        string propertyName, 
        Token token = default) : base(token)
    {
        PropertySource = propertySource;
        PropertyName = propertyName;
    }
}