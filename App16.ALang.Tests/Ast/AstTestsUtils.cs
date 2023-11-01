using App16.ALang.Ast;
using App16.ALang.Ast.Builders;
using App16.ALang.Tests.DefaultLang;
using App16.ALang.Tokenizers;
using FluentAssertions;

namespace App16.ALang.Tests.Ast;

public static class AstTestsUtils
{
    private static readonly AstModelBuilderFactory ModelBuilderFactory = DefAstBuilderFactoryProvider.CreateAstBuilderFactory();

    private static readonly TokenizerFactory TestTokenizerFactory = DefTokenizerUtils.CreateTokenizerFactory();

    public static void Match(IAstNode result, IAstNode expected)
    {
        result.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes().WithStrictOrdering());
    }

    public static IAstNode BuildAstNode(string sourceCode)
    {
        var builder = CreateBuilder(sourceCode);
        return builder.Build();
    }

    private static AstModelBuilder CreateBuilder(string sourceCode)
    {
        var tokenizer = TestTokenizerFactory.CreateTokenizer(sourceCode);

        var tokens = tokenizer.ReadTokens();

        return ModelBuilderFactory.CreateBuilder(sourceCode, tokens);
    }

    

    
}