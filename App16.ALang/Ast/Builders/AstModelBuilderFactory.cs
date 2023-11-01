using App16.ALang.Tokenizers;

namespace App16.ALang.Ast.Builders;

public sealed class AstModelBuilderFactory
{
    private readonly IAstNodeProcessor _nodeProcessor;

    public AstModelBuilderFactory(IAstNodeProcessor nodeProcessor)
    {
        _nodeProcessor = nodeProcessor;
    }
    
    public AstModelBuilder CreateBuilder(string sourceCodeString, List<Token> tokens) => 
        new(_nodeProcessor, sourceCodeString, tokens);
}