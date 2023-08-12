namespace LightJS.Ast;

public class LjsAstGetVar : ILjsAstNode
{
    public string VarName { get; }

    public LjsAstGetVar(string varName)
    {
        VarName = varName;
    }
    
    
    
}