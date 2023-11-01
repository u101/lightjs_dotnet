using App16.ALang.Ast.Builders;

namespace App16.ALang.Ast;

public sealed class AstTernaryOperationInfo
{
    public int TokenType { get; }
    public int DelimiterTokenType { get; }
    public int OperationPriority { get; }

    public IAstProcessorStopPoint DelimiterStopPoint { get; }

    public AstTernaryOperationInfo(
        int tokenType,
        int delimiterTokenType,
        int operationPriority)
    {
        TokenType = tokenType;
        DelimiterTokenType = delimiterTokenType;
        OperationPriority = operationPriority;
        DelimiterStopPoint = new AstStopPointBeforeToken(delimiterTokenType, false);
    }
}