using App16.ALang.Ast;
using App16.ALang.Tokenizers;

namespace App16.ALang.Js.Ast;

public abstract class JsFunctionDeclaration : AstNode
{
    public JsFunctionDeclarationParameter[] Parameters { get; }
    public IAstNode FunctionBody { get; }

    protected JsFunctionDeclaration(
        JsFunctionDeclarationParameter[] parameters, IAstNode functionBody, Token token = default) : base(token)
    {
        Parameters = parameters;
        FunctionBody = functionBody;
    }
}

public sealed class JsAnonymousFunctionDeclaration : JsFunctionDeclaration
{
    public JsAnonymousFunctionDeclaration(
        JsFunctionDeclarationParameter[] parameters, IAstNode functionBody, Token token = default) : 
        base(parameters, functionBody, token)
    { }
}

public sealed class JsNamedFunctionDeclaration : JsFunctionDeclaration
{
    public string Name { get; }

    public JsNamedFunctionDeclaration(
        string name,
        JsFunctionDeclarationParameter[] parameters, IAstNode functionBody, Token token = default) : 
        base(parameters, functionBody, token)
    {
        Name = name;
    }
}

public sealed class JsFunctionDeclarationParameter
{
    public string Name { get; }
    public IAstNode DefaultValue { get; }
    
    public Token Token { get; }

    public JsFunctionDeclarationParameter(string name, IAstNode defaultValue, Token token = default)
    {
        Name = name;
        DefaultValue = defaultValue;
        Token = token;
    }
    
    public JsFunctionDeclarationParameter(string name, Token token = default)
    {
        Name = name;
        DefaultValue = AstEmptyNode.Instance;
        Token = token;
    }
}