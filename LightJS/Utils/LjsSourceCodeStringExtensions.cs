using System.Globalization;

namespace LightJS.Utils;

public static class LjsSourceCodeStringExtensions
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
    
    public static int ReadInt(this string sourceCodeString, int startIndex, int length)
    {
        ThrowIfOutOfRange(sourceCodeString, startIndex, length);

        var substring = sourceCodeString.Substring(startIndex, length);

        return int.Parse(substring, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
    }

    public static double ReadDouble(this string sourceCodeString, int startIndex, int length)
    {
        ThrowIfOutOfRange(sourceCodeString, startIndex, length);

        var substring = sourceCodeString.Substring(startIndex, length);

        return double.Parse(substring, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
    }
    
}