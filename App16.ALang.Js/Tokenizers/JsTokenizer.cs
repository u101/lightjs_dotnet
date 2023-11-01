using App16.ALang.Js.Errors;
using App16.ALang.Tokenizers;

namespace App16.ALang.Js.Tokenizers;

public sealed class JsTokenizer
{
    
    private readonly Tokenizer _tokenizer;

    public JsTokenizer(Tokenizer tokenizer)
    {
        _tokenizer = tokenizer;
    }

    
    /// <summary>
    /// Convert source code into Tokens list 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="JsSyntaxError"></exception>
    public List<Token> ReadTokens()
    {
        try
        {
            var tokens = _tokenizer.ReadTokens();
            return tokens;
        }
        catch (TokenizerError e)
        {
            throw new JsSyntaxError(e.ErrorMessage, e.TokenPosition);
        }
    }
    

    
}