namespace App16.ALang.Ast.Builders;

public sealed class AstDecoratorRecord
{
    public IDecoratorForwardLookup Lookup { get; }
    public IAstDecoratorProcessor Processor { get; }

    public AstDecoratorRecord(
        IDecoratorForwardLookup lookup, 
        IAstDecoratorProcessor processor)
    {
        Lookup = lookup;
        Processor = processor;
    }
}