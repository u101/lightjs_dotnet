using App16.ALang.Js.Errors;
using App16.ALang.Js.Tokenizers;
using App16.ALang.Tokenizers;

namespace App16.ALang.Js.Tests.Tokenizers;

[TestFixture]
public class TokenizerTests
{
    [Test]
     public void ReadValidSingleLineComment()
     {
          var tokens = ReadTokens("// asd asd");
          
          Assert.That(tokens, Is.Empty);
     }
     
     [Test]
     public void ReadValidMultiLineComment()
     {
          var tokens = ReadTokens("/* asd asd \n */");
          
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
          var tokens = ReadTokens(sourceCodeString);

          Assert.That(tokens, Has.Count.EqualTo(1));

          var token = tokens[0];
          
          Assert.That(token.TokenType, Is.EqualTo(JsTokenTypes.StringLiteral));

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
               var tokens = ReadTokens(sourceCodeString);

               Assert.That(tokens, Has.Count.EqualTo(1));

               var token = tokens[0];
          
               Assert.That(token.TokenType, Is.EqualTo(JsTokenTypes.StringLiteral));

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
          var tokens = ReadTokens("a = b + c");
          
          Assert.That(tokens, Has.Count.EqualTo(5));
          
          Assert.That(tokens[0].TokenType, Is.EqualTo(JsTokenTypes.Identifier));
          Assert.That(tokens[2].TokenType, Is.EqualTo(JsTokenTypes.Identifier));
          Assert.That(tokens[4].TokenType, Is.EqualTo(JsTokenTypes.Identifier));
          
          Assert.That(tokens[1].TokenType, Is.EqualTo(JsTokenTypes.OpAssign));
          Assert.That(tokens[3].TokenType, Is.EqualTo(JsTokenTypes.OpPlus));
     }

     [Test]
     public void ReadValidCompositeOperators()
     {
          ReadValidToken("++", JsTokenTypes.OpIncrement);
          ReadValidToken("--", JsTokenTypes.OpDecrement);
          ReadValidToken("+=", JsTokenTypes.OpPlusAssign);
          ReadValidToken("-=", JsTokenTypes.OpMinusAssign);
          ReadValidToken("==", JsTokenTypes.OpEquals);
          ReadValidToken("===", JsTokenTypes.OpEqualsStrict);
          
          ReadValidToken(">=", JsTokenTypes.OpGreaterOrEqual);
          ReadValidToken("<=", JsTokenTypes.OpLessOrEqual);
          ReadValidToken("!=", JsTokenTypes.OpNotEqual);
          ReadValidToken("!==", JsTokenTypes.OpNotEqualStrict);
          
          ReadValidToken("&&", JsTokenTypes.OpLogicalAnd);
          ReadValidToken("||", JsTokenTypes.OpLogicalOr);
     }

     [Test]
     public void ReadValidDecimalInt()
     {
          ReadValidToken("123456", JsTokenTypes.IntDecimal);
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
          ReadValidToken("0x0eF45ab", JsTokenTypes.IntHex);
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
          ReadValidToken("0b0", JsTokenTypes.IntBinary);
          ReadValidToken("0b1010101010101", JsTokenTypes.IntBinary);
          ReadValidToken("0b01", JsTokenTypes.IntBinary);
     }
     
     [Test]
     public void ReadInvalidBinaryInt()
     {
          ReadInvalidToken("0b");
          ReadInvalidToken("0b777");
          ReadInvalidToken("0bAX");
          ReadInvalidToken("0b000101ef");
     }

     private static void ReadValidToken(string testString, int expectedTokenType)
     {
          var sourceCodeString = $"// valid token next \n {testString}";
          var tokens = ReadTokens(sourceCodeString);
          
          Assert.That(tokens, Has.Count.EqualTo(1));

          var token = tokens[0];

          Assert.That(token.TokenType, Is.EqualTo(expectedTokenType));
          
          var str = sourceCodeString.Substring(
               token.Position.CharIndex, token.StringLength);
          
          Assert.That(str, Is.EqualTo(testString));
     }
     
     private static void ReadInvalidToken(string testString)
     {
          Assert.Throws<JsSyntaxError>(() => ReadTokens($"// invalid token next \n {testString}"));
     }
     
     [Test]
     public void ReadValidFloat()
     {
          ReadValidToken("123.456", JsTokenTypes.Float);
          ReadValidToken("1.4", JsTokenTypes.Float);
          ReadValidToken("1.4e+8", JsTokenTypes.FloatE);
          ReadValidToken("0.5e-9", JsTokenTypes.FloatE);
          ReadValidToken("1e+2", JsTokenTypes.FloatE);
          ReadValidToken("1e-4", JsTokenTypes.FloatE);
     }
     
     [Test]
     public void ReadInvalidFloat()
     {
          ReadInvalidToken("12.3.456");
          ReadInvalidToken("1e+2.45");
     }
     
     private static List<Token> ReadTokens(string sourceCodeString)
     {
          var tokenizer = JsTokenizerFactory.CreateTokenizer(sourceCodeString);
        
          return tokenizer.ReadTokens();
     }
}