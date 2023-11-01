using App16.ALang.Ast;
using App16.ALang.Js.Ast;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsFunctionCallProcessor : ILjsCompilerNodeProcessor
{
    private readonly ILjsCompilerNodeProcessor _nodeProcessor;

    public LjsFunctionCallProcessor(ILjsCompilerNodeProcessor nodeProcessor)
    {
        _nodeProcessor = nodeProcessor;
    }

    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;

        if (node is not JsFunctionCall functionCall) throw new Exception();
        
        var specifiedArgumentsCount = functionCall.Arguments.Count;
                
        foreach (var n in functionCall.Arguments.ChildNodes)
        {
            _nodeProcessor.ProcessNode(n, context);
        }
                
        _nodeProcessor.ProcessNode(functionCall.FunctionGetter, context);
                
        instructions.Add(new LjsInstruction(LjsInstructionCode.FuncCall, specifiedArgumentsCount));
    }
}