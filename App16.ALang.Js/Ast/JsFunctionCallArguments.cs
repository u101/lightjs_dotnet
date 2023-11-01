using App16.ALang.Ast;

namespace App16.ALang.Js.Ast;

public sealed class JsFunctionCallArguments : AstSequence<IAstNode>
{
    public JsFunctionCallArguments() {}

    public JsFunctionCallArguments(IEnumerable<IAstNode> nodes) : base(nodes) {}

    public JsFunctionCallArguments(params IAstNode[] nodes) : base(nodes) {}
}