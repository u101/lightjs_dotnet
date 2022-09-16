using FluentAssertions;
using LightJS.Ast;
using LightJS.Tokenizer;

namespace LightJS.Tests;

[TestFixture]
public class LjsAstBuilderTest
{
    
    private static LjsSourceCode LoadSourceCode(string scriptFileName)
    {
        var text = TestUtils.LoadJsFile(scriptFileName);

        return new LjsSourceCode(text);
    }

    private static (LjsAstBuilder, List<LjsToken>) CreateBuilderWithExternalScriptFile(string scriptFileName)
    {
        var text = TestUtils.LoadJsFile(scriptFileName);
        return CreateBuilderWithStringInput(text);
    }

    private static (LjsAstBuilder, List<LjsToken>) CreateBuilderWithStringInput(string sourceCodeString)
    {
        var sourceCode = new LjsSourceCode(sourceCodeString);
        var tokenizer = new LjsTokenizer(new LjsReader(sourceCode));
        var tokens = tokenizer.ReadTokens();

        var astBuilder = new LjsAstBuilder(sourceCode);
        
        return (astBuilder, tokens);
    }
    
    [Test]
    public void Build_ShouldReturnIntNode_WhenGivenIntNumber()
    {
        var (builder, tokens) = CreateBuilderWithStringInput("123456789");

        var astModel = builder.Build(tokens);

        astModel.RootNodes.Should().HaveCount(1);

        astModel.RootNodes[0].Should().BeEquivalentTo(new LjsAstValue<int>(123456789));
        astModel.RootNodes[0].Should().NotBeEquivalentTo(new LjsAstValue<int>(1));
    }

    /*[Test]
    public void Build_ShouldReturnNotNullValue()
    {
        var sourceCode = LoadSourceCode("simpleTest.js");
        var tokenizer = new LjsTokenizer(new LjsReader(sourceCode));
        var tokens = tokenizer.ReadTokens();

        var astBuilder = new LjsAstBuilder(sourceCode);

        var astModel = astBuilder.Build(tokens);

        astModel.Should().NotBeNull();
    } */
    
}