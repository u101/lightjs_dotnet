namespace LightJS.Tokenizer;

public readonly struct LjsToken
{
    public static readonly LjsToken Null = new LjsToken();
    
    public LjsTokenType TokenType { get; }
    public int StringLength { get; }
    public LjsTokenPosition TokenPosition { get; }

    /// <summary>
    /// LjsToken contructor
    /// </summary>
    /// <param name="tokenType"> type of this token </param>
    /// /// <param name="tokenPosition">token position in source code</param>
    /// <param name="stringLength">length of string of this token from source code string</param>
    public LjsToken(LjsTokenType tokenType, LjsTokenPosition tokenPosition, int stringLength )
    {
        TokenType = tokenType;
        StringLength = stringLength;
        TokenPosition = tokenPosition;
    }
    
}