namespace LightJS;

public class LjsTokenizer
{
    private readonly ILjsReader _reader;
    private readonly List<LjsToken> _tokens = new List<LjsToken>();

    private int _line = 0;
    private int _col = 0;

    public LjsTokenizer(ILjsReader reader)
    {
        _reader = reader;
    }

    public List<LjsToken> ReadTokens()
    {

        var line = 0;
        var col = 0;

        Console.Write("line 0:");

        while (_reader.HasNextChar())
        {
            var c = _reader.ReadNextChar();

            if (c == '\r') continue;
            
            Console.Write(c);
            
            if (c == '\n')
            {
                ++line;
                col = 0;
                Console.Write($"line {line}:");
            }
            else
            {
                // Console.Write($"[{col}]");
                ++col;
            }
        }
        
        return _tokens;
    }

    

}