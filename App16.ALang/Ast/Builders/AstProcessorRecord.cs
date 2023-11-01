namespace App16.ALang.Ast.Builders;

public sealed class AstProcessorRecord
{
    public IForwardLookup Lookup { get; }
    public IAstNodeProcessor Processor { get; }

    public AstProcessorRecord(IForwardLookup lookup, IAstNodeProcessor processor)
    {
        Lookup = lookup;
        Processor = processor;
    }
    
}