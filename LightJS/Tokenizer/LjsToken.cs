namespace LightJS.Tokenizer;

public readonly struct LjsToken
{
    
    public LjsTokenClass TokenClass { get; }
    public LjsTokenType TokenType { get; }
    public int StringLength { get; }
    public LjsTokenPosition Position { get; }

    /// <summary>
    /// LjsToken contructor
    /// </summary>
    /// <param name="tokenClass"> class of this token </param>
    /// <param name="tokenType"> type of this token </param>
    /// /// <param name="position">token position in source code</param>
    /// <param name="stringLength">length of string of this token from source code string</param>
    public LjsToken(
        LjsTokenClass tokenClass, 
        LjsTokenType tokenType,
        LjsTokenPosition position, int stringLength )
    {
        TokenClass = tokenClass;
        TokenType = tokenType;
        StringLength = stringLength;
        Position = position;
    }
    
}