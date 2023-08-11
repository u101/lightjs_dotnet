namespace LightJS.Outsource;

public static class MatherTokensParser
{
    public static List<MatherToken> Parse(string exp)
    {
        var strings = exp.Split(' ');
        return strings.Select(GetMatherToken).ToList();
    }

    private static readonly Dictionary<char, MatherTokenType> _ops = new()
    {
        {'+', MatherTokenType.OpPlus},
        {'-', MatherTokenType.OpMinus},
        {'*', MatherTokenType.OpMul},
        {'/', MatherTokenType.OpDiv},
        {'=', MatherTokenType.OpAssign},
        {'(', MatherTokenType.OpParenthesesOpen},
        {')', MatherTokenType.OpParenthesesClose},
    };

    private static MatherToken GetMatherToken(string s)
    {
        if (string.IsNullOrEmpty(s)) 
            throw new ArgumentException("s is null or empty");

        var c = s[0];
        var tokenType = MatherTokenType.Id;
        
        if (char.IsDigit(c))
        {
            tokenType = MatherTokenType.Literal;
        }
        else if (_ops.ContainsKey(c))
        {
            tokenType = _ops[c];
        }
        
        
        return new MatherToken(s, tokenType);
    }
}