namespace LightJS.Ast;

public sealed class LjsAstFunctionCall : ILjsAstNode
{
    public ILjsAstNode FunctionGetter { get; }

    public LjsAstFunctionCallArguments Arguments { get; }

    public LjsAstFunctionCall(ILjsAstNode functionGetter, LjsAstFunctionCallArguments arguments)
    {
        FunctionGetter = functionGetter;
        Arguments = arguments;
    }
}