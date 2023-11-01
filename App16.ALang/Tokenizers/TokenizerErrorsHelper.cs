namespace App16.ALang.Tokenizers;

public static class TokenizerErrorsHelper
{
    public static void ThrowInvalidNumberFormatError(this ICharsReader reader)
    {
        throw new TokenizerError("invalid number format", reader.CurrentTokenPosition);
    }
}