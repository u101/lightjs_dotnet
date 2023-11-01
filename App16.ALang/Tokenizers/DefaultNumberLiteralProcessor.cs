namespace App16.ALang.Tokenizers;

public class DefaultNumberLiteralProcessor : ITokensProcessor
{
    private const char Dot = '.';


    private readonly int _integerTokenType;
    private readonly int _realNumberTokenType;
    private readonly int _expNotationNumberTokenType;
    private readonly HashSet<char> _operatorSymbolsSet;

    public DefaultNumberLiteralProcessor(
        int integerTokenType,
        int realNumberTokenType,
        int expNotationNumberTokenType,
        HashSet<char> operatorSymbolsSet)
    {
        _integerTokenType = integerTokenType;
        _realNumberTokenType = realNumberTokenType;
        _expNotationNumberTokenType = expNotationNumberTokenType;
        _operatorSymbolsSet = operatorSymbolsSet;
    }

    public bool Process(TokenizerContext ctx)
    {
        var reader = ctx.CharsReader;
        var c = reader.CurrentChar;

        if (!char.IsDigit(c)) return false;

        var startIndex = reader.CurrentIndex;
        var tokenPos = reader.CurrentTokenPosition;

        var hasDot = false;
        var hasExponentMark = false;

        while (reader.HasNextChar &&
               !TokenizerUtils.IsEmptySpaceChar(reader.NextChar) &&
               (reader.NextChar == Dot || !IsOperator(reader.NextChar)))
        {
            reader.MoveForward();

            if (reader.CurrentChar == Dot)
            {
                if (hasDot || hasExponentMark ||
                    !reader.HasNextChar || !char.IsDigit(reader.NextChar))
                {
                    reader.ThrowInvalidNumberFormatError();
                }

                hasDot = true;
            }
            else if (reader.CurrentChar == 'e')
            {
                if (hasExponentMark || !reader.HasNextChar)
                {
                    reader.ThrowInvalidNumberFormatError();
                }

                reader.MoveForward();

                if (reader.CurrentChar != '+' && reader.CurrentChar != '-')
                {
                    reader.ThrowInvalidNumberFormatError();
                }

                if (!reader.HasNextChar || !char.IsDigit(reader.NextChar))
                {
                    reader.ThrowInvalidNumberFormatError();
                }

                hasExponentMark = true;
            }
            else if (!char.IsDigit(reader.CurrentChar))
            {
                reader.ThrowInvalidNumberFormatError();
            }
        }

        var ln = (reader.CurrentIndex + 1) - startIndex;

        ctx.AddToken(new Token(
            GetNumberTokenType(hasDot, hasExponentMark), tokenPos, ln));

        return true;
    }

    private int GetNumberTokenType(bool hasDot, bool hasExponentMark) =>
        hasExponentMark ? _expNotationNumberTokenType : (hasDot ? _realNumberTokenType : _integerTokenType);

    private bool IsOperator(char c) => _operatorSymbolsSet.Contains(c);
}