namespace LightJS;

public class LjsReader 
{
    private readonly LjsSourceCode _sourceCode;

    private int _currentIndex = -1;
    
    public LjsReader(LjsSourceCode sourceCode)
    {
        _sourceCode = sourceCode;
    }

    public int CurrentIndex => _currentIndex;

    public char CurrentChar => 
        _currentIndex != -1 ? _sourceCode[_currentIndex] : (char) 0;

    public char NextChar => 
        _currentIndex + 1 < _sourceCode.Length ? _sourceCode[_currentIndex + 1] : (char)0;

    public char PrevChar => 
        _currentIndex > 0 ? _sourceCode[_currentIndex - 1] : (char)0;

    public bool HasNextChar => _currentIndex + 1 < _sourceCode.Length;

    public void MoveForward()
    {
        if (!HasNextChar)
        {
            throw new Exception("str end");
        }
        
        ++_currentIndex;
    }
    
    
    
}