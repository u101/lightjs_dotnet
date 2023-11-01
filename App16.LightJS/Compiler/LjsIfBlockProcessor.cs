using App16.ALang.Ast;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsIfBlockProcessor : ILjsCompilerNodeProcessor
{
    private readonly ILjsCompilerNodeProcessor _nodeProcessor;

    public LjsIfBlockProcessor(ILjsCompilerNodeProcessor nodeProcessor)
    {
        _nodeProcessor = nodeProcessor;
    }

    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;

        if (node is not AstIfBlock ifBlock) throw new Exception();

        var jumpToTheEndPlaceholdersIndices = LjsCompileUtils.GetTemporaryIntList();
        
        _nodeProcessor.ProcessNode(ifBlock.If.Condition, context);

        // if condition value is false, jump to skip all instructions inside if { .. }
        var ifFalseJumpInstructionIndex = instructions.Count;
        instructions.Add(default);
        
        context.StartLocalBlock();
        
        _nodeProcessor.ProcessNode(ifBlock.If.Expression, context);
        
        context.EndLocalBlock();

        jumpToTheEndPlaceholdersIndices.Add(instructions.Count);
        instructions.Add(default);
        
        instructions.SetAt(new LjsInstruction(
            LjsInstructionCode.JumpIfFalse, instructions.Count), ifFalseJumpInstructionIndex);

        if (ifBlock.ElseIfs.Count != 0)
        {
            foreach (var alternative in ifBlock.ElseIfs)
            {
                _nodeProcessor.ProcessNode(alternative.Condition, context);
                
                ifFalseJumpInstructionIndex = instructions.Count;
                instructions.Add(default);
                
                context.StartLocalBlock();
        
                _nodeProcessor.ProcessNode(alternative.Expression, context);
        
                context.EndLocalBlock();
                
                jumpToTheEndPlaceholdersIndices.Add(instructions.Count);
                instructions.Add(default);
                
                instructions.SetAt(new LjsInstruction(
                    LjsInstructionCode.JumpIfFalse, instructions.Count), ifFalseJumpInstructionIndex);
                
            }
        }

        if (ifBlock.Else != AstEmptyNode.Instance)
        {
            context.StartLocalBlock();
        
            _nodeProcessor.ProcessNode(ifBlock.Else, context);
        
            context.EndLocalBlock();
        }

        var ifBlockEndIndex = instructions.Count;

        foreach (var i in jumpToTheEndPlaceholdersIndices)
        {
            instructions.SetAt(new LjsInstruction(LjsInstructionCode.Jump, ifBlockEndIndex), i);
        }
        
        LjsCompileUtils.ReleaseTemporaryIntList(jumpToTheEndPlaceholdersIndices);
        
                
    }
}