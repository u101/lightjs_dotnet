namespace App16.ALang.Tokenizers;

/// <summary>
/// Process one line comments starting with // and multiline comments starting with /* */  
/// </summary>
/// <typeparam name="TTokenType"></typeparam>
public class DefaultCommentsProcessor : ITokensProcessor
{
    private const char Slash = '/';
    private const char Asterisk = '*';
    
    public bool Process(TokenizerContext ctx)
    {
        var reader = ctx.CharsReader;
        var c = reader.CurrentChar;

        if (c != Slash || !reader.HasNextChar || (reader.NextChar != Slash && reader.NextChar != Asterisk))
            return false;
        
        var nextChar = reader.NextChar;

        if (nextChar == Slash)
        {
            // single line comment
            while (reader.NextChar != '\n')
            {
                reader.MoveForward();
                // check if we reached the end of file
                if (!reader.HasNextChar) return true;
            }
        }
        else if (nextChar == Asterisk)
        {
            reader.MoveForward(); // current char is asterisk here
            reader.MoveForward();

            while (reader.HasNextChar)
            {
                reader.MoveForward();
                        
                if (reader.CurrentChar == Slash && reader.PrevChar == Asterisk)
                {
                    //the end of multiline comment
                    break;
                }
            }
        }

        return true;

    }
}