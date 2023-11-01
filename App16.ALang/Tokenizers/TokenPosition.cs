namespace App16.ALang.Tokenizers;

public readonly struct TokenPosition : IEquatable<TokenPosition>
{
    public int CharIndex { get; }
    public int Line { get; }
    public int Column { get; }

    public TokenPosition(int charIndex, int line, int column)
    {
        CharIndex = charIndex;
        Line = line;
        Column = column;
    }

    public bool IsAdjacentTo(TokenPosition p, int distance = 1)
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

    public bool Equals(TokenPosition other)
    {
        return CharIndex == other.CharIndex && Line == other.Line && Column == other.Column;
    }

    public override bool Equals(object? obj)
    {
        return obj is TokenPosition other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CharIndex, Line, Column);
    }

    public override string ToString()
    {
        return $"{nameof(CharIndex)}: {CharIndex}, {nameof(Line)}: {Line}, {nameof(Column)}: {Column}";
    }

    public static bool operator ==(TokenPosition left, TokenPosition right) => left.Equals(right);

    public static bool operator !=(TokenPosition left, TokenPosition right) => !(left == right);
    
}