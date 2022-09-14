using FluentAssertions;

namespace LightJS.Tests;

[TestFixture]
public class LjsAstBuilderTest
{
    
    private static LjsSourceCode LoadSourceCode(string scriptFileName)
    {
        var text = TestUtils.LoadJsFile(scriptFileName);

        return new LjsSourceCode(text);
    }

    [Test]
    public void Build_ShouldReturnNotNullValue()
    {
        var sourceCode = LoadSourceCode("simpleTest.js");
        var tokenizer = new LjsTokenizer(new LjsReader(sourceCode));
        var tokens = tokenizer.ReadTokens();

        var astBuilder = new LjsAstBuilder(sourceCode);

        var astModel = astBuilder.Build(tokens);

        astModel.Should().NotBeNull();
    } 
    
}