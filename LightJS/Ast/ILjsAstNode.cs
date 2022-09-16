namespace LightJS.Ast;

public interface ILjsAstNode
{
    IEnumerable<ILjsAstNode> ChildNodes { get; }
    
    bool HasChildNodes { get; }
}