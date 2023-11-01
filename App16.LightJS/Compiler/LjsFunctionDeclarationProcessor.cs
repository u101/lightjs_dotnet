using App16.ALang.Ast;
using App16.ALang.Js.Ast;
using App16.LightJS.Errors;
using App16.LightJS.Program;
using App16.LightJS.Runtime;

namespace App16.LightJS.Compiler;

public static class LjsFunctionDeclarationProcessor
{
    public static void ProcessFunction(
        JsFunctionDeclaration functionDeclaration,
        LjsCompilerContext context,
        ILjsCompilerNodeProcessor nodeProcessor)
    {
        var parameters = functionDeclaration.Parameters;
        var functionContext = context.CurrentFunction;
        
        var instructions = functionContext.FunctionData.Instructions;

        for (var i = 0; i < parameters.Length; i++)
        {
            var p = parameters[i];
            var defaultValue = GetFunctionParameterDefaultValue(p.DefaultValue);

            AssertArgumentNameIsUniq(p.Name, functionContext);

            
            functionContext.FunctionData.FunctionArgs.Add(
                new LjsFunctionArgument(p.Name, defaultValue));

            functionContext.Locals.Add(p.Name, LjsLocalVarKind.Argument);
        }
        
        nodeProcessor.ProcessNode(functionDeclaration.FunctionBody, context);

        
        
        if (instructions.Count == 0 ||
            instructions.LastInstruction.Code != LjsInstructionCode.Return)
        {
            instructions.Add(new LjsInstruction(LjsInstructionCode.ConstUndef));
            instructions.Add(new LjsInstruction(LjsInstructionCode.Return));
        }
    }
    
    private static LjsObject GetFunctionParameterDefaultValue(IAstNode node) => node switch
    {
        AstNull _ => LjsObject.Null,
        JsUndefined _ => LjsObject.Undefined,
        AstValueLiteral<int> i => new LjsInteger(i.Value),
        AstValueLiteral<double> i => new LjsDouble(i.Value),
        AstValueLiteral<string> i => new LjsString(i.Value),
        AstValueLiteral<bool> i => i.Value ? LjsBoolean.True : LjsBoolean.False,
        _ => LjsObject.Undefined
    };
    
    private static void AssertArgumentNameIsUniq(
        string argumentName,
        LjsCompilerFunctionContext context)
    {
        if (context.Locals.Has(argumentName))
        {
            throw new LjsCompilerError($"duplicate argument name {argumentName}");
        }
    }
}