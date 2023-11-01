using App16.ALang.Tokenizers;

namespace App16.ALang.Ast;

public sealed class AstGetId : AstNode, IAstValueNode
{
    public string IdentifierName { get; }

    public AstGetId(string identifierName, Token token = default):base(token)
    {
        IdentifierName = identifierName;
    }
    
}