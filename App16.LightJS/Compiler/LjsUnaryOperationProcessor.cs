using App16.ALang.Ast;
using App16.ALang.Js.Ast;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsUnaryOperationProcessor : ILjsCompilerNodeProcessor
{
    
    private readonly ILjsCompilerNodeProcessor _nodeProcessor;

    public LjsUnaryOperationProcessor(ILjsCompilerNodeProcessor nodeProcessor)
    {
        _nodeProcessor = nodeProcessor;
    }
    
    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;

        if (node is not AstUnaryOperation unaryOperation) throw new Exception();
        
        var operatorId = unaryOperation.OperationInfo.OperatorId;

        switch (operatorId)
        {
            case JsUnaryOperationTypes.UnaryPlus:
                // just skip, because unary plus does nothing
                _nodeProcessor.ProcessNode(unaryOperation.Operand, context);
                break;

            case JsUnaryOperationTypes.UnaryMinus:
                _nodeProcessor.ProcessNode(unaryOperation.Operand, context);
                instructions.Add(new LjsInstruction(LjsInstructionCode.Minus));
                break;
            
            case JsUnaryOperationTypes.LogicalNot:
                _nodeProcessor.ProcessNode(unaryOperation.Operand, context);
                instructions.Add(new LjsInstruction(LjsInstructionCode.Not));
                break;

            case JsUnaryOperationTypes.BitNot:
                _nodeProcessor.ProcessNode(unaryOperation.Operand, context);
                instructions.Add(new LjsInstruction(LjsInstructionCode.BitNot));
                break;
            
            default:
                throw new Exception();
        }
    }
}

public sealed class LjsUnaryOperationLookup : ILjsCompilerNodeLookup
{
    public bool ShouldProcess(IAstNode node)
    {
        return node is AstUnaryOperation unaryOperation &&
               (unaryOperation.OperationInfo.OperatorId == JsUnaryOperationTypes.UnaryPlus ||
                unaryOperation.OperationInfo.OperatorId == JsUnaryOperationTypes.UnaryMinus ||
                unaryOperation.OperationInfo.OperatorId == JsUnaryOperationTypes.LogicalNot ||
                 unaryOperation.OperationInfo.OperatorId == JsUnaryOperationTypes.BitNot);
    }
}