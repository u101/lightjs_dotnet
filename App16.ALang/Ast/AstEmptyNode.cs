using App16.ALang.Tokenizers;

namespace App16.ALang.Ast;

public sealed class AstEmptyNode : IAstNode
{
    public static readonly IAstNode Instance = new AstEmptyNode();

    public Token GetToken() => default;

    private AstEmptyNode() {}
}