using LightJS.Tokenizer;

namespace LightJS;

public class LjsSyntaxError : Exception
{
    public string ErrorMessage { get; }
    public int Line { get; }
    public int Col { get; }

    public LjsSyntaxError(string errorMessage) : base($"syntax error : {errorMessage}") {}
    
    public LjsSyntaxError(string errorMessage, LjsTokenPosition tokenPosition):
        this(errorMessage, tokenPosition.Line, tokenPosition.Column)
    { }
    
    public LjsSyntaxError(LjsTokenPosition tokenPosition):
        this(tokenPosition.Line, tokenPosition.Column)
    { }
    
    public LjsSyntaxError(string errorMessage, int line, int col):
        base($"syntax error at line:{line} col:{col}. {errorMessage}")
    {
        ErrorMessage = errorMessage;
        Line = line;
        Col = col;
    }
    
    public LjsSyntaxError(int line, int col):
        base($"syntax error at line:{line} col:{col}.")
    {
        ErrorMessage = string.Empty;
        Line = line;
        Col = col;
    }
}