namespace LightJS;

public class LjsAstModel
{
    public List<ILjsAstNode> RootNodes { get; }

    public LjsAstModel(List<ILjsAstNode> rootNodes)
    {
        RootNodes = rootNodes;
    }
    
}