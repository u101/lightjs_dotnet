namespace LightJS;

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