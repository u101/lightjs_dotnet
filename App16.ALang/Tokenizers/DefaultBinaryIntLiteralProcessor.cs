namespace App16.ALang.Tokenizers;

/// <summary>
/// Process binary int literal like 0b0101 
/// </summary>
public class DefaultBinaryIntLiteralProcessor : ITokensProcessor
{
    private readonly int _binaryIntegerTokenType;
    private readonly HashSet<char> _operatorSymbolsSet;

    public DefaultBinaryIntLiteralProcessor(
        int binaryIntegerTokenType,
        HashSet<char> operatorSymbolsSet)
    {
        _binaryIntegerTokenType = binaryIntegerTokenType;
        _operatorSymbolsSet = operatorSymbolsSet;
    }
    
    public bool Process(TokenizerContext ctx)
    {
        var reader = ctx.CharsReader;
        var c = reader.CurrentChar;

        if (c != '0' || !reader.HasNextChar || reader.NextChar != 'b') return false;
        
        var startIndex = reader.CurrentIndex;
        var tokenPos = reader.CurrentTokenPosition;

        reader.MoveForward(); // skip char b

        if (!reader.HasNextChar || !IsBinaryDigitChar(reader.NextChar))
        {
            reader.ThrowInvalidNumberFormatError();
        }

        while (reader.HasNextChar && 
               !TokenizerUtils.IsEmptySpaceChar(reader.NextChar) && 
               !IsOperator(reader.NextChar))
        {
            reader.MoveForward();

            if (!IsBinaryDigitChar(reader.CurrentChar))
            {
                reader.ThrowInvalidNumberFormatError();
            }
        }
                
        var ln = (reader.CurrentIndex + 1) - startIndex;
                
        ctx.AddToken(new Token(
            _binaryIntegerTokenType, tokenPos, ln));
        
        return true;
    }
    
    private bool IsOperator(char c) => _operatorSymbolsSet.Contains(c);
    
    private static bool IsBinaryDigitChar(char c)
    {
        return c == '0' || c == '1';
    }
}