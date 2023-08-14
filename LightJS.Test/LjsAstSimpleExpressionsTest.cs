using LightJS.Ast;
using FluentAssertions;

namespace LightJS.Test;

[TestFixture]
public class LjsAstSimpleExpressionsTest
{

    private static ILjsAstNode Parse(string sourceCode)
    {
        var builder = new LjsAstBuilder2(sourceCode);
        var model = builder.Build();
        return model.RootNode;
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
        var rootNode = astBuilder.Build().RootNode;

        rootNode.Should().BeOfType<LjsAstLiteral<TLiteralType>>();
        rootNode.Should().BeEquivalentTo(new LjsAstLiteral<TLiteralType>(expectedValue));
    }
    
    [Test]
    public void SimpleTest()
    {
        var node = Parse("a + b + 5");

        node.Should().BeEquivalentTo("a".ToVar().Plus("b".ToVar()).Plus(5.ToLit()));
    }

    [Test]
    public void IncrementSimpleTest()
    {
        var a = Parse("++a");
        a.Should().BeEquivalentTo("a".ToVar().WithPrefixIncrement());
        
        a = Parse("a++");
        a.Should().BeEquivalentTo("a".ToVar().WithPostfixIncrement());
    }

    [Test]
    public void UnaryMinusSimpleTest()
    {
        var node = Parse("-a + b");

        node.Should().BeEquivalentTo("a".WithUnaryMinus().Plus("b"));
    }
    
    [Test]
    public void UnaryMinusAssignmentTest()
    {
        var node = Parse("x = -a + -b");

        node.Should().BeEquivalentTo(
            "x".Assign("a".WithUnaryMinus().Plus("b".WithUnaryMinus())));
    }
    
    [Test]
    public void UnaryMinusAssignmentWithParentheses()
    {
        var node = Parse("x = (-(a + b))");

        node.Should().BeEquivalentTo("x".Assign("a".Plus("b").WithUnaryMinus()));
    }
    
    [Test]
    public void UnaryMinusSequence()
    {
        var node = Parse("x = a - - - - - b");

        node.Should()
            .BeEquivalentTo(
                "x".Assign("a".Minus("b".WithUnaryMinus().WithUnaryMinus().WithUnaryMinus().WithUnaryMinus())));
    }
    
    [Test]
    public void UnaryPlusMinusSequence()
    {
        var node = Parse("x = a - + - + - b");

        node.Should()
            .BeEquivalentTo(
                "x".Assign("a".Minus("b".WithUnaryMinus().WithUnaryPlus().WithUnaryMinus().WithUnaryPlus())));
    }
    
    [Test]
    public void UnaryMinusAssignmentComplexTest()
    {
        var node = Parse("x = -(a + -b)");

        node.Should()
            .BeEquivalentTo(
                "x".Assign("a".Plus("b".WithUnaryMinus()).WithUnaryMinus()));
    }
    
    [Test]
    public void UnaryPlusAssignmentTest()
    {
        var node = Parse("x = +a + +b");

        node.Should().BeEquivalentTo("x".Assign("a".WithUnaryPlus().Plus("b".WithUnaryPlus())));
    }
    
    [Test]
    public void ParenthesesTest()
    {
        var node = Parse("a + ( b - c ) + d");

        node.Should().BeEquivalentTo(
            "a".Plus("b".Minus("c")).Plus("d"));
    }
    
    [Test]
    public void ParenthesesSimpleExpressionTest()
    {
        Check("(a+b)-(c+d)");
        Check("((a+b)-(c+d))");
        Check("(((a+b)-(c+d)))");
        Check("(((a+b)-((c+d))))");

        void Check(string expression)
        {
            var node = Parse(expression);
            node.Should().BeEquivalentTo(("a".Plus("b")).Minus("c".Plus("d")));
        }
    }
    
    [Test]
    public void AssignSimpleTest()
    {
        var node = Parse("a = b + c");

        node.Should().BeEquivalentTo("a".Assign("b".Plus("c")));
    }
    
    [Test]
    public void AssignSequenceTest()
    {
        var node = Parse("x = (y = (a = b + c))");

        node.Should().BeEquivalentTo(
            "x".Assign("y".Assign("a".Assign(
                "b".Plus("c")
            ))));
    }
    
    [Test]
    public void AssignWithParenthesesTest()
    {
        var node = Parse("x = a + ( b - c ) + d");

        node.Should().BeEquivalentTo(
            "x".Assign(
                "a".Plus("b".Minus("c")).Plus("d")));
    }

    [Test]
    public void DotAccessSimpleTest()
    {
        var node = Parse("x = a.foo.bar");
        node.Should().BeEquivalentTo(
            "x".Assign("a".GetProp("foo").GetProp("bar")));
    }
    
    [Test]
    public void DotPropertyAssignSimpleTest()
    {
        var node = Parse("a.foo.bar = x");
        var expected = "a".GetProp("foo").SetProp("bar", "x".ToVar());
        node.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
    }

    [Test]
    public void SimpleFuncCallWithoutArgs()
    {
        var node = Parse("x = a() + (b() + c())");
        var expected = "x".Assign(
            "a".FuncCall().Plus("b".FuncCall().Plus("c".FuncCall())));
        
        node.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
    }

    /*[Test]
    public void SimpleFuncCall()
    {
        var node = Parse("x = foo(a, c-(a+b))");
        var expected = "x".Assign("foo".FuncCall(
            "a".ToVar(),
            "c".Minus("a".Plus("b"))
        ));
        node.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
    }*/
}