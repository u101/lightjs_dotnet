using App16.ALang.Ast;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsDotPropertyAssignProcessor : ILjsCompilerNodeProcessor
{
    private readonly ILjsCompilerNodeProcessor _nodeProcessor;

    public LjsDotPropertyAssignProcessor(ILjsCompilerNodeProcessor nodeProcessor)
    {
        _nodeProcessor = nodeProcessor;
    }

    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;

        if (node is not AstBinaryOperation binaryOperation) throw new Exception();

        var operatorId = binaryOperation.OperationInfo.OperatorId;
        var assignmentExp = binaryOperation.RightOperand;
        var assignMode = LjsCompileUtils.GetAssignMode(operatorId);

        if (binaryOperation.LeftOperand is not AstGetDotProperty getDotProperty) throw new Exception();
        
        if (assignMode == LjsCompilerAssignMode.Normal)
        {
            _nodeProcessor.ProcessNode(assignmentExp, context);
        }
        else
        {
            _nodeProcessor.ProcessNode(getDotProperty.PropertySource, context);

            instructions.Add(LjsCompileUtils.GetStringConstInstruction(
                getDotProperty.PropertyName, context.Constants));
                
            instructions.Add(new LjsInstruction(LjsInstructionCode.GetProp));

            _nodeProcessor.ProcessNode(assignmentExp, context);

            instructions.Add(new LjsInstruction(
                LjsCompileUtils.GetComplexAssignmentOpCode(assignMode)));
        }

        _nodeProcessor.ProcessNode(getDotProperty.PropertySource, context);
        
        instructions.Add(LjsCompileUtils.GetStringConstInstruction(
            getDotProperty.PropertyName, context.Constants));

        instructions.Add(new LjsInstruction(LjsInstructionCode.SetProp));
    }
}

public sealed class LjsDotPropertyAssignLookup : ILjsCompilerNodeLookup
{
    public bool ShouldProcess(IAstNode node)
    {
        return node is AstBinaryOperation binaryOperation &&
               LjsCompileUtils.IsAssignOperation(binaryOperation.OperationInfo.OperatorId) &&
               binaryOperation.LeftOperand is AstGetDotProperty;
    }
}