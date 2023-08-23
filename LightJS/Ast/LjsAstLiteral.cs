namespace LightJS.Ast;


public class LjsAstLiteral<TValue> : ILjsAstNode
{
    public TValue Value { get; }

    public LjsAstLiteral(TValue value)
    {
        Value = value;
    }
    
}

