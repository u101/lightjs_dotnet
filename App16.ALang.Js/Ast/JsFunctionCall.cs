using App16.ALang.Ast;
using App16.ALang.Tokenizers;

namespace App16.ALang.Js.Ast;

public sealed class JsFunctionCall : AstNode, IAstValueNode
{
    public IAstNode FunctionGetter { get; }

    public JsFunctionCallArguments Arguments { get; }

    public JsFunctionCall(IAstNode functionGetter, JsFunctionCallArguments arguments, Token token = default) : base(token)
    {
        FunctionGetter = functionGetter;
        Arguments = arguments;
    }
}