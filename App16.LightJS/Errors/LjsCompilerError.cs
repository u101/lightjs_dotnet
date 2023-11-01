using App16.ALang.Tokenizers;

namespace App16.LightJS.Errors;

public class LjsCompilerError : Exception
{
    public Token Token { get; }

    public LjsCompilerError() {}

    public LjsCompilerError(string errorMessage, Token token = default) : base(errorMessage)
    {
        Token = token;
    }
}