namespace LightJS.Ast;

public sealed class LjsAstIncrementVar : ILjsAstNode
{
    public string VarName { get; }
    public LjsAstIncrementSign Sign { get; }
    public LjsAstIncrementOrder Order { get; }

    public LjsAstIncrementVar(
        string varName, 
        LjsAstIncrementSign sign, 
        LjsAstIncrementOrder order)
    {
        VarName = varName;
        Sign = sign;
        Order = order;
    }
}