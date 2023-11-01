namespace App16.ALang.Tokenizers;

public sealed class TokenizerContext
{
    public ICharsReader CharsReader { get; }
    public string SourceCodeString { get; }
    public List<Token> Tokens { get; } = new();

    public TokenizerContext(ICharsReader charsReader, string sourceCodeString)
    {
        CharsReader = charsReader;
        SourceCodeString = sourceCodeString;
    }

    public void AddToken(Token token)
    {
        Tokens.Add(token);
    }

    public Token RemoveLastToken()
    {
        var ln = Tokens.Count;
        
        if (ln == 0)
            throw new Exception();

        
        var t = Tokens[ln - 1];
        Tokens.RemoveAt(ln - 1);
        return t;
    }
    
    public void ReplaceLastToken(Token newToken)
    {
        Tokens[^1] = newToken;
    }

    public Token PreviousToken => 
        Tokens.Count > 0 ? Tokens[^1] : default;
    
}