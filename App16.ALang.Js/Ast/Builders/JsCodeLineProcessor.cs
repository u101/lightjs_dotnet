using App16.ALang.Ast;
using App16.ALang.Ast.Builders;

namespace App16.ALang.Js.Ast.Builders;

public sealed class JsCodeLineProcessor : IAstNodeProcessor
{
    private readonly AstProcessorRecord[] _processors;
    private readonly IAstNodeProcessor _defaultProcessor;

    public JsCodeLineProcessor(AstProcessorRecord[] processors, IAstNodeProcessor defaultProcessor)
    {
        _processors = processors;
        _defaultProcessor = defaultProcessor;
    }
    
    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        for (var i = 0; i < _processors.Length; i++)
        {
            var p = _processors[i];
            if (p.Lookup.LookForward(context.TokensIterator))
            {
                return p.Processor.ProcessNext(context);
            }
        }
        
        return _defaultProcessor.ProcessNext(context);
    }
}