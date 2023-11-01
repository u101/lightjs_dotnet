using App16.ALang.Ast;
using App16.ALang.Js.Ast;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsDotPropertyIncrementProcessor : ILjsCompilerNodeProcessor
{
    private readonly ILjsCompilerNodeProcessor _nodeProcessor;

    public LjsDotPropertyIncrementProcessor(ILjsCompilerNodeProcessor nodeProcessor)
    {
        _nodeProcessor = nodeProcessor;
    }
    
    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;

        if (node is not AstUnaryOperation unaryOperation) throw new Exception();

        var isPrefix = unaryOperation.IsPrefix;
        var isPostfix = !isPrefix; 
        var isPositive = unaryOperation.OperationInfo.OperatorId == JsUnaryOperationTypes.Increment;

        if (unaryOperation.Operand is not AstGetDotProperty getDotProperty) throw new Exception();
        
        // load property to stack
        _nodeProcessor.ProcessNode(getDotProperty.PropertySource, context);
        instructions.Add(LjsCompileUtils.GetStringConstInstruction(getDotProperty.PropertyName, context.Constants));
        instructions.Add(new LjsInstruction(LjsInstructionCode.GetProp));
        // :end
            
        if (isPostfix) instructions.Add(new LjsInstruction(LjsInstructionCode.Copy));
            
        instructions.Add(new LjsInstruction(LjsInstructionCode.ConstIntOne));
            
        instructions.Add(new LjsInstruction(isPositive ? LjsInstructionCode.Add : LjsInstructionCode.Sub));
        
        // write property
        _nodeProcessor.ProcessNode(getDotProperty.PropertySource, context);
        
        instructions.Add(LjsCompileUtils.GetStringConstInstruction(getDotProperty.PropertyName, context.Constants));

        instructions.Add(new LjsInstruction(LjsInstructionCode.SetProp));
        // :end
            
        if (isPostfix) instructions.Add(new LjsInstruction(LjsInstructionCode.Discard));
    }
}

public sealed class LjsDotPropertyIncrementLookup : ILjsCompilerNodeLookup
{
    public bool ShouldProcess(IAstNode node)
    {
        return node is AstUnaryOperation unaryOperation &&
               (unaryOperation.OperationInfo.OperatorId == JsUnaryOperationTypes.Increment ||
                unaryOperation.OperationInfo.OperatorId == JsUnaryOperationTypes.Decrement) &&
               unaryOperation.Operand is AstGetDotProperty;
    }
}