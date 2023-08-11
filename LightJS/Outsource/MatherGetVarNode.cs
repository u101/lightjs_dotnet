namespace LightJS.Outsource;

public class MatherGetVarNode : IMatherNode
{
    public string Id { get; }

    public MatherGetVarNode(string id)
    {
        Id = id;
    }
    
}