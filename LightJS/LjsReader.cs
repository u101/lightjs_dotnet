namespace LightJS;

public class LjsReader : ILjsReader
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

    public char ReadNextChar()
    {
        if (!HasNextChar())
        {
            throw new Exception("str end");
        }

        ++_currentIndex;
        return _sourceCode[_currentIndex];
    }

    public bool HasNextChar()
    {
        return _currentIndex + 1 < _sourceCodeLn;
    }
    
    
    
}