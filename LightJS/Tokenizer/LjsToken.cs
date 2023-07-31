namespace LightJS.Tokenizer;

public readonly struct LjsToken
{
    
    public LjsTokenType TokenType { get; }
    public int StringLength { get; }
    public LjsTokenPosition Position { get; }

    /// <summary>
    /// LjsToken contructor
    /// </summary>
    /// <param name="tokenType"> type of this token </param>
    /// /// <param name="position">token position in source code</param>
    /// <param name="stringLength">length of string of this token from source code string</param>
    public LjsToken(LjsTokenType tokenType, LjsTokenPosition position, int stringLength )
    {
        TokenType = tokenType;
        StringLength = stringLength;
        Position = position;
    }
    
}