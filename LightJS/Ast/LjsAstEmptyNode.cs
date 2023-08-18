namespace LightJS.Ast;

public class LjsAstEmptyNode : ILjsAstNode
{

    public static readonly ILjsAstNode Instance = new LjsAstEmptyNode();
    
    private LjsAstEmptyNode() {}
    
}