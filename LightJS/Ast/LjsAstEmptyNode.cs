namespace LightJS.Ast;

public sealed class LjsAstEmptyNode : ILjsAstNode
{

    public static readonly ILjsAstNode Instance = new LjsAstEmptyNode();
    
    private LjsAstEmptyNode() {}
    
}