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
     
     [Test]
     public void ReadTokens_ShouldReturnTokens_WhenJsScriptIsValid()
     {
          var text = TestUtils.LoadJsFile("simpleTest.js");

          var ljsTokenizer = new LjsTokenizer(new LjsReader(text));

          var tokens = ljsTokenizer.ReadTokens();

          tokens.Should().NotBeEmpty();
     } 
     
     // MethodName_Should_When
    
}