using System.Globalization;

namespace LightJS.Tokenizer;

public static class LjsTokenizerUtils
{
    
    private static void ThrowIfOutOfRange(string sourceCodeString, int startIndex, int length)
    {
        if (startIndex < 0 || startIndex >= sourceCodeString.Length)
            throw new ArgumentException($"invalid start index {startIndex}");
        
        if (length < 0)
            throw new ArgumentException($"invalid length {length}");

        if (startIndex + length > sourceCodeString.Length)
            throw new ArgumentException(
                $"invalid length {length} with start index {startIndex} > code ln {sourceCodeString.Length}");
    }

    public static string GetTokenStringValue(string sourceCodeString, LjsToken token)
    {
        var length = token.StringLength;
        var startIndex = token.Position.CharIndex;
        
        ThrowIfOutOfRange(sourceCodeString, startIndex, length);
        
        return sourceCodeString.Substring(startIndex, length);
    }
    
    public static int GetTokenIntValue(string sourceCodeString, LjsToken token)
    {
        var length = token.StringLength;
        var startIndex = token.Position.CharIndex;
        
        ThrowIfOutOfRange(sourceCodeString, startIndex, length);

        switch (token.TokenType)
        {
            case LjsTokenType.IntDecimal:
                return int.Parse(
                    sourceCodeString.AsSpan(startIndex, length), 
                    NumberStyles.None, NumberFormatInfo.InvariantInfo);
            
            case LjsTokenType.IntHex:
                return int.Parse(
                    sourceCodeString.AsSpan(startIndex + 2, length - 2), // skip leading 0x 
                    NumberStyles.AllowHexSpecifier, NumberFormatInfo.InvariantInfo);
            
            case LjsTokenType.IntBinary:
                return Convert.ToInt32(
                    sourceCodeString.Substring(startIndex + 2, length - 2), 2); // skip leading 0b
            
            default:
                throw new IndexOutOfRangeException($"invalid int token type {token.TokenType}");
        }
    }

    public static double GetTokenFloatValue(string sourceCodeString, LjsToken token)
    {
        var length = token.StringLength;
        var startIndex = token.Position.CharIndex;
        
        ThrowIfOutOfRange(sourceCodeString, startIndex, length);

        return double.Parse(
            sourceCodeString.AsSpan(startIndex, length), 
            NumberStyles.Float, NumberFormatInfo.InvariantInfo);
    }
    
}