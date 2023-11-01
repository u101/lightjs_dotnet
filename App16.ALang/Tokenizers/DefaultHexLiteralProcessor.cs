namespace App16.ALang.Tokenizers;

/// <summary>
/// Process hex int literal like 0xff 
/// </summary>
public class DefaultHexLiteralProcessor : ITokensProcessor
{
    private readonly int _hexIntegerTokenType;
    private readonly HashSet<char> _operatorSymbolsSet;

    public DefaultHexLiteralProcessor(
        int hexIntegerTokenType,
        HashSet<char> operatorSymbolsSet)
    {
        _hexIntegerTokenType = hexIntegerTokenType;
        _operatorSymbolsSet = operatorSymbolsSet;
    }
    
    public bool Process(TokenizerContext ctx)
    {
        var reader = ctx.CharsReader;
        var c = reader.CurrentChar;

        if (c != '0' || !reader.HasNextChar || reader.NextChar != 'x') return false;
        
        var startIndex = reader.CurrentIndex;
        var tokenPos = reader.CurrentTokenPosition;

        reader.MoveForward(); // skip char x

        if (!reader.HasNextChar || !IsHexChar(reader.NextChar))
        {
            reader.ThrowInvalidNumberFormatError();
        }

        while (reader.HasNextChar && 
               !TokenizerUtils.IsEmptySpaceChar(reader.NextChar) && 
               !IsOperator(reader.NextChar))
        {
            reader.MoveForward();

            if (!IsHexChar(reader.CurrentChar))
            {
                reader.ThrowInvalidNumberFormatError();
            }
        }
                
        var ln = (reader.CurrentIndex + 1) - startIndex;
                
        ctx.AddToken(new Token(_hexIntegerTokenType, tokenPos, ln));

        return true;
    }
    
    private static bool IsHexChar(char c)
    {
        var charCode = (int)c;
        
        return 
            // 0-9
            (charCode >= '0' && charCode <= '9') ||
            // A-F
            (charCode >= 'A' && charCode <= 'F') ||
            // a-f
            (charCode >= 'a' && charCode <= 'f');
    }

    private bool IsOperator(char c) => _operatorSymbolsSet.Contains(c);
}