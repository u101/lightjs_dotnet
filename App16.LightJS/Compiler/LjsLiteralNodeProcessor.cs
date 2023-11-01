using App16.ALang.Ast;
using App16.ALang.Js.Ast;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsLiteralNodeProcessor : ILjsCompilerNodeProcessor
{
    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        var functionContext = context.CurrentFunction;
        var instructions = functionContext.FunctionData.Instructions;

        switch (node)
        {
            case AstValueLiteral<int> lit:
                instructions.Add(
                    LjsCompileUtils.GetIntLiteralInstruction(lit.Value));
                break;
            
            case AstValueLiteral<double> lit:
                instructions.Add(
                    LjsCompileUtils.GetDoubleLiteralInstruction(lit.Value, context.Constants));
                break;
            
            case AstValueLiteral<string> lit:
                instructions.Add(
                    LjsCompileUtils.GetStringConstInstruction(lit.Value, context.Constants));
                break;
            
            case AstNull _:
                instructions.Add(new LjsInstruction(LjsInstructionCode.ConstNull));
                break;
            
            case JsUndefined _:
                instructions.Add(new LjsInstruction(LjsInstructionCode.ConstUndef));
                break;
            
            case AstValueLiteral<bool> lit:
                instructions.Add(new LjsInstruction(
                    lit.Value ? LjsInstructionCode.ConstTrue : LjsInstructionCode.ConstFalse));
                break;
            
            case AstGetThis _:
                instructions.Add(new LjsInstruction(LjsInstructionCode.GetThis));
                break;
            
            default:
                throw new Exception();
                
        }

    }
    
    
}

public sealed class LjsLiteralNodeLookup : ILjsCompilerNodeLookup
{
    public bool ShouldProcess(IAstNode node)
    {
        return node is AstValueLiteral<int> || 
               node is AstValueLiteral<double> ||
               node is AstValueLiteral<string> ||
               node is AstValueLiteral<bool> ||
               node is AstNull ||
               node is JsUndefined ||
               node is AstGetThis;
    }
}