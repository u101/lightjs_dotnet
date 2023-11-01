using App16.ALang.Ast;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsSqbPropertyAssignProcessor : ILjsCompilerNodeProcessor
{
    private readonly ILjsCompilerNodeProcessor _nodeProcessor;

    public LjsSqbPropertyAssignProcessor(ILjsCompilerNodeProcessor nodeProcessor)
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

        if (binaryOperation.LeftOperand is not AstGetSquareBracketsProp sqbProp) throw new Exception();
        
        if (assignMode == LjsCompilerAssignMode.Normal)
        {
            _nodeProcessor.ProcessNode(assignmentExp, context);
        }
        else
        {
            _nodeProcessor.ProcessNode(sqbProp.PropertySource, context);
            _nodeProcessor.ProcessNode(sqbProp.Expression, context);
            
            instructions.Add(new LjsInstruction(LjsInstructionCode.GetProp));

            _nodeProcessor.ProcessNode(assignmentExp, context);

            instructions.Add(new LjsInstruction(LjsCompileUtils.GetComplexAssignmentOpCode(assignMode)));
        }

        _nodeProcessor.ProcessNode(sqbProp.PropertySource, context);
        _nodeProcessor.ProcessNode(sqbProp.Expression, context);

        instructions.Add(new LjsInstruction(LjsInstructionCode.SetProp));
    }
}

public sealed class LjsSqbPropertyAssignLookup : ILjsCompilerNodeLookup
{
    public bool ShouldProcess(IAstNode node)
    {
        return node is AstBinaryOperation binaryOperation &&
               LjsCompileUtils.IsAssignOperation(binaryOperation.OperationInfo.OperatorId) &&
               binaryOperation.LeftOperand is AstGetSquareBracketsProp;
    }
}