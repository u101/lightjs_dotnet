using static App16.ALang.Tokenizers.TokenizerUtils;

namespace App16.ALang.Tokenizers;

public sealed class Tokenizer
{
    private readonly string _sourceCodeString;
    
    private readonly CharsReader _reader;

    private readonly IReadOnlyList<ITokensProcessor> _processors;

    public Tokenizer(string sourceCodeString, IReadOnlyList<ITokensProcessor> processors)
    {
        if (string.IsNullOrEmpty(sourceCodeString))
        {
            throw new ArgumentException("input string is null or empty");
        }
        
        _sourceCodeString = sourceCodeString;
        _reader = new CharsReader(_sourceCodeString);
        _processors = processors;
    }
    
    public List<Token> ReadTokens()
    {
        var ctx = new TokenizerContext(_reader, _sourceCodeString);
        
        while (_reader.HasNextChar)
        {
            _reader.MoveForward();
            
            var c = _reader.CurrentChar;

            // check if we have space, line break, tabulation at this point
            if (IsEmptySpaceChar(c))
            {
                // just skip it
                continue;
            }

            var processed = false;
            
            for (var i = 0; i < _processors.Count; ++i)
            {
                var p = _processors[i];

                if (p.Process(ctx))
                {
                    processed = true;
                    break;
                }
                
            }

            if (!processed)
            {
                throw new TokenizerError(
                    "unknown token", _reader.CurrentTokenPosition);
            }
            
        }
        
        return ctx.Tokens;
    }
    
    

}