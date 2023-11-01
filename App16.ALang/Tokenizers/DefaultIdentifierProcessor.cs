namespace App16.ALang.Tokenizers;

/// <summary>
/// Process identifiers and keywords starting with char [a-zA-Z_$] and containing chars [a-zA-Z0-9_$]
/// </summary>
public class DefaultIdentifierProcessor : ITokensProcessor
{
    private readonly int _identifierTokenType;
    private readonly Dictionary<string, int> _keywordsMap;

    public DefaultIdentifierProcessor(
        int identifierTokenType, 
        Dictionary<string, int> keywordsMap)
    {
        _identifierTokenType = identifierTokenType;
        _keywordsMap = keywordsMap;
    }

    public bool Process(TokenizerContext ctx)
    {
        var reader = ctx.CharsReader;
        var c = reader.CurrentChar;

        if (!IsIdentifierLetterChar(c)) return false;

        var startIndex = reader.CurrentIndex;
        var tokenPos = reader.CurrentTokenPosition;

        while (reader.HasNextChar &&
               (IsIdentifierLetterChar(reader.NextChar) || char.IsDigit(reader.NextChar)))
        {
            reader.MoveForward();
        }

        var ln = (reader.CurrentIndex + 1) - startIndex;

        var wordSpan = ctx.SourceCodeString.Substring(startIndex, ln);

        if (_keywordsMap.TryGetValue(wordSpan, out var tokenType))
        {
            ctx.AddToken(new Token(tokenType, tokenPos, ln));
        }
        else
        {
            ctx.AddToken(new Token(_identifierTokenType, tokenPos, ln));
        }

        return true;
    }
    
    private static bool IsIdentifierLetterChar(char c)
    {
        var charCode = (int)c;

        // (see ascii_chars.txt)
        return c == '$' || // $ sign
               c == '_' || // _ sign
               (charCode >= 'A' && charCode <= 'Z') || // uppercase letters 
               (charCode >= 'a' && charCode <= 'z'); //lower case letters
    }
}