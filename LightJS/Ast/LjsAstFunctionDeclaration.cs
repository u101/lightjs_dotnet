namespace LightJS.Ast;

public abstract class LjsAstFunctionDeclaration : ILjsAstNode
{
    public LjsAstFunctionDeclarationParameter[] Parameters { get; }
    public ILjsAstNode FunctionBody { get; }

    protected LjsAstFunctionDeclaration(LjsAstFunctionDeclarationParameter[] parameters, ILjsAstNode functionBody)
    {
        Parameters = parameters;
        FunctionBody = functionBody;
    }
}

public sealed class LjsAstAnonymousFunctionDeclaration : LjsAstFunctionDeclaration
{
    public LjsAstAnonymousFunctionDeclaration(
        LjsAstFunctionDeclarationParameter[] parameters, ILjsAstNode functionBody) : base(parameters, functionBody)
    { }
}

public sealed class LjsAstNamedFunctionDeclaration : LjsAstFunctionDeclaration
{
    public string Name { get; }

    public LjsAstNamedFunctionDeclaration(
        string name,
        LjsAstFunctionDeclarationParameter[] parameters, ILjsAstNode functionBody) : base(parameters, functionBody)
    {
        Name = name;
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