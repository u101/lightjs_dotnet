using App16.ALang.Tokenizers;

namespace App16.ALang.Js.Errors;

public class JsSyntaxError : Exception
{
    public string ErrorMessage { get; }
    public TokenPosition TokenPosition { get; }


    public JsSyntaxError(string errorMessage, TokenPosition tokenPosition) : 
        base($"syntax error at line:{tokenPosition.Line} col:{tokenPosition.Column}. {errorMessage}")
    {
        ErrorMessage = errorMessage;
        TokenPosition = tokenPosition;
    }
    
    public JsSyntaxError(string errorMessage) : 
        base(errorMessage)
    {
        ErrorMessage = errorMessage;
        TokenPosition = default;
    }
}