namespace LightJS.Outsource;

public class MatherLiteralNode : IMatherNode
{
    public string Value { get; }

    public MatherLiteralNode(string value)
    {
        Value = value;
    }
    
}