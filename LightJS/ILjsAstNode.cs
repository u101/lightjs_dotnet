namespace LightJS;

public interface ILjsAstNode
{
    IEnumerable<ILjsAstNode> ChildNodes { get; }
    
    bool HasChildNodes { get; }
}