namespace LightJS.Ast;

public class LjsAstFunctionDeclaration : ILjsAstNode
{
    public LjsAstFunctionDeclarationParameter[] Parameters { get; }
    public ILjsAstNode FunctionBody { get; }

    public LjsAstFunctionDeclaration(LjsAstFunctionDeclarationParameter[] parameters, ILjsAstNode functionBody)
    {
        Parameters = parameters;
        FunctionBody = functionBody;
    }
    
}

public class LjsAstFunctionDeclarationParameter
{
    public string Name { get; }
    public ILjsAstNode DefaultValue { get; }

    public LjsAstFunctionDeclarationParameter(string name, ILjsAstNode defaultValue)
    {
        Name = name;
        DefaultValue = defaultValue;
    }
    
    public LjsAstFunctionDeclarationParameter(string name)
    {
        Name = name;
        DefaultValue = LjsAstEmptyNode.Instance;
    }
}