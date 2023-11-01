using App16.ALang.Ast;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsReturnProcessor : ILjsCompilerNodeProcessor
{
    
    private readonly ILjsCompilerNodeProcessor _nodeProcessor;

    public LjsReturnProcessor(ILjsCompilerNodeProcessor nodeProcessor)
    {
        _nodeProcessor = nodeProcessor;
    }

    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;

        if (node is not AstReturn astReturn) throw new Exception();
        
        if (astReturn.Expression != AstEmptyNode.Instance)
        {
            _nodeProcessor.ProcessNode(astReturn.Expression, context);
        }
        else
        {
            instructions.Add(new LjsInstruction(LjsInstructionCode.ConstUndef));
        }
                
        instructions.Add(new LjsInstruction(LjsInstructionCode.Return));
    }

}