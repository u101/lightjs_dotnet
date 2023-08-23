namespace LightJS.Ast;

public class LjsAstVariableDeclaration : ILjsAstNode
{
    public string Name { get; }
    public bool Mutable { get; }
    public ILjsAstNode Value { get; }

    public LjsAstVariableDeclaration(string name, ILjsAstNode value, bool mutable)
    {
        Name = name;
        Value = value;
        Mutable = mutable;
    }
    
    public LjsAstVariableDeclaration(string name, bool mutable)
    {
        Name = name;
        Mutable = mutable;
        Value = LjsAstEmptyNode.Instance;
    }
    
}