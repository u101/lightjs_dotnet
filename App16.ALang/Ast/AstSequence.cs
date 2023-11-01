using App16.ALang.Tokenizers;

namespace App16.ALang.Ast;

public abstract class AstSequence<TNode> : IAstNode where TNode : IAstNode
{
    private readonly List<TNode> _nodes = new();

    private readonly Token _token;
    
    public Token GetToken() => default;

    public IEnumerable<TNode> ChildNodes => _nodes;
    public bool HasChildNodes => _nodes.Count != 0;

    public int Count => _nodes.Count;

    public bool IsEmpty => _nodes.Count == 0;

    protected AstSequence(Token token = default)
    {
        _token = token;
    }

    protected AstSequence(IEnumerable<TNode> nodes)
    {
        _nodes.AddRange(nodes);
        _token = default;
    }
    
    protected AstSequence(params TNode[] nodes)
    {
        _nodes.AddRange(nodes);
        _token = default;
    }

    public TNode this[int index] => _nodes[index];

    public void AddNode(TNode node)
    {
        _nodes.Add(node);
    }
    
}

public sealed class AstSequence : AstSequence<IAstNode>
{
    public AstSequence() {}

    public AstSequence(IEnumerable<IAstNode> nodes) : base(nodes) {}

    public AstSequence(params IAstNode[] nodes) : base(nodes) {}
}