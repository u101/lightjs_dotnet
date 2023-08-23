namespace LightJS.Ast;

public sealed class LjsAstObjectLiteral : LjsAstSequence<LjsAstObjectLiteralProperty>
{
    public LjsAstObjectLiteral()
    { }

    public LjsAstObjectLiteral(IEnumerable<LjsAstObjectLiteralProperty> nodes) : base(nodes)
    { }

    public LjsAstObjectLiteral(params LjsAstObjectLiteralProperty[] nodes) : base(nodes)
    { }
}

public sealed class LjsAstObjectLiteralProperty : ILjsAstNode
{
    public string Name { get; }
    public ILjsAstNode Value { get; }

    public LjsAstObjectLiteralProperty(string name, ILjsAstNode value)
    {
        Name = name;
        Value = value;
    }
}