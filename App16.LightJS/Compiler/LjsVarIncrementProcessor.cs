using App16.ALang.Ast;
using App16.ALang.Js.Ast;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsVarIncrementProcessor : ILjsCompilerNodeProcessor
{
    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;

        if (node is not AstUnaryOperation unaryOperation) throw new Exception();

        var isPrefix = unaryOperation.IsPrefix;
        var isPostfix = !isPrefix; 
        var isPositive = unaryOperation.OperationInfo.OperatorId == JsUnaryOperationTypes.Increment;

        if (unaryOperation.Operand is not AstGetId getId) throw new Exception();

        instructions.Add(LjsCompileUtils.CreateVarLoadInstruction(getId.IdentifierName, context));
            
        if (isPostfix) instructions.Add(new LjsInstruction(LjsInstructionCode.Copy));
            
        instructions.Add(new LjsInstruction(LjsInstructionCode.ConstIntOne));
            
        instructions.Add(new LjsInstruction(isPositive ? LjsInstructionCode.Add : LjsInstructionCode.Sub));

        instructions.Add(LjsCompileUtils.CreateVarStoreInstruction(getId.IdentifierName, context, getId.GetToken()));
            
        if (isPostfix) instructions.Add(new LjsInstruction(LjsInstructionCode.Discard));

    }
}

public sealed class LjsVarIncrementLookup : ILjsCompilerNodeLookup
{
    public bool ShouldProcess(IAstNode node)
    {
        return node is AstUnaryOperation unaryOperation &&
               (unaryOperation.OperationInfo.OperatorId == JsUnaryOperationTypes.Increment ||
                unaryOperation.OperationInfo.OperatorId == JsUnaryOperationTypes.Decrement) &&
               unaryOperation.Operand is AstGetId;
    }
}