namespace LightJS.Ast;

public class LjsAstGetVar : LjsAstLeafNode
{
    public string VarName { get; }

    public LjsAstGetVar(string varName)
    {
        VarName = varName;
    }
    
    
    
}