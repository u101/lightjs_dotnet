namespace LightJS.Tokenizer;

public class LjsTokenizerError : Exception
{
    public int CharIndex { get; }
    
    public LjsTokenizerError(int charIndex):base($"syntax error at char {charIndex}")
    {
        CharIndex = charIndex;
    }
    
}