namespace LightJS.Ast;

public class LjsAstFunctionCallArguments: LjsAstSequence<ILjsAstNode>
{
    public LjsAstFunctionCallArguments() {}

    public LjsAstFunctionCallArguments(IEnumerable<ILjsAstNode> nodes) : base(nodes) {}

    public LjsAstFunctionCallArguments(params ILjsAstNode[] nodes) : base(nodes) {}
}