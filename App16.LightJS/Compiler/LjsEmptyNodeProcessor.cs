using App16.ALang.Ast;

namespace App16.LightJS.Compiler;

public sealed class LjsEmptyNodeProcessor : ILjsCompilerNodeProcessor
{
    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        // do nothing
    }
}