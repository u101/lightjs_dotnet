namespace LightJS.Ast;

public class LjsAstVariableDeclaration : ILjsAstNode
{
    public string Name { get; }
    public ILjsAstNode Value { get; }

    public LjsAstVariableDeclaration(string name, ILjsAstNode value)
    {
        Name = name;
        Value = value;
    }
    
    public LjsAstVariableDeclaration(string name)
    {
        Name = name;
        Value = LjsAstEmptyNode.Instance;
    }
    
}