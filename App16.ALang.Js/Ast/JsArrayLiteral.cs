using App16.ALang.Ast;
using App16.ALang.Tokenizers;

namespace App16.ALang.Js.Ast;

public sealed class JsArrayLiteral : AstSequence<IAstNode>, IAstValueNode
{
    // just a sequence
    public JsArrayLiteral(Token token = default) : base(token)
    {}

    public JsArrayLiteral(IEnumerable<IAstNode> nodes) : base(nodes)
    {}

    public JsArrayLiteral(params IAstNode[] nodes) : base(nodes)
    {}
}