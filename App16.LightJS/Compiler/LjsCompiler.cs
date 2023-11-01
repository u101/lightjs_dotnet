using App16.ALang.Ast;
using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

public sealed class LjsCompiler
{
    private readonly ILjsCompilerNodeProcessor _nodeProcessor;
    private readonly IAstNode _astModel;
    
    public LjsCompiler(IAstNode astModel, ILjsCompilerNodeProcessor nodeProcessor)
    {
        _astModel = astModel;
        _nodeProcessor = nodeProcessor;
    }

    public LjsProgram Compile()
    {
        var mainFunc = new LjsCompilerFunctionData(0);
        
        var functionContext = new LjsCompilerFunctionContext(mainFunc);

        var compilerContext = new LjsCompilerContext();
        
        compilerContext.StartFunction(functionContext);
        
        _nodeProcessor.ProcessNode(_astModel, compilerContext);

        mainFunc.Instructions.Add(
            new LjsInstruction(LjsInstructionCode.Halt));
        
        compilerContext.EndFunction();

        var functions = compilerContext.AllFunctions.Select(
            f => new LjsFunctionData(
                f.FunctionData.FunctionIndex,
                f.FunctionData.Instructions.Instructions.ToArray(), 
                f.FunctionData.FunctionArgs.ToArray(), 
                f.Locals.Pointers.ToArray()
        )).ToArray();
        
        return new LjsProgram(
            compilerContext.Constants, functions, compilerContext.TopLevelNamedFunctions);
    }
}