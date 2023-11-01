using App16.ALang.Tokenizers;

namespace App16.ALang.Ast;

public sealed class AstGetThis : AstNode, IAstValueNode
{
    public AstGetThis(Token token = default) : base(token) {}
    
}