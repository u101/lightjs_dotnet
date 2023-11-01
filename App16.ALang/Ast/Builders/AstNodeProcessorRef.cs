namespace App16.ALang.Ast.Builders;

public sealed class AstNodeProcessorRef : IAstNodeProcessor
{
    public IAstNodeProcessor Processor { get; set; } = AstEmptyNodeProcessor.Instance;

    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        return Processor.ProcessNext(context);
    }
}