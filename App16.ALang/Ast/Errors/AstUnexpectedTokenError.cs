using App16.ALang.Tokenizers;

namespace App16.ALang.Ast.Errors;

public class AstUnexpectedTokenError : Exception
{
    private const string MessageString = "unexpected token";
    
    public Token Token { get; }
    public int ExpectedTokenType { get; }
    
    public AstUnexpectedTokenError(int expectedTokenType, Token currentToken) : base(MessageString)
    {
        Token = currentToken;
        ExpectedTokenType = expectedTokenType;
    }
    
    public AstUnexpectedTokenError(Token currentToken) : base(MessageString)
    {
        Token = currentToken;
        ExpectedTokenType = 0;
    }
    
    
}