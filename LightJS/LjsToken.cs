namespace LightJS;

public readonly struct LjsToken
{
    public LjsTokenType TokenType { get; }
    public int Line { get; }
    public int Col { get; }
    public int StringStartIndex { get; }
    public int StringEndIndex { get; }

    public LjsToken(LjsTokenType tokenType, int line, int col, int stringStartIndex, int stringEndIndex)
    {
        TokenType = tokenType;
        Line = line;
        Col = col;
        StringStartIndex = stringStartIndex;
        StringEndIndex = stringEndIndex;
    }
    
}

public enum LjsTokenType
{
    Word,
    Int,
    Float,
    String,
    Dot,
    Comma,
    OperatorPlus,
    OperatorMinus,
    OperatorEqual,
    BraceOpen,
    BraceClose,
    BracketOpen,
    BracketClose,
    SquareBracketOpen,
    SquareBracketClose
}