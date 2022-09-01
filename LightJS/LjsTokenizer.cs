namespace LightJS;

public class LjsTokenizer
{
    private readonly LjsReader _reader;
    private readonly List<LjsToken> _tokens = new List<LjsToken>();

    private int _line = 0;
    private int _col = 0;

    private static char End = (char)0;
    private static char DoubleQuotes = '"';
    private static char SingleQuotes = '\'';
    private static char NewLine = '\n';
    private static char Slash = '/';

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

        while (_reader.HasNextChar())
        {
            var c = ReadNextChar();
            
            if (IsEmptyChar(c)) continue;

            if (c == DoubleQuotes || c == SingleQuotes)
            {
                ReadString(c);
            }


        }
    }

    private void ReadString(char startChar)
    {
        while (_reader.HasNextChar())
        {
            var c = ReadNextChar();
            
            if (IsEmptyChar(c)) continue;

            if (c == DoubleQuotes || c == SingleQuotes)
            {
                ReadString(c);
            }


        }
    }

    /// <summary>
    /// check if char with specified char code is space, newline, tabulation or special ansii code
    /// </summary>
    private static bool IsEmptyChar(char c)
    {
        var charCode = (int)c;
        
        return charCode <= 32;
    }

    private static bool IsUnsupportedChar(char c)
    {
        return c >= 127;
    }

    private static bool IsSymbolChar(char c)
    {
        var charCode = (int)c;
        
        return (charCode >= 33 && charCode <= 47) ||
               (charCode >= 58 && charCode <= 64) ||
               (charCode >= 91 && charCode <= 96) ||
               (charCode >= 123 && charCode <= 126);
    }

    private static bool IsLetterChar(char c)
    {
        var charCode = (int)c;

        return (charCode >= 65 && charCode <= 90) ||
               (charCode >= 97 && charCode <= 122);
    }

    private static bool IsNumberChar(char c)
    {
        var charCode = (int)c;
        return charCode >= 48 && charCode <= 57;
    }
    

    private char ReadNextChar()
    {
        if (!_reader.HasNextChar()) return End;
        
        var c = _reader.ReadNextChar();

        if (c == '\n')
        {
            ++_line;
            _col = 0;
        }
        else
        {
            ++_col;
        }
        
        return c;
    }

}

/*
char  code 0
char  code 1
char  code 2
char  code 3
char  code 4
char  code 5
char  code 6
char  code 7
char  code 8
char  code 9
char  code 10
char  code 11
char  code 12
char  code 13
char  code 14
char  code 15
char  code 16
char  code 17
char  code 18
char  code 19
char  code 20
char  code 21
char  code 22
char  code 23
char  code 24
char  code 25
char  code 26
char  code 27
char  code 28
char  code 29
char  code 30
char  code 31
char  code 32

char ! code 33
char " code 34
char # code 35
char $ code 36
char % code 37
char & code 38
char ' code 39
char ( code 40
char ) code 41
char * code 42
char + code 43
char , code 44
char - code 45
char . code 46
char / code 47

char 0 code 48
char 1 code 49
char 2 code 50
char 3 code 51
char 4 code 52
char 5 code 53
char 6 code 54
char 7 code 55
char 8 code 56
char 9 code 57

char : code 58
char ; code 59
char < code 60
char = code 61
char > code 62
char ? code 63
char @ code 64

char A code 65
char B code 66
char C code 67
char D code 68
char E code 69
char F code 70
char G code 71
char H code 72
char I code 73
char J code 74
char K code 75
char L code 76
char M code 77
char N code 78
char O code 79
char P code 80
char Q code 81
char R code 82
char S code 83
char T code 84
char U code 85
char V code 86
char W code 87
char X code 88
char Y code 89
char Z code 90

char [ code 91
char \ code 92
char ] code 93
char ^ code 94
char _ code 95
char ` code 96

char a code 97
char b code 98
char c code 99
char d code 100
char e code 101
char f code 102
char g code 103
char h code 104
char i code 105
char j code 106
char k code 107
char l code 108
char m code 109
char n code 110
char o code 111
char p code 112
char q code 113
char r code 114
char s code 115
char t code 116
char u code 117
char v code 118
char w code 119
char x code 120
char y code 121
char z code 122

char { code 123
char | code 124
char } code 125
char ~ code 126

char code 127
*/