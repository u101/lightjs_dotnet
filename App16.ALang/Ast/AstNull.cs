using App16.ALang.Tokenizers;

namespace App16.ALang.Ast;

public sealed class AstNull : AstNode,IAstValueNode
{
    public AstNull(Token token = default):base(token) {}
}