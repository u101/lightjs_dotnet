using App16.ALang.Ast;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsSqbPropertyGetProcessor : ILjsCompilerNodeProcessor
{
    private readonly ILjsCompilerNodeProcessor _nodeProcessor;

    public LjsSqbPropertyGetProcessor(ILjsCompilerNodeProcessor nodeProcessor)
    {
        _nodeProcessor = nodeProcessor;
    }

    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;
        
        if (node is not AstGetSquareBracketsProp sqbProp) throw new Exception();
        
        _nodeProcessor.ProcessNode(sqbProp.PropertySource, context);
        _nodeProcessor.ProcessNode(sqbProp.Expression, context);
        instructions.Add(new LjsInstruction(LjsInstructionCode.GetProp));
    }
}