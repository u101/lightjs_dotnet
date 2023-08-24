namespace LightJS.Ast;

public sealed class LjsAstFunctionCall : ILjsAstNode
{
    public ILjsAstNode FunctionGetter { get; }

    public List<ILjsAstNode> Arguments { get; } = new();

    public LjsAstFunctionCall(ILjsAstNode functionGetter)
    {
        FunctionGetter = functionGetter;
    }
    
    public LjsAstFunctionCall(ILjsAstNode functionGetter, params ILjsAstNode[] args)
    {
        FunctionGetter = functionGetter;
        Arguments.AddRange(args);
    }
}