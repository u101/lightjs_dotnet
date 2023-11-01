using App16.ALang.Ast;

namespace App16.LightJS.Compiler;

public interface ILjsCompilerNodeProcessor
{
    void ProcessNode(IAstNode node, LjsCompilerContext context);
}