namespace App16.ALang.Tokenizers;

public readonly struct Token : IEquatable<Token>
{
    public int TokenType { get; }
    public int StringLength { get; }
    public TokenPosition Position { get; }

    public Token(
        int tokenType,
        TokenPosition position, 
        int stringLength )
    {
        TokenType = tokenType;
        StringLength = stringLength;
        Position = position;
    }

    public bool Equals(Token other)
    {
        return TokenType == other.TokenType && 
               StringLength == other.StringLength && 
               Position.Equals(other.Position);
    }

    public override bool Equals(object? obj)
    {
        return obj is Token other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TokenType, StringLength, Position);
    }

    public static bool operator ==(Token left, Token right) => left.Equals(right);

    public static bool operator !=(Token left, Token right) => !(left == right);
    
}