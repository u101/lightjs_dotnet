namespace LightJS.Ast;

public class LjsAstArrayLiteral : LjsAstSequence<ILjsAstNode>
{
    // just a sequence
    public LjsAstArrayLiteral()
    {}

    public LjsAstArrayLiteral(IEnumerable<ILjsAstNode> nodes) : base(nodes)
    {}

    public LjsAstArrayLiteral(params ILjsAstNode[] nodes) : base(nodes)
    {}
}