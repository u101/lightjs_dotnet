using App16.ALang.Ast;
using App16.ALang.Js.Ast;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsAnonymousFunctionDeclarationProcessor : ILjsCompilerNodeProcessor
{
    private readonly ILjsCompilerNodeProcessor _nodeProcessor;

    public LjsAnonymousFunctionDeclarationProcessor(ILjsCompilerNodeProcessor nodeProcessor)
    {
        _nodeProcessor = nodeProcessor;
    }

    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {

        if (node is not JsAnonymousFunctionDeclaration funcDeclaration) throw new Exception();

        var funcContext = context.AddAnonymousFunctionDeclaration();
        
        context.StartFunction(funcContext);
        
        LjsFunctionDeclarationProcessor.ProcessFunction(funcDeclaration, context, _nodeProcessor);
        
        context.EndFunction();
        
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;
        
        instructions.Add(new LjsInstruction(
            LjsInstructionCode.FuncRef, funcContext.FunctionData.FunctionIndex));
    }

}