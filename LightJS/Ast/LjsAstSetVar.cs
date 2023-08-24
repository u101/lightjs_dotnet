namespace LightJS.Ast;

public class LjsAstSetVar : ILjsAstNode
{
    public string VarName { get; }
    public ILjsAstNode Expression { get; }
    public LjsAstAssignMode AssignMode { get; }

    public LjsAstSetVar(string varName, ILjsAstNode expression, LjsAstAssignMode assignMode)
    {
        VarName = varName;
        Expression = expression;
        AssignMode = assignMode;
    }
    
}