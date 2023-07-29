using FluentAssertions;
using LightJS.Tokenizer;

namespace LightJS.Tests;

[TestFixture]
public class LjsTokenizerTest
{
     [Test]
     public void ReadValidStringLiteralTest()
     {
          var sourceCode = new LjsSourceCode("\"abc\"");
          var ljsTokenizer = new LjsTokenizer(sourceCode);
          var tokens = ljsTokenizer.ReadTokens();
          
          Assert.That(tokens.Count, Is.EqualTo(1));
          Assert.That(tokens[0].TokenType, Is.EqualTo(LjsTokenType.String));
          Assert.That(tokens[0].StringLength, Is.EqualTo(3));
     }
     
     [Test]
     public void ReadInvalidUnclosedStringLiteralTest()
     {
          var sourceCode = new LjsSourceCode("\"abc");
          var ljsTokenizer = new LjsTokenizer(sourceCode);

          Assert.Throws<LjsTokenizerError>(() => ljsTokenizer.ReadTokens());
     }
     

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
          var ljsTokenizer = new LjsTokenizer(sourceCode);
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

          var ljsTokenTypes = Enum.GetValues<LjsTokenType>();

          foreach (var tokenType in ljsTokenTypes)
          {
               tokens.Should().Contain(x => x.TokenType == tokenType);
          }
     }

     
     // todo write tests to see how invalid scripts are handled
     
     // MethodName_Should_When
    
}