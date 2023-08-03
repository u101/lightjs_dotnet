using FluentAssertions;
using LightJS.Tokenizer;

namespace LightJS.Test;

[TestFixture]
public class LjsTokenizerTest
{

     [Test]
     public void ReadValidSingleLineComment()
     {
          var ljsTokenizer = new LjsTokenizer("// asd asd");
          var tokens = ljsTokenizer.ReadTokens();
          
          Assert.That(tokens, Is.Empty);
     }
     
     [Test]
     public void ReadValidMultiLineComment()
     {
          var ljsTokenizer = new LjsTokenizer("/* asd asd \n */");
          var tokens = ljsTokenizer.ReadTokens();
          
          Assert.That(tokens, Is.Empty);
     }
     
     [Test]
     public void ReadValidStringLiteralTest()
     {
          ReadValidStringLiteralTest("abc", '"');
          ReadValidStringLiteralTest("defg", '\'');
          ReadValidStringLiteralTest("", '"');
          ReadValidStringLiteralTest("", '\'');
     }

     private static void ReadValidStringLiteralTest(string testString, char quoteMark)
     {
          var sourceCodeString = $"{quoteMark}{testString}{quoteMark}";
          var ljsTokenizer = new LjsTokenizer(sourceCodeString);
          var tokens = ljsTokenizer.ReadTokens();

          Assert.That(tokens, Has.Count.EqualTo(1));

          var token = tokens[0];
          
          Assert.That(token.TokenType, Is.EqualTo(LjsTokenType.StringLiteral));

          var str = sourceCodeString.Substring(
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
               var sourceCodeString = $"\"{testString}\"";
               var ljsTokenizer = new LjsTokenizer(sourceCodeString);
               var tokens = ljsTokenizer.ReadTokens();

               Assert.That(tokens, Has.Count.EqualTo(1));

               var token = tokens[0];
          
               Assert.That(token.TokenType, Is.EqualTo(LjsTokenType.StringLiteral));

               var str = sourceCodeString.Substring(
                    token.Position.CharIndex, token.StringLength);
          
               Assert.That(str, Is.EqualTo(testString));
          }
     }

    [Test]
     public void ReadInvalidUnclosedStringLiteralTest()
     {
          ReadInvalidToken("\"abc");
     }
     
     [Test]
     public void ReadInvalidMultilineStringLiteralTest()
     {
          ReadInvalidToken("\"abc\nxyz\"");
     }

     [Test]
     public void ReadValidSimpleExpression()
     {
          var ljsTokenizer = new LjsTokenizer("a = b + c");
          
          var tokens = ljsTokenizer.ReadTokens();
          
          Assert.That(tokens, Has.Count.EqualTo(5));
          
          Assert.That(tokens[0].TokenClass, Is.EqualTo(LjsTokenClass.Word));
          Assert.That(tokens[2].TokenClass, Is.EqualTo(LjsTokenClass.Word));
          Assert.That(tokens[4].TokenClass, Is.EqualTo(LjsTokenClass.Word));
          
          Assert.That(tokens[1].TokenClass, Is.EqualTo(LjsTokenClass.Operator));
          Assert.That(tokens[3].TokenClass, Is.EqualTo(LjsTokenClass.Operator));
     }

     [Test]
     public void ReadValidCompositeOperators()
     {
          ReadValidToken("++", LjsTokenType.OpIncrement);
          ReadValidToken("--", LjsTokenType.OpDecrement);
          ReadValidToken("+=", LjsTokenType.OpPlusAssign);
          ReadValidToken("-=", LjsTokenType.OpMinusAssign);
          ReadValidToken("==", LjsTokenType.OpEquals);
          ReadValidToken("===", LjsTokenType.OpEqualsStrict);
          
          ReadValidToken(">=", LjsTokenType.OpGreaterOrEqual);
          ReadValidToken("<=", LjsTokenType.OpLessOrEqual);
          ReadValidToken("!=", LjsTokenType.OpNotEqual);
          ReadValidToken("!==", LjsTokenType.OpNotEqualStrict);
          
          ReadValidToken("&&", LjsTokenType.OpLogicalAnd);
          ReadValidToken("||", LjsTokenType.OpLogicalOr);
     }

     [Test]
     public void ReadValidDecimalInt()
     {
          ReadValidToken("123456", LjsTokenType.IntDecimal);
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
          ReadValidToken("0x0eF45ab", LjsTokenType.IntHex);
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
          ReadValidToken("0b0", LjsTokenType.IntBinary);
          ReadValidToken("0b1010101010101", LjsTokenType.IntBinary);
          ReadValidToken("0b01", LjsTokenType.IntBinary);
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
          var sourceCodeString = $"// valid token next \n {testString}";
          var ljsTokenizer = new LjsTokenizer(sourceCodeString);
          
          var tokens = ljsTokenizer.ReadTokens();
          
          Assert.That(tokens, Has.Count.EqualTo(1));

          var token = tokens[0];

          Assert.That(token.TokenType, Is.EqualTo(expectedTokenType));
          
          var str = sourceCodeString.Substring(
               token.Position.CharIndex, token.StringLength);
          
          Assert.That(str, Is.EqualTo(testString));
     }
     
     private static void ReadInvalidToken(string testString)
     {
          var ljsTokenizer = new LjsTokenizer($"// invalid token next \n {testString}");
          
          Assert.Throws<LjsSyntaxError>(() => ljsTokenizer.ReadTokens());
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
          var sourceCodeString = TestUtils.LoadJsFile(scriptFileName);
          var ljsTokenizer = new LjsTokenizer(sourceCodeString);
          return ljsTokenizer;
     }

     


     /*[Test]
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

          var ljsTokenTypes = Enum.GetValues<LjsTokenClass>();

          foreach (var tokenType in ljsTokenTypes)
          {
               tokens.Should().Contain(x => x.TokenClass == tokenType);
          }
     }*/
    
}