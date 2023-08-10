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

    public bool IsAdjacentTo(LjsTokenPosition p, int distance = 1)
    {
        var (_, pLine, pCol) = p;

        if (pLine != Line) return false;

        var maxCol = Column > pCol ? Column : pCol;
        var minCol = Column < pCol ? Column : pCol;

        return maxCol - minCol == distance;
    }

    public void Deconstruct(out int charIndex, out int line, out int col)
    {
        charIndex = CharIndex;
        line = Line;
        col = Column;
    }

    public override string ToString()
    {
        return $"{nameof(CharIndex)}: {CharIndex}, {nameof(Line)}: {Line}, {nameof(Column)}: {Column}";
    }
}