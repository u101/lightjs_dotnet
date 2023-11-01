using App16.ALang.Ast;
using App16.ALang.Tokenizers;

namespace App16.ALang.Js.Ast;

public sealed class JsUndefined : AstNode
{
    public JsUndefined(Token token = default):base(token) {}
    
}