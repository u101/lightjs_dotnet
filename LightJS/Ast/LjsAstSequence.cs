namespace LightJS.Ast;

public class LjsAstSequence : ILjsAstNode
{
    private readonly List<ILjsAstNode> _nodes = new();

    public IEnumerable<ILjsAstNode> ChildNodes => _nodes;
    public bool HasChildNodes => _nodes.Count != 0;

    public void AddNode(ILjsAstNode node)
    {
        _nodes.Add(node);
    }
}