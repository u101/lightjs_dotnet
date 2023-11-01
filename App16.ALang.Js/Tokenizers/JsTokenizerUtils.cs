using System.Globalization;
using App16.ALang.Tokenizers;

namespace App16.ALang.Js.Tokenizers;

public static class JsTokenizerUtils
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

    public static int GetTokenDecimalIntValue(string sourceCodeString, Token token)
    {
        var length = token.StringLength;
        var startIndex = token.Position.CharIndex;
        
        ThrowIfOutOfRange(sourceCodeString, startIndex, length);
        
        return int.Parse(
            sourceCodeString.AsSpan(startIndex, length), 
            NumberStyles.None, NumberFormatInfo.InvariantInfo);
    }
    
    public static int GetTokenHexIntValue(string sourceCodeString, Token token)
    {
        var length = token.StringLength;
        var startIndex = token.Position.CharIndex;
        
        ThrowIfOutOfRange(sourceCodeString, startIndex, length);
        
        return int.Parse(
            sourceCodeString.AsSpan(startIndex + 2, length - 2), // skip leading 0x 
            NumberStyles.AllowHexSpecifier, NumberFormatInfo.InvariantInfo);
    }
    
    public static int GetTokenBinaryIntValue(string sourceCodeString, Token token)
    {
        var length = token.StringLength;
        var startIndex = token.Position.CharIndex;
        ThrowIfOutOfRange(sourceCodeString, startIndex, length);
        
        return Convert.ToInt32(
            sourceCodeString.Substring(startIndex + 2, length - 2), 2); // skip leading 0b
    }

    public static double GetTokenDoubleValue(string sourceCodeString, Token token)
    {
        var length = token.StringLength;
        var startIndex = token.Position.CharIndex;
        
        ThrowIfOutOfRange(sourceCodeString, startIndex, length);

        return double.Parse(
            sourceCodeString.AsSpan(startIndex, length), 
            NumberStyles.Float, NumberFormatInfo.InvariantInfo);
    }
    
}