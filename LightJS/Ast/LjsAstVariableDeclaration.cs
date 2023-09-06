namespace LightJS.Ast;

public sealed class LjsAstVariableDeclaration : ILjsAstNode
{
    public string Name { get; }
    public ILjsAstNode Value { get; }
    public LjsAstVariableKind VariableKind { get; }

    public LjsAstVariableDeclaration(string name, ILjsAstNode value, LjsAstVariableKind variableKind)
    {
        Name = name;
        Value = value;
        VariableKind = variableKind;
    }
    
    public LjsAstVariableDeclaration(string name, LjsAstVariableKind variableKind)
    {
        Name = name;
        Value = LjsAstEmptyNode.Instance;
        VariableKind = variableKind;
    }
    
}