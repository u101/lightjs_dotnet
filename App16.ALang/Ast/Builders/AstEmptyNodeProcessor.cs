namespace App16.ALang.Ast.Builders;

public sealed class AstEmptyNodeProcessor : IAstNodeProcessor
{
    public static readonly IAstNodeProcessor Instance = new AstEmptyNodeProcessor();
    
    private AstEmptyNodeProcessor() {}
    
    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        return AstEmptyNode.Instance;
    }
}