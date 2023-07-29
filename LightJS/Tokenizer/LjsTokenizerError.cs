namespace LightJS.Tokenizer;

public class LjsTokenizerError : Exception
{
    public string ErrorMessage { get; }
    public LjsTokenPosition TokenPosition { get; }
    
    public LjsTokenizerError(string errorMessage, LjsTokenPosition tokenPosition):
        base($"syntax error at line:{tokenPosition.Line} col:{tokenPosition.Column}. {errorMessage}")
    {
        ErrorMessage = errorMessage;
        TokenPosition = tokenPosition;
    }
    
}