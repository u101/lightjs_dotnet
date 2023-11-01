using App16.ALang.Ast;
using App16.LightJS.Errors;

namespace App16.LightJS.Compiler;

public sealed class LjsCompilerNodeSelectorWithLookup : ILjsCompilerNodeProcessor
{
    private readonly ILjsCompilerNodeProcessor _defaultProcessor;
    private readonly List<Entry> _entries = new();
    
    public LjsCompilerNodeSelectorWithLookup(ILjsCompilerNodeProcessor defaultProcessor)
    {
        _defaultProcessor = defaultProcessor;
    }
    
    public void Register(ILjsCompilerNodeLookup lookup, ILjsCompilerNodeProcessor processor)
    {
        _entries.Add(new Entry(lookup, processor));
    }
    
    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {

        var nodeType = node.GetType();
        
        for (var i = 0; i < _entries.Count; i++)
        {
            var e = _entries[i];

            if (e.Lookup.ShouldProcess(node))
            {
                e.Processor.ProcessNode(node, context);
                return;
            }
        }
        
        _defaultProcessor.ProcessNode(node, context);
    }
    
    private class Entry
    {
        public ILjsCompilerNodeLookup Lookup { get; }
        public ILjsCompilerNodeProcessor Processor { get; }

        public Entry(
            ILjsCompilerNodeLookup lookup, 
            ILjsCompilerNodeProcessor processor)
        {
            Lookup = lookup;
            Processor = processor;
        }
    }
    
}
public sealed class LjsCompilerNodeSelectorByType : ILjsCompilerNodeProcessor
{
    private readonly ILjsCompilerNodeProcessor _defaultProcessor;
    private readonly Dictionary<Type, ILjsCompilerNodeProcessor> _processorsByNodeType = new();

    public LjsCompilerNodeSelectorByType(ILjsCompilerNodeProcessor defaultProcessor)
    {
        _defaultProcessor = defaultProcessor;
    }
    
    public void Register(Type nodeType, ILjsCompilerNodeProcessor processor)
    {
        _processorsByNodeType.Add(nodeType, processor);
    }
    
    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {

        var nodeType = node.GetType();

        if (_processorsByNodeType.TryGetValue(nodeType, out var proc))
        {
            proc.ProcessNode(node, context);
        }
        else
        {
            _defaultProcessor.ProcessNode(node, context);
        }
        
    }
    
}

public sealed class LjsCompilerNodeSelectorDefault : ILjsCompilerNodeProcessor
{

    public void ProcessNode(IAstNode node, LjsCompilerContext context)
    {
        throw new LjsCompilerError($"processor not found for node type {node.GetType()}");
    }

    
}