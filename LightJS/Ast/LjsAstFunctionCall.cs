namespace LightJS.Ast;

public class LjsAstFunctionCall : ILjsAstNode
{
    public ILjsAstNode FunctionGetter { get; }

    public List<ILjsAstNode> Arguments { get; } = new();

    public LjsAstFunctionCall(ILjsAstNode functionGetter)
    {
        FunctionGetter = functionGetter;
    }

    public IEnumerable<ILjsAstNode> ChildNodes
    {
        get
        {
            var list = new List<ILjsAstNode> { FunctionGetter };
            list.AddRange(Arguments);
            return list;
        }
    }
    public bool HasChildNodes => true;
}