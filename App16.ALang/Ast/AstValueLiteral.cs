using System.Diagnostics.CodeAnalysis;
using App16.ALang.Tokenizers;

namespace App16.ALang.Ast;

public sealed class AstValueLiteral<TValue> : AstNode, IAstValueNode
{
    [NotNull]
    public TValue Value { get; }

    public AstValueLiteral(TValue value, Token token = default) : base(token)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public override string ToString()
    {
        return Value?.ToString() ?? "";
    }
}