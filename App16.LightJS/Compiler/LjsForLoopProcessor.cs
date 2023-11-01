using App16.ALang.Ast;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsForLoopProcessor : ILjsCompilerNodeProcessor
{
    private readonly ILjsCompilerNodeProcessor _nodeProcessor;

    public LjsForLoopProcessor(ILjsCompilerNodeProcessor nodeProcessor)
    {
        _nodeProcessor = nodeProcessor;
    }

    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;

        if (node is not AstForLoop forLoop) throw new Exception();
        
        context.StartLocalBlock();
        
        _nodeProcessor.ProcessNode(forLoop.InitExpression, context);
        
        var loopStartIndex = instructions.Count;
        var loopConditionalJumpIndex = -1;
        
        if (forLoop.Condition != AstEmptyNode.Instance)
        {
            _nodeProcessor.ProcessNode(forLoop.Condition, context);
                
            loopConditionalJumpIndex = instructions.Count;
            instructions.Add(default);
        }
        
        context.PushPlaceholdersIndices();
        
        _nodeProcessor.ProcessNode(forLoop.Body, context);
        
        
        var loopIteratorIndex = instructions.Count;
        
        _nodeProcessor.ProcessNode(forLoop.IterationExpression, context);
        
        instructions.Add(new LjsInstruction(
            LjsInstructionCode.Jump, loopStartIndex));
        
        context.EndLocalBlock();
        
        var loopEndIndex = instructions.Count;

        if (loopConditionalJumpIndex != -1)
        {
            instructions.SetAt(new LjsInstruction(
                    LjsInstructionCode.JumpIfFalse, loopEndIndex), 
                loopConditionalJumpIndex);
        }
        
        var jumpToEndIndices = context.JumpToEndIndices;
        var jumpToStartIndices = context.JumpToStartIndices;
                
        foreach (var i in jumpToStartIndices)
        {
            instructions.SetAt(new LjsInstruction(
                LjsInstructionCode.Jump, loopIteratorIndex), i);
        }
                
        foreach (var i in jumpToEndIndices)
        {
            instructions.SetAt(new LjsInstruction(
                LjsInstructionCode.Jump, loopEndIndex), i);
        }
        
        context.PopPlaceholdersIndices();
    }
}