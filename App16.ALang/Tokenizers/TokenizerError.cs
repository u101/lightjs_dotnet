namespace App16.ALang.Tokenizers;

public class TokenizerError : Exception
{
    public string ErrorMessage { get; }
    
    public TokenPosition TokenPosition { get; }

    public TokenizerError(string errorMessage, TokenPosition tokenPosition) : 
        base($"TokenizerError:{errorMessage} at {tokenPosition}")
    {
        ErrorMessage = errorMessage;
        TokenPosition = tokenPosition;
    }
    
}