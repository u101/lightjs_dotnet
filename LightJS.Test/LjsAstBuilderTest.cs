using FluentAssertions;
using LightJS.Ast;
using LightJS.Tokenizer;

namespace LightJS.Test;

[TestFixture]
public class LjsAstBuilderTest
{
    
    [Test]
    public void Build_ShouldReturnIntNode_WhenGivenIntLiteral()
    {
        var astBuilder = new LjsAstBuilder("123456789");
        var astModel = astBuilder.Build();

        astModel.RootNodes.Should().HaveCount(1);

        astModel.RootNodes[0].Should().BeEquivalentTo(new LjsAstValue<int>(123456789));
        astModel.RootNodes[0].Should().NotBeEquivalentTo(new LjsAstValue<int>(1));
    }
    
    [Test]
    public void Build_ShouldReturnDoubleNode_WhenGivenDoubleLiteral()
    {
        var astBuilder = new LjsAstBuilder("3.14");
        var astModel = astBuilder.Build();

        astModel.RootNodes.Should().HaveCount(1);

        astModel.RootNodes[0].Should().BeEquivalentTo(new LjsAstValue<double>(3.14));
        astModel.RootNodes[0].Should().NotBeEquivalentTo(new LjsAstValue<double>(3.141));
    }
    
    [Test]
    public void Build_ShouldReturnStringNode_WhenGivenStringLiteral()
    {
        const string someString = "Hello world";
        const string sourceCodeString = $"\"{someString}\"";
        
        var astBuilder = new LjsAstBuilder(sourceCodeString);
        var astModel = astBuilder.Build();

        astModel.RootNodes.Should().HaveCount(1);

        astModel.RootNodes[0].Should().BeEquivalentTo(new LjsAstValue<string>(someString));
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