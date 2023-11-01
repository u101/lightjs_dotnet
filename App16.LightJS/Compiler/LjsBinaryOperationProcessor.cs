using App16.ALang.Ast;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsBinaryOperationProcessor : ILjsCompilerNodeProcessor
{
    private readonly ILjsCompilerNodeProcessor _operandsProcessor;

    public LjsBinaryOperationProcessor(ILjsCompilerNodeProcessor operandsProcessor)
    {
        _operandsProcessor = operandsProcessor;
    }
    
    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;

        if (node is not AstBinaryOperation binaryOperation) throw new Exception();
        
        var operatorId = binaryOperation.OperationInfo.OperatorId;
        
        _operandsProcessor.ProcessNode(binaryOperation.LeftOperand, context);
        _operandsProcessor.ProcessNode(binaryOperation.RightOperand, context);
                
        instructions.Add(new LjsInstruction(LjsCompileUtils.GetBinaryOpCode(operatorId)));

    }
}