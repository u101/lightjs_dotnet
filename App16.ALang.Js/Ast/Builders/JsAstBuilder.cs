using App16.ALang.Ast;
using App16.ALang.Ast.Builders;
using App16.ALang.Ast.Errors;
using App16.ALang.Js.Errors;
using App16.ALang.Js.Tokenizers;
using App16.ALang.Tokenizers;

namespace App16.ALang.Js.Ast.Builders;

public sealed class JsAstBuilder
{
    private readonly string _sourceCodeString;
    private readonly AstModelBuilder _modelBuilder;

    public JsAstBuilder(string sourceCodeString, AstModelBuilder modelBuilder)
    {
        _sourceCodeString = sourceCodeString;
        _modelBuilder = modelBuilder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="JsSyntaxError"></exception>
    /// <returns></returns>
    public IAstNode Build()
    {
        try
        {
            var result = _modelBuilder.Build();
            return result;
        }
        catch (AstUnexpectedTokenError unexpectedTokenError)
        {
            throw new JsSyntaxError(
                $"unexpected token {TokenToString(unexpectedTokenError.Token)}",
                unexpectedTokenError.Token.Position);
        }
        catch (AstUnexpectedEofError eofError)
        {
            throw new JsSyntaxError("unexpected EOF");
        }
        catch (AstInvalidBinaryOperation invalidBinaryOperation)
        {
            throw new JsSyntaxError($"invalid binary operation {TokenToString(invalidBinaryOperation.Token)}",
                invalidBinaryOperation.Token.Position);
        }
        catch (AstInvalidTernaryOperation invalidBinaryOperation)
        {
            throw new JsSyntaxError($"invalid ternary operation {TokenToString(invalidBinaryOperation.Token)}",
                invalidBinaryOperation.Token.Position);
        }
    }

    private string TokenToString(Token token)
    {
        if (token.TokenType == JsTokenTypes.None || 
            token.StringLength <= 0) return "NONE";
        return TokenizerUtils.GetTokenStringValue(_sourceCodeString, token);
    }
    
}