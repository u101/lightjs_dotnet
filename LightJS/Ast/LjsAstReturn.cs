namespace LightJS.Ast;

public sealed class LjsAstReturn : ILjsAstNode
{
    public ILjsAstNode ReturnValue { get; }

    public LjsAstReturn(ILjsAstNode returnValue)
    {
        ReturnValue = returnValue;
    }

    public LjsAstReturn()
    {
        ReturnValue = LjsAstEmptyNode.Instance;
    }
    
}