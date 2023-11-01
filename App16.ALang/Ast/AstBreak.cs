using App16.ALang.Tokenizers;

namespace App16.ALang.Ast;

public sealed class AstBreak : AstNode
{
    public AstBreak(Token token = default):base(token) {}
}