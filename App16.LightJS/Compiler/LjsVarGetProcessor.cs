using App16.ALang.Ast;

namespace App16.LightJS.Compiler;

public sealed class LjsVarGetProcessor : ILjsCompilerNodeProcessor
{
    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;

        if (node is not AstGetId getId) throw new Exception();

        instructions.Add(LjsCompileUtils.CreateVarLoadInstruction(getId.IdentifierName, context));

    }
}