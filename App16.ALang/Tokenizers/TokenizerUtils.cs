namespace App16.ALang.Tokenizers;

public static class TokenizerUtils
{
    /// <summary>
    /// check if char is space, newline, tabulation or special ansii code at the beginning of ansii table
    /// </summary>
    public static bool IsEmptySpaceChar(char c)
    {
        var charCode = (int)c;
        
        return charCode <= 32;
    }
    
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

    public static string GetTokenStringValue(string sourceCodeString, Token token)
    {
        var length = token.StringLength;
        var startIndex = token.Position.CharIndex;
        
        ThrowIfOutOfRange(sourceCodeString, startIndex, length);
        
        return sourceCodeString.Substring(startIndex, length);
    }
    
}