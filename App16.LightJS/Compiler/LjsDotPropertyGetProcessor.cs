using App16.ALang.Ast;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsDotPropertyGetProcessor : ILjsCompilerNodeProcessor
{
    private readonly ILjsCompilerNodeProcessor _nodeProcessor;

    public LjsDotPropertyGetProcessor(ILjsCompilerNodeProcessor nodeProcessor)
    {
        _nodeProcessor = nodeProcessor;
    }

    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;
        
        if (node is not AstGetDotProperty getDotProperty) throw new Exception();
        
        _nodeProcessor.ProcessNode(getDotProperty.PropertySource, context);
        instructions.Add(LjsCompileUtils.GetStringConstInstruction(getDotProperty.PropertyName, context.Constants));
        instructions.Add(new LjsInstruction(LjsInstructionCode.GetProp));
    }
}