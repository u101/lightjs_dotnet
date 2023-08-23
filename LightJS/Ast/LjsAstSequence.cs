namespace LightJS.Ast;

public abstract class LjsAstSequence<TNode> : ILjsAstNode where TNode : ILjsAstNode
{
    private readonly List<TNode> _nodes = new();

    public IEnumerable<TNode> ChildNodes => _nodes;
    public bool HasChildNodes => _nodes.Count != 0;
    
    public LjsAstSequence() {}

    public LjsAstSequence(IEnumerable<TNode> nodes)
    {
        _nodes.AddRange(nodes);
    }

    public LjsAstSequence(params TNode[] nodes)
    {
        _nodes.AddRange(nodes);
    }

    public void AddNode(TNode node)
    {
        _nodes.Add(node);
    }
}

public class LjsAstSequence : LjsAstSequence<ILjsAstNode>
{
    public LjsAstSequence() {}

    public LjsAstSequence(IEnumerable<ILjsAstNode> nodes) : base(nodes) {}

    public LjsAstSequence(params ILjsAstNode[] nodes) : base(nodes) {}
}