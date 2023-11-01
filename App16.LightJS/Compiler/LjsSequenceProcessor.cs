using App16.ALang.Ast;
using App16.ALang.Js.Ast;

namespace App16.LightJS.Compiler;

public sealed class LjsSequenceProcessor : ILjsCompilerNodeProcessor
{
    
    private readonly ILjsCompilerNodeProcessor _nodeProcessor;

    public LjsSequenceProcessor(ILjsCompilerNodeProcessor nodeProcessor)
    {
        _nodeProcessor = nodeProcessor;
    }
    
    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {

        if (node is not AstSequence sequence) throw new Exception();
        
        foreach (var n in 
                 sequence.ChildNodes.OfType<JsNamedFunctionDeclaration>())
        {
            context.AddNamedFunctionDeclaration(n.Name);
        }
                
        foreach (var n in sequence.ChildNodes)
        {
            _nodeProcessor.ProcessNode(n, context);
        }
    }
}