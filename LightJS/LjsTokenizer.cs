namespace LightJS;

public class LjsTokenizer
{
    private readonly LjsReader _reader;
    private readonly List<LjsToken> _tokens = new List<LjsToken>();
    
    private const char DoubleQuotes = '"';
    private const char SingleQuotes = '\'';
    private const char NewLine = '\n';
    private const char Slash = '/';
    private const char Asterisk = '*';
    private const char Dot = '.';

    public LjsTokenizer(LjsReader reader)
    {
        _reader = reader;
    }

    public List<LjsToken> ReadTokens()
    {
        ReadMain();
        
        return _tokens;
    }

    private void ReadMain()
    {
        // start reading source code text
        while (_reader.HasNextChar)
        {
            _reader.MoveForward();
            
            var c = _reader.CurrentChar;

            // check if we have space, line break, tabulation at this point
            if (IsSpaceOrSpecialAnsiiChar(c))
            {
                // just skip it
                continue;
            }

            // if we have slash - it is either a single line or multiline comment
            if (c == Slash && _reader.HasNextChar && 
                (_reader.NextChar == Slash || _reader.NextChar == Asterisk))
            {
                var nextChar = _reader.NextChar;

                if (nextChar == Slash)
                {
                    // single line comment
                    while (_reader.NextChar != '\n')
                    {
                        _reader.MoveForward();
                        // check if we reached the end of file
                        if (!_reader.HasNextChar) return;
                    }
                }
                else if (nextChar == Asterisk)
                {
                    _reader.MoveForward(); // current char is asterisk here
                    _reader.MoveForward();

                    while (_reader.HasNextChar)
                    {
                        _reader.MoveForward();
                        
                        if (_reader.CurrentChar == Slash && _reader.PrevChar == Asterisk)
                        {
                            //the end of multiline comment
                            break;
                        }
                    }
                }
            }
            // check if this is start of a string
            else if (c == DoubleQuotes || c == SingleQuotes)
            {
                // todo process escape characters (quotes escape, line breaks, etc..)
                
                var startIndex = _reader.CurrentIndex + 1; // we ignore quotes (+1)
                
                _reader.MoveForward();
                
                while (_reader.CurrentChar != c)
                {
                    if (!_reader.HasNextChar)
                    {
                        throw new LjsTokenizerError(_reader.CurrentIndex);
                    }
                    
                    _reader.MoveForward();
                }

                var ln = _reader.CurrentIndex - startIndex; // end index is exclusive
                
                AddToken(new LjsToken(
                    LjsTokenType.String, startIndex, ln));
            }
            // dot operator
            else if (c == Dot)
            {
                // here we need to check if dot here is legal
                var prevToken = _tokens.Count > 0 ? 
                    _tokens[_tokens.Count - 1] : LjsToken.Null;
                
                var prevTokenType = prevToken.TokenType;
                var prevTokenIsValid = prevTokenType == LjsTokenType.Word ||
                                       prevTokenType == LjsTokenType.String ||
                                       prevTokenType == LjsTokenType.BracketClose ||
                                       prevTokenType == LjsTokenType.SquareBracketClose;

                if (!prevTokenIsValid)
                {
                    throw new LjsTokenizerError(_reader.CurrentIndex);
                }
                
                AddToken(new LjsToken(
                    LjsTokenType.Dot, _reader.CurrentIndex, 1));

            }
            // key word or identifier
            else if (IsLetterChar(c))
            {
                var startIndex = _reader.CurrentIndex;

                while (_reader.HasNextChar && 
                       (IsLetterChar(_reader.NextChar) || IsNumberChar(_reader.NextChar)))
                {
                    _reader.MoveForward();
                }

                var ln = (_reader.CurrentIndex + 1) - startIndex;
                
                AddToken(new LjsToken(
                    LjsTokenType.Word, startIndex, ln));
            }
            // number
            else if (IsNumberChar(c))
            {
                var startIndex = _reader.CurrentIndex;
                var isFloat = false;
                
                while (_reader.HasNextChar && 
                       (IsNumberChar(_reader.NextChar) || (!isFloat && _reader.NextChar == Dot)))
                {
                    isFloat = isFloat || _reader.NextChar == Dot;
                    _reader.MoveForward();
                }
                
                var ln = (_reader.CurrentIndex + 1) - startIndex;

                AddToken(new LjsToken(
                    isFloat ? LjsTokenType.Float : LjsTokenType.Int, startIndex, ln));
            }
            else if (IsOperator(c))
            {
                AddToken(new LjsToken(LjsTokenType.Operator, _reader.CurrentIndex, 1));
            }
            else if (c == '(')
            {
                AddToken(new LjsToken(LjsTokenType.BracketOpen, _reader.CurrentIndex, 1));
            }
            else if (c == ')')
            {
                AddToken(new LjsToken(LjsTokenType.BracketClose, _reader.CurrentIndex, 1));
            }
            else if (c == '{')
            {
                AddToken(new LjsToken(LjsTokenType.BraceOpen, _reader.CurrentIndex, 1));
            }
            else if (c == '}')
            {
                AddToken(new LjsToken(LjsTokenType.BraceClose, _reader.CurrentIndex, 1));
            }
            else if (c == '[')
            {
                AddToken(new LjsToken(LjsTokenType.SquareBracketOpen, _reader.CurrentIndex, 1));
            }
            else if (c == ']')
            {
                AddToken(new LjsToken(LjsTokenType.SquareBracketClose, _reader.CurrentIndex, 1));
            }
            

        }
    }

    private void AddToken(LjsToken token)
    {
        Console.WriteLine($"token {token.TokenType} |{_reader.GetCodeString(token.StringStartIndex, token.StringLength)}|");
        
        _tokens.Add(token);
    }

    private static bool IsOperator(char c)
    {
        return c == '>' ||
               c == '<' ||
               c == '=' ||
               c == '+' ||
               c == '-' ||
               c == '*' ||
               c == '/' ||
               c == '&' ||
               c == '|' ||
               c == '!' ||
               c == ',' ||
               c == '.' ||
               c == ':' ||
               c == ';';
    }

    /// <summary>
    /// check if char is space, newline, tabulation or special ansii code at the beginning of ansii table
    /// </summary>
    private static bool IsSpaceOrSpecialAnsiiChar(char c)
    {
        var charCode = (int)c;
        
        return charCode <= 32;
    }

    private static bool IsLetterChar(char c)
    {
        var charCode = (int)c;

        // (see ascii_chars.txt)
        return charCode == 36 || // $ sign
               (charCode >= 65 && charCode <= 90) || // uppercase letters 
               (charCode >= 97 && charCode <= 122); //lower case letters
    }

    private static bool IsNumberChar(char c)
    {
        var charCode = (int)c;
        return charCode >= 48 && charCode <= 57;
    }

}