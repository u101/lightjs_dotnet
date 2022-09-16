namespace LightJS.Ast;

/// <summary>
/// Abstract syntax tree leaf node that does not have children
/// </summary>
public abstract class LjsAstLeafNode : ILjsAstNode
{
    public IEnumerable<ILjsAstNode> ChildNodes => Array.Empty<ILjsAstNode>();

    public bool HasChildNodes => false;
}