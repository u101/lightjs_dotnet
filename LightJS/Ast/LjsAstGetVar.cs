namespace LightJS.Ast;

public sealed class LjsAstGetVar : ILjsAstNode
{
    public string VarName { get; }

    public LjsAstGetVar(string varName)
    {
        VarName = varName;
    }
    
    
    
}