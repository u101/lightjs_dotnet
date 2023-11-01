using App16.ALang.Tokenizers;

namespace App16.ALang.Ast.Errors;

public class AstInvalidBinaryOperation : Exception
{
    private const string MessageString = "invalid binary operation";
    
    public AstBinaryOperationInfo OperationInfo { get; }
    public Token Token { get; }
    

    public AstInvalidBinaryOperation(AstBinaryOperationInfo operationInfo, Token token) : base(MessageString)
    {
        OperationInfo = operationInfo;
        Token = token;
    }
}