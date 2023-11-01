using App16.ALang.Ast;
using App16.LightJS.Errors;

namespace App16.LightJS.Compiler;

public sealed class LjsControlTransferNodesProcessor : ILjsCompilerNodeProcessor
{
    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;

        switch (node)
        {
            case AstBreak _:

                if (!context.CanJumpToEnd)
                {
                    throw new LjsCompilerError("unexpected break statement", node.GetToken());
                }
                
                
                context.AddJumpToEnd(instructions.Count);
                instructions.Add(default);
                
                break;
            
            case AstContinue _:
                if (!context.CanJumpToStart)
                {
                    throw new LjsCompilerError("unexpected continue statement", node.GetToken());
                }
                
                context.AddJumpToStart(instructions.Count);
                instructions.Add(default);
                
                break;
        }
    }
}