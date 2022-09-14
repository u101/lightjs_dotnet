using FluentAssertions;

namespace LightJS.Tests;

using LightJS;

[TestFixture]
public class LjsTokenizerTest
{

     [Test]
     public void TestLoadScript()
     {
          var text = TestUtils.LoadJsFile("simpleTest.js");
        
          Assert.That(text, Is.Not.Null);
          Assert.That(text, Is.Not.Empty);
     }
     
     private static LjsTokenizer CreateLjsTokenizer(string scriptFileName)
     {
          var text = TestUtils.LoadJsFile(scriptFileName);

          var sourceCode = new LjsSourceCode(text);
          var ljsReader = new LjsReader(sourceCode);
          var ljsTokenizer = new LjsTokenizer(ljsReader);
          return ljsTokenizer;
     }
     
     [Test]
     public void ReadTokens_ShouldReturnTokens_WhenJsScriptIsValid()
     {
          var ljsTokenizer = CreateLjsTokenizer("simpleTest.js");

          var tokens = ljsTokenizer.ReadTokens();

          tokens.Should().NotBeEmpty();
     }

     

     [Test]
     public void ReadTokens_ShouldReturnTokensOfAllValidTypes_WhenJsScriptIsValid()
     {
          var ljsTokenizer = CreateLjsTokenizer("simpleTest.js");

          var tokens = ljsTokenizer.ReadTokens();

          var ljsTokenTypes = Enum.GetValues<LjsTokenType>().
               Where(v => v != LjsTokenType.Null);

          foreach (var tokenType in ljsTokenTypes)
          {
               tokens.Should().Contain(x => x.TokenType == tokenType);
          }

          tokens.Should().NotContain(x => x.TokenType == LjsTokenType.Null);
     }

     
     // todo write tests to see how invalid scripts are handled
     
     // MethodName_Should_When
    
}