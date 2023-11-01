using App16.ALang.Ast;
using App16.ALang.Js.Ast;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsArrayLiteralProcessor : ILjsCompilerNodeProcessor
{
    private readonly ILjsCompilerNodeProcessor _nodeProcessor;

    public LjsArrayLiteralProcessor(ILjsCompilerNodeProcessor nodeProcessor)
    {
        _nodeProcessor = nodeProcessor;
    }

    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;

        if (node is not JsArrayLiteral arrayLiteral) throw new Exception();
        
        foreach (var e in arrayLiteral.ChildNodes)
        {
            _nodeProcessor.ProcessNode(e, context);
        }
                
        instructions.Add(new LjsInstruction(LjsInstructionCode.NewArray, arrayLiteral.Count));
    }
}