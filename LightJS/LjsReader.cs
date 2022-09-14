namespace LightJS;

public class LjsReader 
{
    private readonly string _sourceCode;
    private readonly int _sourceCodeLn;

    private int _currentIndex = -1;
    
    public LjsReader(string sourceCode)
    {
        if (string.IsNullOrEmpty(sourceCode))
        {
            throw new ArgumentException("input string is null or empty");
        }
        
        _sourceCode = sourceCode;
        _sourceCodeLn = sourceCode.Length;
    }

    public int CurrentIndex => _currentIndex;

    public char CurrentChar => 
        _currentIndex != -1 ? _sourceCode[_currentIndex] : (char) 0;

    public char NextChar => 
        _currentIndex + 1 < _sourceCodeLn ? _sourceCode[_currentIndex + 1] : (char)0;

    public char PrevChar => 
        _currentIndex > 0 ? _sourceCode[_currentIndex - 1] : (char)0;

    public bool HasNextChar => _currentIndex + 1 < _sourceCodeLn;

    public string GetCodeString(int startIndex, int length)
    {
        if (startIndex < 0 || startIndex >= _sourceCodeLn)
            throw new ArgumentException($"invalid start index {startIndex}");
        
        if (length <= 0)
            throw new ArgumentException($"invalid length {length}");

        if (startIndex + length > _sourceCodeLn)
            throw new ArgumentException(
                $"invalid length {length} with start index {startIndex} > code ln {_sourceCodeLn}");
        
        return _sourceCode.Substring(startIndex, length);
    }

    public void MoveForward()
    {
        if (!HasNextChar)
        {
            throw new Exception("str end");
        }
        
        ++_currentIndex;
    }
    
    
    
}