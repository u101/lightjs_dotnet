using App16.ALang.Tokenizers;

namespace App16.ALang.Ast.Errors;

public class AstUnexpectedEofError : Exception
{
    private const string MessageString = "unexpected EOF";
    
    public Token Token { get; }
    
    
    public AstUnexpectedEofError(Token currentToken) : base(MessageString)
    {
        Token = currentToken;
    }
}