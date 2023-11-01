using App16.ALang.Tokenizers;

namespace App16.ALang.Ast.Errors;

public class AstInvalidTernaryOperation : Exception
{
    private const string MessageString = "invalid binary operation";
    
    public AstTernaryOperationInfo OperationInfo { get; }
    public Token Token { get; }
    

    public AstInvalidTernaryOperation(AstTernaryOperationInfo operationInfo, Token token) : base(MessageString)
    {
        OperationInfo = operationInfo;
        Token = token;
    }
}