namespace LightJS.Tokenizer;

public class LjsTokenizer
{
    private readonly string _sourceCodeString;
    
    private SourceCodeCharsReader _reader;
    private List<LjsToken> _tokens;
    
    private const char DoubleQuotes = '"';
    private const char SingleQuotes = '\'';
    private const char NewLine = '\n';
    private const char Slash = '/';
    private const char BackSlash = '\\';
    private const char Asterisk = '*';
    private const char Dot = '.';

    private int _currentLine;
    private int _currentCol;

    private static readonly Dictionary<string, LjsTokenType> _keywordsMap = new()
    {
        {"null", LjsTokenType.Null},
        {"undefined", LjsTokenType.Undefined},
        {"this", LjsTokenType.This},
        
        {"true", LjsTokenType.True},
        {"false", LjsTokenType.False},
        
        {"var", LjsTokenType.Var},
        {"const", LjsTokenType.Const},
        {"function", LjsTokenType.Function},
        
        {"return", LjsTokenType.Return},
        {"break", LjsTokenType.Break},
        {"continue", LjsTokenType.Continue},
        
        {"if", LjsTokenType.If},
        {"else", LjsTokenType.Else},
        {"while", LjsTokenType.While},
        {"do", LjsTokenType.Do},
        {"for", LjsTokenType.For},
    };

    public LjsTokenizer(string sourceCodeString)
    {
        if (string.IsNullOrEmpty(sourceCodeString))
        {
            throw new ArgumentException("input string is null or empty");
        }
        
        _sourceCodeString = sourceCodeString;
    }

    public List<LjsToken> ReadTokens()
    {
        _currentCol = 0;
        _currentLine = 0;
        
        _reader = new SourceCodeCharsReader(_sourceCodeString);
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
                var startIndex = _reader.CurrentIndex + 1; // we ignore quotes (+1)
                var tokenPos = new LjsTokenPosition(startIndex, _currentLine, _currentCol);

                ReadNextChar();

                var hasEscapeChar = false;
                
                while (_reader.CurrentChar != c || hasEscapeChar)
                {
                    if (!_reader.HasNextChar || _reader.NextChar == NewLine)
                    {
                        throw new LjsSyntaxError(
                            "unterminated string literal", 
                            new LjsTokenPosition(_reader.CurrentIndex, _currentLine, _currentCol));
                    }

                    hasEscapeChar = !hasEscapeChar && _reader.CurrentChar == BackSlash;

                    ReadNextChar();
                }

                var ln = _reader.CurrentIndex - startIndex; // end index is exclusive
                
                AddToken(new LjsToken(
                    LjsTokenClass.Literal, LjsTokenType.StringLiteral, tokenPos, ln));
            }
            // key word or identifier
            else if (IsLetterChar(c))
            {
                var startIndex = _reader.CurrentIndex;
                var tokenPos = new LjsTokenPosition(startIndex, _currentLine, _currentCol);

                while (_reader.HasNextChar && 
                       (IsLetterChar(_reader.NextChar) || char.IsDigit(_reader.NextChar)))
                {
                    ReadNextChar();
                }

                var ln = (_reader.CurrentIndex + 1) - startIndex;

                var wordSpan = _sourceCodeString.Substring(startIndex, ln);

                if (_keywordsMap.TryGetValue(wordSpan, out var tokenType))
                {
                    switch (tokenType)
                    {
                        case LjsTokenType.Null:
                        case LjsTokenType.Undefined:
                        case LjsTokenType.True:
                        case LjsTokenType.False:
                            AddToken(new LjsToken(LjsTokenClass.Literal, tokenType, tokenPos, ln));
                            break;
                        default:
                            AddToken(new LjsToken(LjsTokenClass.Word, tokenType, tokenPos, ln));
                            break;
                    }
                }
                else
                {
                    AddToken(new LjsToken(
                        LjsTokenClass.Word, LjsTokenType.Identifier, tokenPos, ln));
                }

                
            }
            // hex int
            else if (c == '0' && _reader.HasNextChar && _reader.NextChar == 'x')
            {
                var startIndex = _reader.CurrentIndex;
                var tokenPos = new LjsTokenPosition(startIndex, _currentLine, _currentCol);

                ReadNextChar(); // skip char x

                if (!_reader.HasNextChar || !IsHexChar(_reader.NextChar))
                {
                    ThrowInvalidNumberFormatError();
                }

                while (_reader.HasNextChar && 
                       !IsEmptySpace(_reader.NextChar) && 
                       !IsOperator(_reader.NextChar))
                {
                    ReadNextChar();

                    if (!IsHexChar(_reader.CurrentChar))
                    {
                        ThrowInvalidNumberFormatError();
                    }
                }
                
                var ln = (_reader.CurrentIndex + 1) - startIndex;
                
                AddToken(new LjsToken(LjsTokenClass.Literal, LjsTokenType.IntHex, tokenPos, ln));
            }
            // binary int
            else if (c == '0' && _reader.HasNextChar && _reader.NextChar == 'b')
            {
                var startIndex = _reader.CurrentIndex;
                var tokenPos = new LjsTokenPosition(startIndex, _currentLine, _currentCol);

                ReadNextChar(); // skip char b

                if (!_reader.HasNextChar || !IsBinaryDigitChar(_reader.NextChar))
                {
                    ThrowInvalidNumberFormatError();
                }

                while (_reader.HasNextChar && 
                       !IsEmptySpace(_reader.NextChar) && 
                       !IsOperator(_reader.NextChar))
                {
                    ReadNextChar();

                    if (!IsBinaryDigitChar(_reader.CurrentChar))
                    {
                        ThrowInvalidNumberFormatError();
                    }
                }
                
                var ln = (_reader.CurrentIndex + 1) - startIndex;
                
                AddToken(new LjsToken(
                    LjsTokenClass.Literal, LjsTokenType.IntBinary, tokenPos, ln));
            }
            
            // number
            else if (char.IsDigit(c))
            {
                var startIndex = _reader.CurrentIndex;
                var tokenPos = new LjsTokenPosition(startIndex, _currentLine, _currentCol);

                var hasDot = false;
                var hasExponentMark = false;

                while (_reader.HasNextChar && 
                       !IsEmptySpace(_reader.NextChar) && 
                       (_reader.NextChar == Dot || !IsOperator(_reader.NextChar)))
                {
                    ReadNextChar();

                    if (_reader.CurrentChar == Dot)
                    {
                        if (hasDot || hasExponentMark || 
                            !_reader.HasNextChar || !char.IsDigit(_reader.NextChar))
                        {
                            ThrowInvalidNumberFormatError();
                        }

                        hasDot = true;
                    }
                    else if (_reader.CurrentChar == 'e')
                    {
                        if (hasExponentMark || !_reader.HasNextChar)
                        {
                            ThrowInvalidNumberFormatError();
                        }
                        
                        ReadNextChar();

                        if (_reader.CurrentChar != '+' && _reader.CurrentChar != '-')
                        {
                            ThrowInvalidNumberFormatError();
                        }

                        if (!_reader.HasNextChar || !char.IsDigit(_reader.NextChar))
                        {
                            ThrowInvalidNumberFormatError();
                        }

                        hasExponentMark = true;
                    }
                    else if (!char.IsDigit(_reader.CurrentChar))
                    {
                        ThrowInvalidNumberFormatError();
                    }
                }
                
                var ln = (_reader.CurrentIndex + 1) - startIndex;

                AddToken(new LjsToken(
                    LjsTokenClass.Literal, GetNumberTokenType(hasDot, hasExponentMark), tokenPos, ln));
            }
            else if (IsOperator(c))
            {
                var prevToken = PreviousToken;
                var opPosition = new LjsTokenPosition(_reader.CurrentIndex, _currentLine, _currentCol);
                var opType = GetOperatorTokenType(c);
                var compOp = LjsTokenType.None;

                if ((prevToken.TokenClass & LjsTokenClass.Operator) != 0 && 
                    prevToken.Position.IsAdjacentTo(opPosition, prevToken.StringLength) && 
                    (compOp = GetOperatorComposition(prevToken.TokenType, opType)) != LjsTokenType.None)
                {
                    ReplaceLastToken(new LjsToken(
                        GetOperatorTokenClass(compOp), compOp, prevToken.Position, prevToken.StringLength + 1));
                }
                else
                {
                    AddToken(new LjsToken(
                        GetOperatorTokenClass(opType), opType, opPosition,1));
                }
            }
            else
            {
                throw new LjsSyntaxError(
                    "unknown token",
                    new LjsTokenPosition(_reader.CurrentIndex, _currentLine, _currentCol));
            }
            

        }
    }

    private void ThrowInvalidNumberFormatError()
    {
        throw new LjsSyntaxError(
            "invalid number format", 
            new LjsTokenPosition(_reader.CurrentIndex, _currentLine, _currentCol));
    }

    private static LjsTokenType GetNumberTokenType(bool hasDot, bool hasExponentMark)
    {
        if (hasExponentMark) return LjsTokenType.FloatE;
        return hasDot ? LjsTokenType.Float : LjsTokenType.IntDecimal;
    }
    
    private void AddToken(LjsToken token)
    {
        _tokens.Add(token);
    }

    private void ReplaceLastToken(LjsToken newToken)
    {
        _tokens[^1] = newToken;
    }

    private LjsToken PreviousToken => 
        _tokens.Count > 0 ? _tokens[^1] : default;

    private static LjsTokenType GetOperatorComposition(LjsTokenType prevOp, LjsTokenType nexOp)
    {
        return prevOp switch
        {
            LjsTokenType.OpPlus when nexOp == LjsTokenType.OpPlus => LjsTokenType.OpIncrement,
            LjsTokenType.OpMinus when nexOp == LjsTokenType.OpMinus => LjsTokenType.OpDecrement,
            
            LjsTokenType.OpPlus when nexOp == LjsTokenType.OpAssign => LjsTokenType.OpPlusAssign,
            LjsTokenType.OpMinus when nexOp == LjsTokenType.OpAssign => LjsTokenType.OpMinusAssign,
            
            LjsTokenType.OpAssign when nexOp == LjsTokenType.OpAssign => LjsTokenType.OpEquals,
            LjsTokenType.OpEquals when nexOp == LjsTokenType.OpAssign => LjsTokenType.OpEqualsStrict,
            
            LjsTokenType.OpGreater when nexOp == LjsTokenType.OpAssign => LjsTokenType.OpGreaterOrEqual,
            LjsTokenType.OpLess when nexOp == LjsTokenType.OpAssign => LjsTokenType.OpLessOrEqual,
            LjsTokenType.OpNegate when nexOp == LjsTokenType.OpAssign => LjsTokenType.OpNotEqual,
            LjsTokenType.OpNotEqual when nexOp == LjsTokenType.OpAssign => LjsTokenType.OpNotEqualStrict,
            
            LjsTokenType.OpBitAnd when nexOp == LjsTokenType.OpBitAnd => LjsTokenType.OpLogicalAnd,
            LjsTokenType.OpBitOr when nexOp == LjsTokenType.OpBitOr => LjsTokenType.OpLogicalOr,
            
            
            _ => LjsTokenType.None
        };
    }
    
    private static LjsTokenType GetOperatorTokenType(char c)
    {
        return c switch
        {
            '>' => LjsTokenType.OpGreater,
            '<' => LjsTokenType.OpLess,
            '=' => LjsTokenType.OpAssign,
            '+' => LjsTokenType.OpPlus,
            '-' => LjsTokenType.OpMinus,
            '*' => LjsTokenType.OpMultiply,
            '/' => LjsTokenType.OpDiv,
            '&' => LjsTokenType.OpBitAnd,
            '|' => LjsTokenType.OpBitOr,
            '!' => LjsTokenType.OpNegate,
            '?' => LjsTokenType.OpQuestionMark,
            ',' => LjsTokenType.OpComma,
            '.' => LjsTokenType.OpDot,
            ':' => LjsTokenType.OpColon,
            ';' => LjsTokenType.OpSemicolon,
            '{' => LjsTokenType.OpBracketOpen,
            '}' => LjsTokenType.OpBracketClose,
            '(' => LjsTokenType.OpParenthesesOpen,
            ')' => LjsTokenType.OpParenthesesClose,
            '[' => LjsTokenType.OpSquareBracketsOpen,
            ']' => LjsTokenType.OpSquareBracketsClose,
            _ => LjsTokenType.None
        };
    }

    private static LjsTokenClass GetOperatorTokenClass(LjsTokenType tokenType)
    {
        return tokenType switch
        {
            LjsTokenType.OpPlus => LjsTokenClass.Operator | LjsTokenClass.PolymorphicOperator,
            LjsTokenType.OpMinus => LjsTokenClass.Operator | LjsTokenClass.PolymorphicOperator,
            LjsTokenType.OpNegate => LjsTokenClass.Operator | LjsTokenClass.UnaryOperator,
            LjsTokenType.OpIncrement => LjsTokenClass.Operator | LjsTokenClass.UnaryOperator,
            LjsTokenType.OpDecrement => LjsTokenClass.Operator | LjsTokenClass.UnaryOperator,
            
            LjsTokenType.OpGreater => LjsTokenClass.Operator | LjsTokenClass.BinaryOperator,
            LjsTokenType.OpLess => LjsTokenClass.Operator | LjsTokenClass.BinaryOperator,
            LjsTokenType.OpAssign => LjsTokenClass.Operator | LjsTokenClass.BinaryOperator,
            LjsTokenType.OpMultiply => LjsTokenClass.Operator | LjsTokenClass.BinaryOperator,
            LjsTokenType.OpDiv => LjsTokenClass.Operator | LjsTokenClass.BinaryOperator,
            LjsTokenType.OpBitAnd => LjsTokenClass.Operator | LjsTokenClass.BinaryOperator,
            LjsTokenType.OpBitOr => LjsTokenClass.Operator | LjsTokenClass.BinaryOperator,
            
            LjsTokenType.OpPlusAssign => LjsTokenClass.Operator | LjsTokenClass.BinaryOperator,
            LjsTokenType.OpMinusAssign => LjsTokenClass.Operator | LjsTokenClass.BinaryOperator,
            LjsTokenType.OpEquals => LjsTokenClass.Operator | LjsTokenClass.BinaryOperator,
            LjsTokenType.OpEqualsStrict => LjsTokenClass.Operator | LjsTokenClass.BinaryOperator,
            LjsTokenType.OpGreaterOrEqual => LjsTokenClass.Operator | LjsTokenClass.BinaryOperator,
            LjsTokenType.OpLessOrEqual => LjsTokenClass.Operator | LjsTokenClass.BinaryOperator,
            LjsTokenType.OpNotEqual => LjsTokenClass.Operator | LjsTokenClass.BinaryOperator,
            LjsTokenType.OpNotEqualStrict => LjsTokenClass.Operator | LjsTokenClass.BinaryOperator,
            LjsTokenType.OpLogicalAnd => LjsTokenClass.Operator | LjsTokenClass.BinaryOperator,
            LjsTokenType.OpLogicalOr => LjsTokenClass.Operator | LjsTokenClass.BinaryOperator,
            
            _ => LjsTokenClass.Operator
        };
    }

    private static bool IsOperator(char c)
    {
        return GetOperatorTokenType(c) != LjsTokenType.None;
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

    private static bool IsBinaryDigitChar(char c)
    {
        return c == '0' || c == '1';
    }

    private static bool IsHexChar(char c)
    {
        var charCode = (int)c;
        
        return 
            // 0-9
            (charCode >= 48 && charCode <= 57) ||
            // A-F
            (charCode >= 65 && charCode <= 70) ||
            // a-f
            (charCode >= 97 && charCode <= 102);
    }
    
    private class SourceCodeCharsReader
    {
        private readonly string _sourceCodeString;

        private int _currentIndex = -1;
    
        public SourceCodeCharsReader(string sourceCodeString)
        {
            _sourceCodeString = sourceCodeString;
        }

        public int CurrentIndex => _currentIndex;

        public char CurrentChar => 
            _currentIndex != -1 ? _sourceCodeString[_currentIndex] : (char) 0;

        public char NextChar => 
            _currentIndex + 1 < _sourceCodeString.Length ? _sourceCodeString[_currentIndex + 1] : (char)0;

        public char PrevChar => 
            _currentIndex > 0 ? _sourceCodeString[_currentIndex - 1] : (char)0;

        public bool HasNextChar => _currentIndex + 1 < _sourceCodeString.Length;

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