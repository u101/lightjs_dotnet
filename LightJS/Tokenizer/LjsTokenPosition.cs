namespace LightJS.Tokenizer;

public readonly struct LjsTokenPosition
{
    public int CharIndex { get; }
    public int Line { get; }
    public int Column { get; }

    public LjsTokenPosition(int charIndex, int line, int column)
    {
        CharIndex = charIndex;
        Line = line;
        Column = column;
    }
    
}