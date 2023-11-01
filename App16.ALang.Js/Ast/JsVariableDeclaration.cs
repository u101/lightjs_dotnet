using App16.ALang.Ast;
using App16.ALang.Tokenizers;

namespace App16.ALang.Js.Ast;

public sealed class JsVariableDeclaration : AstNode
{
    public string Name { get; }
    public IAstNode Value { get; }
    public JsVariableKind VariableKind { get; }

    public JsVariableDeclaration(string name, IAstNode value, JsVariableKind variableKind, Token token = default):base(token)
    {
        Name = name;
        Value = value;
        VariableKind = variableKind;
    }
    
    public JsVariableDeclaration(string name, JsVariableKind variableKind, Token token = default):base(token)
    {
        Name = name;
        Value = AstEmptyNode.Instance;
        VariableKind = variableKind;
    }
    
}