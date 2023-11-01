using App16.ALang.Ast;
using App16.ALang.Js.Ast;
using App16.LightJS.Errors;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsVarDeclarationProcessor : ILjsCompilerNodeProcessor
{
    private readonly ILjsCompilerNodeProcessor _nodeProcessor;

    public LjsVarDeclarationProcessor(ILjsCompilerNodeProcessor nodeProcessor)
    {
        _nodeProcessor = nodeProcessor;
    }

    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;

        if (node is not JsVariableDeclaration variableDeclaration) throw new Exception();
        
        var localVarKind = LjsCompileUtils.GetVarKind(variableDeclaration.VariableKind);

        AssertVariableNameIsUniq(variableDeclaration, context);
                
                
        var varIndex = context.AddLocal(
            variableDeclaration.Name, 
            localVarKind);

        if (variableDeclaration.Value != AstEmptyNode.Instance)
        {
            _nodeProcessor.ProcessNode(variableDeclaration.Value, context);
                    
            instructions.Add(new LjsInstruction(LjsInstructionCode.VarStore, varIndex));
            
            instructions.Add(new LjsInstruction(LjsInstructionCode.Discard));
        }
        else
        {
            if (localVarKind == LjsLocalVarKind.Const)
                throw new LjsCompilerError($"const {variableDeclaration.Name} must be initialized");
        }
    }
    
    private static void AssertVariableNameIsUniq(
        JsVariableDeclaration variableDeclaration, LjsCompilerContext context)
    {
        var varKind = LjsCompileUtils.GetVarKind(variableDeclaration.VariableKind);

        if (varKind == LjsLocalVarKind.Let)
        {
            if (context.HasLocal(variableDeclaration.Name, false))
            {
                throw new LjsCompilerError($"duplicate {varKind} name {variableDeclaration.Name}");
            }
        }
        else
        {
            if (context.HasLocal(variableDeclaration.Name, true))
            {
                throw new LjsCompilerError($"duplicate {varKind} name {variableDeclaration.Name}");
            }
        }

        
    }
}