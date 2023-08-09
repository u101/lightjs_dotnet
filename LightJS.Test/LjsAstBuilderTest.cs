using FluentAssertions;
using LightJS.Ast;
using LightJS.Tokenizer;

namespace LightJS.Test;

[TestFixture]
public class LjsAstBuilderTest
{

    [Test]
    public void BuildSimpleExpression()
    {
        var astBuilder = new LjsAstBuilder("a + b");
        var rootNode = astBuilder.Build();
        
        rootNode.Should().BeOfType<LjsAstBinaryOperation>();

        rootNode.Should().BeEquivalentTo(
            new LjsAstBinaryOperation(
                new LjsAstGetVar("a"), 
                new LjsAstGetVar("b"), 
                LjsTokenType.OpPlus));
    }

    [Test]
    public void BuildSimpleTernaryIfExpression()
    {
        var astBuilder = new LjsAstBuilder("a ? b : c");
        var rootNode = astBuilder.Build();
        
        rootNode.Should().BeOfType<LjsAstTernaryIfOperation>();

        rootNode.Should().BeEquivalentTo(
            new LjsAstTernaryIfOperation(
                new LjsAstGetVar("a"),
                new LjsAstGetVar("b"),
                new LjsAstGetVar("c")));
    }

    [Test]
    public void BuildSimpleExpressionWithParentheses()
    {
        BuildSimpleExpressionWithParentheses("(a+b)-(c+d)");
        BuildSimpleExpressionWithParentheses("((a+b)-(c+d))");
        BuildSimpleExpressionWithParentheses("(((a+b)-(c+d)))");
        BuildSimpleExpressionWithParentheses("(((a+b)-((c+d))))");
    }
    
    private static void BuildSimpleExpressionWithParentheses(string expression)
    {
        var astBuilder = new LjsAstBuilder(expression);
        var rootNode = astBuilder.Build();
        
        rootNode.Should().BeOfType<LjsAstBinaryOperation>();

        rootNode.Should().BeEquivalentTo(
            new LjsAstBinaryOperation(
                new LjsAstBinaryOperation(new LjsAstGetVar("a"), new LjsAstGetVar("b"), LjsTokenType.OpPlus),
                new LjsAstBinaryOperation(new LjsAstGetVar("c"), new LjsAstGetVar("d"), LjsTokenType.OpPlus),
                LjsTokenType.OpMinus));
    }

    [Test]
    public void BuildInvalidParenthesesExpression()
    {
        BuildInvalidExpression("(a+b))");
        BuildInvalidExpression("(a+b");
    }
    
    [Test]
    public void BuildInvalidOperatorsExpression()
    {
        BuildInvalidExpression("a * * * b");
    }

    private static void BuildInvalidExpression(string expression)
    {
        var astBuilder = new LjsAstBuilder(expression);
        Assert.Throws<LjsSyntaxError>(() => astBuilder.Build());
    }
    
    [Test]
    public void BuildSimpleExpressionWithOperatorsPriority()
    {
        var astBuilder = new LjsAstBuilder("a + b/c + d");
        var rootNode = astBuilder.Build();
        
        rootNode.Should().BeOfType<LjsAstBinaryOperation>();

        var expectedResult = new LjsAstBinaryOperation(
            new LjsAstBinaryOperation(
                new LjsAstGetVar("a"),
                new LjsAstBinaryOperation(new LjsAstGetVar("b"), new LjsAstGetVar("c"), LjsTokenType.OpDiv),
                LjsTokenType.OpPlus),
            new LjsAstGetVar("d"),
            LjsTokenType.OpPlus);

        rootNode.Should().BeEquivalentTo(expectedResult);
    }
    
    [Test]
    public void BuildSimpleLogicalExpression()
    {
        var astBuilder = new LjsAstBuilder("a + b != c + d");
        var rootNode = astBuilder.Build();
        
        rootNode.Should().BeOfType<LjsAstBinaryOperation>();
        
        var expectedResult = new LjsAstBinaryOperation(
            new LjsAstBinaryOperation(new LjsAstGetVar("a"), new LjsAstGetVar("b"), LjsTokenType.OpPlus),
            new LjsAstBinaryOperation(new LjsAstGetVar("c"), new LjsAstGetVar("d"), LjsTokenType.OpPlus),
            LjsTokenType.OpNotEqual);

        rootNode.Should().BeEquivalentTo(expectedResult);
    }
    
    
    [Test]
    public void BuildSimpleLiteral()
    {
        ValidLiteralTest("123456789", 123456789);
        ValidLiteralTest("0xff1267", 0xff1267);
        ValidLiteralTest("0b010101", 0b010101);
        
        ValidLiteralTest("3.14", 3.14);
        ValidLiteralTest("3.14e+3", 3.14e+3);
        
        ValidLiteralTest("true", true);
        ValidLiteralTest("false", false);
        
        ValidLiteralTest<string>("\"Hello world\"", "Hello world");
        ValidLiteralTest<string>("'Hello world'", "Hello world");
    }

    private static void ValidLiteralTest<TLiteralType>(
        string literal, TLiteralType expectedValue)
    {
        var astBuilder = new LjsAstBuilder(literal);
        var rootNode = astBuilder.Build();

        rootNode.Should().BeOfType<LjsAstLiteral<TLiteralType>>();
        rootNode.Should().BeEquivalentTo(new LjsAstLiteral<TLiteralType>(expectedValue));
    }
    
}