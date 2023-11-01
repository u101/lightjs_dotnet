using App16.ALang.Ast;
using App16.ALang.Tokenizers;

namespace App16.ALang.Js.Ast;

public sealed class JsObjectLiteral : AstSequence<JsObjectLiteralProperty>, IAstValueNode
{
    public JsObjectLiteral(Token token) : base(token) { }

    public JsObjectLiteral(IEnumerable<JsObjectLiteralProperty> nodes) : base(nodes)
    { }

    public JsObjectLiteral(params JsObjectLiteralProperty[] nodes) : base(nodes)
    { }
}

public sealed class JsObjectLiteralProperty : AstNode
{
    public string Name { get; }
    public IAstNode Value { get; }

    public JsObjectLiteralProperty(string name, IAstNode value, Token token = default) : base(token)
    {
        Name = name;
        Value = value;
    }
}