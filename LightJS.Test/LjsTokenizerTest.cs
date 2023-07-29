using FluentAssertions;
using LightJS.Tokenizer;

namespace LightJS.Tests;

[TestFixture]
public class LjsTokenizerTest
{

     [Test]
     public void ReadValidSingleLineComment()
     {
          var sourceCode = new LjsSourceCode("// asd asd");
          var ljsTokenizer = new LjsTokenizer(sourceCode);
          var tokens = ljsTokenizer.ReadTokens();
          
          Assert.That(tokens, Is.Empty);
     }
     
     [Test]
     public void ReadValidMultiLineComment()
     {
          var sourceCode = new LjsSourceCode("/* asd asd \n */");
          var ljsTokenizer = new LjsTokenizer(sourceCode);
          var tokens = ljsTokenizer.ReadTokens();
          
          Assert.That(tokens, Is.Empty);
     }
     
     [Test]
     public void ReadValidStringLiteralTest()
     {
          const string testString = "abc";
          
          var sourceCode = new LjsSourceCode($"\"{testString}\"");
          var ljsTokenizer = new LjsTokenizer(sourceCode);
          var tokens = ljsTokenizer.ReadTokens();

          Assert.That(tokens, Has.Count.EqualTo(1));

          var token = tokens[0];
          
          Assert.That(token.TokenType, Is.EqualTo(LjsTokenType.String));
          Assert.That(token.StringLength, Is.EqualTo(3));

          var str = sourceCode.Substring(
               token.Position.CharIndex, token.StringLength);
          
          Assert.That(str, Is.EqualTo(testString));
     }
     
     [Test]
     public void ReadValidStringLiteralWithEscapeQuotesTest()
     {
          var testStrings = new[]
          {
               "abc\\\"def\\\"",
               "abc\\\\",
               "abc\\\\\\\"def\\\"",
          };

          foreach (var testString in testStrings)
          {
               var sourceCode = new LjsSourceCode($"\"{testString}\"");
               var ljsTokenizer = new LjsTokenizer(sourceCode);
               var tokens = ljsTokenizer.ReadTokens();

               Assert.That(tokens, Has.Count.EqualTo(1));

               var token = tokens[0];
          
               Assert.That(token.TokenType, Is.EqualTo(LjsTokenType.String));

               var str = sourceCode.Substring(
                    token.Position.CharIndex, token.StringLength);
          
               Assert.That(str, Is.EqualTo(testString));
          }
     }

    [Test]
     public void ReadInvalidUnclosedStringLiteralTest()
     {
          var sourceCode = new LjsSourceCode("\"abc");
          var ljsTokenizer = new LjsTokenizer(sourceCode);

          Assert.Throws<LjsTokenizerError>(() => ljsTokenizer.ReadTokens());
     }
     
     [Test]
     public void ReadInvalidMultilineStringLiteralTest()
     {
          var sourceCode = new LjsSourceCode("\"abc\nxyz\"");
          var ljsTokenizer = new LjsTokenizer(sourceCode);

          Assert.Throws<LjsTokenizerError>(() => ljsTokenizer.ReadTokens());
     }

     [Test]
     public void ReadValidSimpleExpression()
     {
          var sourceCode = new LjsSourceCode("a = b + c");
          var ljsTokenizer = new LjsTokenizer(sourceCode);
          
          var tokens = ljsTokenizer.ReadTokens();
          
          Assert.That(tokens, Has.Count.EqualTo(5));
          
          Assert.That(tokens[0].TokenType, Is.EqualTo(LjsTokenType.Word));
          Assert.That(tokens[2].TokenType, Is.EqualTo(LjsTokenType.Word));
          Assert.That(tokens[4].TokenType, Is.EqualTo(LjsTokenType.Word));
          
          Assert.That(tokens[1].TokenType, Is.EqualTo(LjsTokenType.Operator));
          Assert.That(tokens[3].TokenType, Is.EqualTo(LjsTokenType.Operator));
     }

     [Test]
     public void ReadValidDecimalInt()
     {
          ReadValidToken("123456", LjsTokenType.Int);
     }
     
     [Test]
     public void ReadInvalidDecimalInt()
     {
          ReadInvalidToken("12ex");
          ReadInvalidToken("12foo");
     }

     [Test]
     public void ReadValidHexInt()
     {
          ReadValidToken("0x0eF45ab", LjsTokenType.HexInt);
     }
     
     [Test]
     public void ReadInvalidHexInt()
     {
          ReadInvalidToken("0x");
          ReadInvalidToken("0xCGH");
     }
     
     [Test]
     public void ReadValidBinaryInt()
     {
          ReadValidToken("0b0", LjsTokenType.BinaryInt);
          ReadValidToken("0b1010101010101", LjsTokenType.BinaryInt);
          ReadValidToken("0b01", LjsTokenType.BinaryInt);
     }
     
     [Test]
     public void ReadInvalidBinaryInt()
     {
          ReadInvalidToken("0b");
          ReadInvalidToken("0b777");
          ReadInvalidToken("0bAX");
          ReadInvalidToken("0b000101ef");
     }

     private static void ReadValidToken(string testString, LjsTokenType expectedTokenType)
     {
          var sourceCode = new LjsSourceCode($"// valid token next \n {testString}");
          var ljsTokenizer = new LjsTokenizer(sourceCode);
          
          var tokens = ljsTokenizer.ReadTokens();
          
          Assert.That(tokens, Has.Count.EqualTo(1));

          var token = tokens[0];

          Assert.That(token.TokenType, Is.EqualTo(expectedTokenType));
          
          var str = sourceCode.Substring(
               token.Position.CharIndex, token.StringLength);
          
          Assert.That(str, Is.EqualTo(testString));
     }
     
     private static void ReadInvalidToken(string testString)
     {
          var sourceCode = new LjsSourceCode($"// invalid token next \n {testString}");
          var ljsTokenizer = new LjsTokenizer(sourceCode);
          
          Assert.Throws<LjsTokenizerError>(() => ljsTokenizer.ReadTokens());
     }
     
     [Test]
     public void ReadValidFloat()
     {
          ReadValidToken("123.456", LjsTokenType.Float);
          ReadValidToken("1.4", LjsTokenType.Float);
          ReadValidToken("1.4e+8", LjsTokenType.FloatE);
          ReadValidToken("0.5e-9", LjsTokenType.FloatE);
          ReadValidToken("1e+2", LjsTokenType.FloatE);
          ReadValidToken("1e-4", LjsTokenType.FloatE);
     }
     
     [Test]
     public void ReadInvalidFloat()
     {
          ReadInvalidToken("12.3.456");
          ReadInvalidToken("1e+2.45");
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
    
}