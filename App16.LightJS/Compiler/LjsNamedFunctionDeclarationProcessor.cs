using App16.ALang.Ast;
using App16.ALang.Js.Ast;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsNamedFunctionDeclarationProcessor : ILjsCompilerNodeProcessor
{
    private readonly ILjsCompilerNodeProcessor _nodeProcessor;

    public LjsNamedFunctionDeclarationProcessor(ILjsCompilerNodeProcessor nodeProcessor)
    {
        _nodeProcessor = nodeProcessor;
    }

    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {

        if (node is not JsNamedFunctionDeclaration funcDeclaration) throw new Exception();

        LjsCompilerFunctionContext funcContext;

        if (context.HasFunctionWithName(funcDeclaration.Name))
        {
            var funcIndex = context.GetFunctionIndex(funcDeclaration.Name);
            funcContext = context.GetFunctionContext(funcIndex);
        }
        else
        {
            funcContext = context.AddNamedFunctionDeclaration(funcDeclaration.Name);
        }

        context.StartFunction(funcContext);
        context.PushNamedFunctionsMap();
        
        LjsFunctionDeclarationProcessor.ProcessFunction(funcDeclaration, context, _nodeProcessor);
        
        context.PopNamedFunctionsMap();
        context.EndFunction();
        
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;
        
        instructions.Add(new LjsInstruction(
            LjsInstructionCode.FuncRef, funcContext.FunctionData.FunctionIndex));
    }

}