namespace LightJS.Outsource;

public class MatherToken
{
    public string Value { get; }
    public MatherTokenType TokenType { get; }

    public MatherToken(string value, MatherTokenType tokenType)
    {
        Value = value;
        TokenType = tokenType;
    }
}