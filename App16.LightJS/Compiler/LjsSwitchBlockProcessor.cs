using App16.ALang.Ast;
using App16.ALang.Js.Ast;
using App16.LightJS.Errors;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsSwitchBlockProcessor : ILjsCompilerNodeProcessor
{
    private readonly ILjsCompilerNodeProcessor _nodeProcessor;

    public LjsSwitchBlockProcessor(ILjsCompilerNodeProcessor nodeProcessor)
    {
        _nodeProcessor = nodeProcessor;
    }

    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;

        if (node is not JsSwitchBlock switchBlock) throw new Exception();

        if (switchBlock.Body == AstEmptyNode.Instance) return;

        var body = switchBlock.Body;

        context.StartLocalBlock();
        context.PushPlaceholdersIndices();
        
        var switchBreakIndices = context.JumpToEndIndices;
        var caseFalseJumpIndices = LjsCompileUtils.GetTemporaryIntList();
        var caseTrueJumpIndices = LjsCompileUtils.GetTemporaryIntList();

        var defaultIsReached = false;
        var blockEnd = false;
        var prevNode = AstEmptyNode.Instance;
        
        

        for (var i = 0; i < body.Count && !blockEnd; i++)
        {
            var n = body[i];
            switch (n)
            {
                case JsSwitchCase switchCase:
                    if (defaultIsReached)
                        throw new LjsCompilerError("unexpected switch case after default");

                    if (prevNode is JsSwitchCase)
                    {
                        caseTrueJumpIndices.Add(instructions.Count);
                        instructions.Add(default);
                    }

                    SetFalseJumps(caseFalseJumpIndices, instructions.Count, instructions);
                    caseFalseJumpIndices.Clear();

                    _nodeProcessor.ProcessNode(switchBlock.Expression, context);
                    _nodeProcessor.ProcessNode(switchCase.Value, context);
                    
                    instructions.Add(new LjsInstruction(LjsInstructionCode.Eq));

                    caseFalseJumpIndices.Add(instructions.Count);
                    instructions.Add(default);
                    break;

                case AstBreak _:
                    if (defaultIsReached)
                    {
                        blockEnd = true;
                    }
                    else
                    {
                        SetJumps(caseTrueJumpIndices, instructions.Count, instructions);
                        caseTrueJumpIndices.Clear();

                        switchBreakIndices.Add(instructions.Count);
                        instructions.Add(default);
                    }

                    break;

                case JsSwitchDefault _:
                    defaultIsReached = true;

                    SetFalseJumps(caseFalseJumpIndices, instructions.Count, instructions);
                    caseFalseJumpIndices.Clear();

                    SetJumps(caseTrueJumpIndices, instructions.Count, instructions);
                    caseTrueJumpIndices.Clear();

                    break;

                default:

                    SetJumps(caseTrueJumpIndices, instructions.Count, instructions);
                    caseTrueJumpIndices.Clear();

                    _nodeProcessor.ProcessNode(n, context);

                    break;
            }

            prevNode = n;
        }

        SetJumps(caseTrueJumpIndices, instructions.Count, instructions);
        SetJumps(switchBreakIndices, instructions.Count, instructions);
        SetFalseJumps(caseFalseJumpIndices, instructions.Count, instructions);

        LjsCompileUtils.ReleaseTemporaryIntList(caseTrueJumpIndices);
        LjsCompileUtils.ReleaseTemporaryIntList(caseFalseJumpIndices);
        
        context.EndLocalBlock();
        
        context.PopPlaceholdersIndices();
    }
    
    private static void SetFalseJumps(
        IReadOnlyList<int> indices, int jumpIndex, LjsInstructionsList instructionsList)
    {
        for (var i = 0; i < indices.Count; i++)
        {
            var j = indices[i];
            instructionsList.SetAt(new LjsInstruction(LjsInstructionCode.JumpIfFalse, jumpIndex), j);
        }
    }
    
    private static void SetJumps(
        IReadOnlyList<int> indices, int jumpIndex, LjsInstructionsList instructionsList)
    {
        for (var i = 0; i < indices.Count; i++)
        {
            var j = indices[i];
            instructionsList.SetAt(new LjsInstruction(LjsInstructionCode.Jump, jumpIndex), j);
        }
    }
    
}