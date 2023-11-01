namespace App16.ALang.Tokenizers;

public class DefaultOperatorsProcessor : ITokensProcessor
{
    private readonly Dictionary<char, int> _operatorsMap;

    public DefaultOperatorsProcessor(Dictionary<char, int> operatorsMap)
    {
        _operatorsMap = operatorsMap;
    }
    
    public bool Process(TokenizerContext ctx)
    {
        var reader = ctx.CharsReader;
        var c = reader.CurrentChar;

        if (!_operatorsMap.TryGetValue(c, out var opType)) return false;
        
        ctx.AddToken(new Token(opType, reader.CurrentTokenPosition, 1));
        
        return true;
    }
}