namespace LightJS.Ast;

public abstract class LjsAstLiteral : ILjsAstNode {}

public class LjsAstLiteral<TValue> : LjsAstLiteral
{
    public TValue Value { get; }

    public LjsAstLiteral(TValue value)
    {
        Value = value;
    }
    
}

public class LjsAstNull : LjsAstLiteral
{
    public LjsAstNull() {}
}

public class LjsAstUndefined : LjsAstLiteral
{
    public LjsAstUndefined() {}
}