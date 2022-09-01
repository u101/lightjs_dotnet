namespace LightJS;

public readonly struct LjsToken
{
    public static readonly LjsToken Null = new LjsToken();
    
    public LjsTokenType TokenType { get; }
    public int StringStartIndex { get; }
    public int StringLength { get; }

    /// <summary>
    /// LjsToken contructor
    /// </summary>
    /// <param name="tokenType"> type of this token </param>
    /// <param name="stringStartIndex">starting char index of this token from source code string (inclusive)</param>
    /// <param name="stringLength">length of string of this token from source code string</param>
    public LjsToken(LjsTokenType tokenType, int stringStartIndex, int stringLength)
    {
        TokenType = tokenType;
        StringStartIndex = stringStartIndex;
        StringLength = stringLength;
    }
    
}

public enum LjsTokenType
{
    /// <summary>
    /// Non existing token type
    /// </summary>
    Null,
    Word,
    Int,
    Float,
    String,
    Operator,
    BraceOpen,
    BraceClose,
    BracketOpen,
    BracketClose,
    SquareBracketOpen,
    SquareBracketClose
}