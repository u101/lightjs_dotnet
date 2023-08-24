namespace LightJS.Ast;


public sealed class LjsAstLiteral<TValue> : ILjsAstNode
{
    public TValue Value { get; }

    public LjsAstLiteral(TValue value)
    {
        Value = value;
    }
    
}

