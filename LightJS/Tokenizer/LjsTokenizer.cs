using System.Diagnostics.CodeAnalysis;

namespace LightJS.Tokenizer;

public class LjsTokenizer
{
    private readonly LjsSourceCode _sourceCode;
    
    [AllowNull] private SourceCodeCharsReader _reader;
    [AllowNull] private List<LjsToken> _tokens;
    
    private const char DoubleQuotes = '"';
    private const char SingleQuotes = '\'';
    private const char NewLine = '\n';
    private const char Slash = '/';
    private const char Asterisk = '*';
    private const char Dot = '.';

    private int _currentLine;
    private int _currentCol;
    
    public LjsTokenizer(LjsSourceCode sourceCode)
    {
        _sourceCode = sourceCode;
    }

    public List<LjsToken> ReadTokens()
    {
        _currentCol = 0;
        _currentLine = 0;
        
        _reader = new SourceCodeCharsReader(_sourceCode);
        _tokens = new List<LjsToken>();
        
        ReadMain();
        
        return _tokens;
    }

    private void ReadNextChar()
    {
        _reader.MoveForward();

        var c = _reader.CurrentChar;

        if (c == '\n')
        {
            ++_currentLine;
            _currentCol = 0;
        }
        else
        {
            ++_currentCol;
        }
    }

    private void ReadMain()
    {
        var lineIndex = 0;
        
        while (_reader.HasNextChar)
        {
            ReadNextChar();
            
            var c = _reader.CurrentChar;

            // check if we have space, line break, tabulation at this point
            if (IsEmptySpace(c))
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
                        ReadNextChar();
                        // check if we reached the end of file
                        if (!_reader.HasNextChar) return;
                    }
                }
                else if (nextChar == Asterisk)
                {
                    ReadNextChar(); // current char is asterisk here
                    ReadNextChar();

                    while (_reader.HasNextChar)
                    {
                        ReadNextChar();
                        
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

                lineIndex = _currentLine; 
                
                ReadNextChar();
                
                while (_reader.CurrentChar != c)
                {
                    if (!_reader.HasNextChar || _reader.NextChar == NewLine)
                    {
                        throw new LjsTokenizerError(
                            "unterminated string literal", 
                            new LjsTokenPosition(_reader.CurrentIndex, _currentLine, _currentCol));
                    }
                    
                    ReadNextChar();
                }

                var ln = _reader.CurrentIndex - startIndex; // end index is exclusive
                
                AddToken(new LjsToken(
                    LjsTokenType.String, startIndex, ln, lineIndex));
            }
            // key word or identifier
            else if (IsLetterChar(c))
            {
                var startIndex = _reader.CurrentIndex;

                lineIndex = _currentLine;

                while (_reader.HasNextChar && 
                       (IsLetterChar(_reader.NextChar) || IsNumberChar(_reader.NextChar)))
                {
                    ReadNextChar();
                }

                var ln = (_reader.CurrentIndex + 1) - startIndex;
                
                AddToken(new LjsToken(
                    LjsTokenType.Word, startIndex, ln, lineIndex));
            }
            // number
            else if (IsNumberChar(c))
            {
                var startIndex = _reader.CurrentIndex;
                var isFloat = false;

                lineIndex = _currentLine;
                
                while (_reader.HasNextChar && 
                       (IsNumberChar(_reader.NextChar) || (!isFloat && _reader.NextChar == Dot)))
                {
                    isFloat = isFloat || _reader.NextChar == Dot;
                    ReadNextChar();
                }
                
                var ln = (_reader.CurrentIndex + 1) - startIndex;

                AddToken(new LjsToken(
                    isFloat ? LjsTokenType.Float : LjsTokenType.Int, startIndex, ln, lineIndex));
            }
            else if (IsOperator(c))
            {
                AddToken(new LjsToken(LjsTokenType.Operator, _reader.CurrentIndex, 1, _currentLine));
            }
            else
            {
                throw new LjsTokenizerError(
                    "unknown token",
                    new LjsTokenPosition(_reader.CurrentIndex, _currentLine, _currentCol));
            }
            

        }
    }

    private void AddToken(LjsToken token)
    {
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
               c == ';' ||
               c == '{' || c == '}' ||
               c == '(' || c == ')' ||
               c == '[' || c == ']';
    }

    /// <summary>
    /// check if char is space, newline, tabulation or special ansii code at the beginning of ansii table
    /// </summary>
    private static bool IsEmptySpace(char c)
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
    
    private class SourceCodeCharsReader
    {
        private readonly LjsSourceCode _sourceCode;

        private int _currentIndex = -1;
    
        public SourceCodeCharsReader(LjsSourceCode sourceCode)
        {
            _sourceCode = sourceCode;
        }

        public int CurrentIndex => _currentIndex;

        public char CurrentChar => 
            _currentIndex != -1 ? _sourceCode[_currentIndex] : (char) 0;

        public char NextChar => 
            _currentIndex + 1 < _sourceCode.Length ? _sourceCode[_currentIndex + 1] : (char)0;

        public char PrevChar => 
            _currentIndex > 0 ? _sourceCode[_currentIndex - 1] : (char)0;

        public bool HasNextChar => _currentIndex + 1 < _sourceCode.Length;

        public void MoveForward()
        {
            if (!HasNextChar)
            {
                throw new Exception("str end");
            }
        
            ++_currentIndex;
        }
    }

}