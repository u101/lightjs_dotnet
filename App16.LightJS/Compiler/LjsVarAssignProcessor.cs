using App16.ALang.Ast;
using App16.LightJS.Errors;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsVarAssignProcessor : ILjsCompilerNodeProcessor
{
    private readonly ILjsCompilerNodeProcessor _nodeProcessor;

    public LjsVarAssignProcessor(ILjsCompilerNodeProcessor nodeProcessor)
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

        if (binaryOperation.LeftOperand is not AstGetId getId) throw new Exception();

        var varName = getId.IdentifierName;
        var isLocal = context.HasLocal(varName);
                
        if (!isLocal && context.HasFunctionWithName(varName))
        {
            throw new LjsCompilerError($"illegal named function assign {varName}", node.GetToken());
        }
                
        if (assignMode == LjsCompilerAssignMode.Normal)
        {
            _nodeProcessor.ProcessNode(assignmentExp, context);
        }
        else
        {
            instructions.Add(LjsCompileUtils.CreateVarLoadInstruction(varName, context));
                    
            _nodeProcessor.ProcessNode(assignmentExp, context);
                    
            instructions.Add(new LjsInstruction(
                LjsCompileUtils.GetComplexAssignmentOpCode(assignMode)));
        }
        
        instructions.Add(LjsCompileUtils.CreateVarStoreInstruction(varName, context, getId.GetToken()));
    }
}

public sealed class LjsVarAssignLookup : ILjsCompilerNodeLookup
{
    public bool ShouldProcess(IAstNode node)
    {
        return node is AstBinaryOperation binaryOperation &&
               LjsCompileUtils.IsAssignOperation(binaryOperation.OperationInfo.OperatorId) &&
               binaryOperation.LeftOperand is AstGetId;
    }
}