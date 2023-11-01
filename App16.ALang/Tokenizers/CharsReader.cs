namespace App16.ALang.Tokenizers;

internal sealed class CharsReader : ICharsReader
{
    private readonly string _sourceCodeString;

    private int _currentIndex = -1;

    public int CurrentLine { get; private set; }  = 0;
    public int CurrentCol { get; private set; } = -1;
    
    public CharsReader(string sourceCodeString)
    {
        _sourceCodeString = sourceCodeString;
    }

    public int CurrentIndex => _currentIndex;

    public char CurrentChar => 
        _currentIndex != -1 ? _sourceCodeString[_currentIndex] : (char) 0;

    public char NextChar => 
        _currentIndex + 1 < _sourceCodeString.Length ? _sourceCodeString[_currentIndex + 1] : (char)0;

    public char PrevChar => 
        _currentIndex > 0 ? _sourceCodeString[_currentIndex - 1] : (char)0;

    public bool HasNextChar => _currentIndex + 1 < _sourceCodeString.Length;

    public bool CanLookForward(int offset)
    {
        if (offset <= 0)
            throw new ArgumentException("offset <= 0");
        return _currentIndex + offset < _sourceCodeString.Length;
    }

    public char LookForward(int offset) => _sourceCodeString[_currentIndex + offset];

    public TokenPosition CurrentTokenPosition => new(_currentIndex, CurrentLine, CurrentCol);

    public void MoveForward()
    {
        if (!HasNextChar)
        {
            throw new Exception("str end");
        }
        
        ++_currentIndex;
        
        var c = CurrentChar;

        if (c == '\n')
        {
            ++CurrentLine;
            CurrentCol = -1;
        }
        else
        {
            ++CurrentCol;
        }
    }
}