namespace LightJS.Tokenizer;

public readonly struct LjsToken
{
    public static readonly LjsToken Null = new LjsToken();
    
    public LjsTokenType TokenType { get; }
    public int StringStartIndex { get; }
    public int StringLength { get; }
    public int LineIndex { get; }

    /// <summary>
    /// LjsToken contructor
    /// </summary>
    /// <param name="tokenType"> type of this token </param>
    /// <param name="stringStartIndex">starting char index of this token from source code string (inclusive)</param>
    /// <param name="stringLength">length of string of this token from source code string</param>
    /// <param name="lineIndex">token line index in source code</param>
    public LjsToken(LjsTokenType tokenType, int stringStartIndex, int stringLength, int lineIndex)
    {
        TokenType = tokenType;
        StringStartIndex = stringStartIndex;
        StringLength = stringLength;
        LineIndex = lineIndex;
    }
    
}