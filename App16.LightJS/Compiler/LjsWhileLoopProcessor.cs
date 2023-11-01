using App16.ALang.Ast;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsWhileLoopProcessor : ILjsCompilerNodeProcessor
{
    private readonly ILjsCompilerNodeProcessor _nodeProcessor;

    public LjsWhileLoopProcessor(ILjsCompilerNodeProcessor nodeProcessor)
    {
        _nodeProcessor = nodeProcessor;
    }

    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;
        
        if (node is not AstWhileLoop whileLoop) throw new Exception();

        var startIndex = instructions.Count;
        
        _nodeProcessor.ProcessNode(whileLoop.Condition, context);

        // if condition value is false, jump to skip all instructions inside if { .. }
        var ifFalseJumpInstructionIndex = instructions.Count;
        instructions.Add(default);
        
        
        context.StartLocalBlock();
        context.PushPlaceholdersIndices();
        
        _nodeProcessor.ProcessNode(whileLoop.Body, context);
        
        instructions.Add(new LjsInstruction(LjsInstructionCode.Jump, startIndex));
        
        context.EndLocalBlock();
        
        var endIndex = instructions.Count;
        
        instructions.SetAt(
            new LjsInstruction(LjsInstructionCode.JumpIfFalse, endIndex),
            ifFalseJumpInstructionIndex);

        var jumpToEndIndices = context.JumpToEndIndices;
        var jumpToStartIndices = context.JumpToStartIndices;

        foreach (var i in jumpToEndIndices)
        {
            instructions.SetAt(new LjsInstruction(LjsInstructionCode.Jump, endIndex), i);
        }
        
        foreach (var i in jumpToStartIndices)
        {
            instructions.SetAt(new LjsInstruction(LjsInstructionCode.Jump, startIndex), i);
        }
        
        context.PopPlaceholdersIndices();
    }

}