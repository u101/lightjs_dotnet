using App16.ALang.Ast;

namespace App16.LightJS.Compiler;

public interface ILjsCompilerNodeLookup
{
    bool ShouldProcess(IAstNode node);
}