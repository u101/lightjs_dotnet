namespace LightJS;

public class LjsSourceCode
{
    private readonly string _sourceCode;

    public int Length { get; }
    
    public LjsSourceCode(string sourceCodeString)
    {
        if (string.IsNullOrEmpty(sourceCodeString))
        {
            throw new ArgumentException("input string is null or empty");
        }
        _sourceCode = sourceCodeString;
        Length = _sourceCode.Length;
    }

    public char this[int index] => _sourceCode[index];

    public string Substring(int startIndex, int length)
    {
        if (startIndex < 0 || startIndex >= Length)
            throw new ArgumentException($"invalid start index {startIndex}");
        
        if (length <= 0)
            throw new ArgumentException($"invalid length {length}");

        if (startIndex + length > Length)
            throw new ArgumentException(
                $"invalid length {length} with start index {startIndex} > code ln {Length}");
        
        return _sourceCode.Substring(startIndex, length);
    }
    
    
}