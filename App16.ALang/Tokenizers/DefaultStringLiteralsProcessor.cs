namespace App16.ALang.Tokenizers;

/// <summary>
/// Process string literals starting with \' or \"
/// </summary>
public class DefaultStringLiteralsProcessor : ITokensProcessor
{
    private const char DoubleQuotes = '"';
    private const char SingleQuotes = '\'';
    private const char NewLine = '\n';
    private const char BackSlash = '\\';
    
    private readonly int _stringLiteralType;

    public DefaultStringLiteralsProcessor(int stringLiteralType)
    {
        _stringLiteralType = stringLiteralType;
    }

    public bool Process(TokenizerContext ctx)
    {
        var reader = ctx.CharsReader;
        var c = reader.CurrentChar;

        if (c != DoubleQuotes && c != SingleQuotes) return false;


        reader.MoveForward();// we ignore quotes
        
        var startIndex = reader.CurrentIndex; 
        var tokenPos = reader.CurrentTokenPosition;

        var hasEscapeChar = false;

        while (reader.CurrentChar != c || hasEscapeChar)
        {
            if (!reader.HasNextChar || reader.NextChar == NewLine)
            {
                throw new TokenizerError(
                    "unterminated string literal", reader.CurrentTokenPosition);
            }

            hasEscapeChar = !hasEscapeChar && reader.CurrentChar == BackSlash;

            reader.MoveForward();
        }

        var ln = reader.CurrentIndex - startIndex; // end index is exclusive
        
        ctx.AddToken(new Token(_stringLiteralType, tokenPos, ln));

        return true;
    }
}