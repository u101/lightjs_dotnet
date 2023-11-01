using App16.ALang.Tokenizers;

namespace App16.ALang.Ast;

public interface IAstNode
{
    Token GetToken();
}

public abstract class AstNode : IAstNode
{
    private readonly Token _token;

    public Token GetToken() => _token;

    protected AstNode()
    {
        _token = default;
    }

    protected AstNode(Token token)
    {
        _token = token;
    }
}