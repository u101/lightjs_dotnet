using App16.ALang.Ast;
using App16.ALang.Js.Ast;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsObjectLiteralProcessor : ILjsCompilerNodeProcessor
{
    private readonly ILjsCompilerNodeProcessor _nodeProcessor;

    public LjsObjectLiteralProcessor(ILjsCompilerNodeProcessor nodeProcessor)
    {
        _nodeProcessor = nodeProcessor;
    }

    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;

        if (node is not JsObjectLiteral objectLiteral) throw new Exception();
        
        foreach (var prop in objectLiteral.ChildNodes)
        {
            _nodeProcessor.ProcessNode(prop.Value, context);
            
            instructions.Add(LjsCompileUtils.GetStringConstInstruction(
                prop.Name, context.Constants));
        }
                
        instructions.Add(new LjsInstruction(
            LjsInstructionCode.NewDictionary, objectLiteral.Count));
    }
}