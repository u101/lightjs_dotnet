using LightJS.Ast;
using FluentAssertions;

namespace LightJS.Test;

[TestFixture]
public class LjsAstSimpleExpressionsTest
{
    
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
        var node = TestUtils.BuildAstNode("a + b + 5");

        node.Should().BeEquivalentTo("a".ToVar().Plus("b".ToVar()).Plus(5.ToLit()));
    }

    [Test]
    public void IncrementSimpleTest()
    {
        var a = TestUtils.BuildAstNode("++a");
        a.Should().BeEquivalentTo("a".ToVar().WithPrefixIncrement());
        
        a = TestUtils.BuildAstNode("a++");
        a.Should().BeEquivalentTo("a".ToVar().WithPostfixIncrement());
    }

    [Test]
    public void UnaryMinusSimpleTest()
    {
        var node = TestUtils.BuildAstNode("-a + b");

        node.Should().BeEquivalentTo("a".WithUnaryMinus().Plus("b"));
    }
    
    [Test]
    public void UnaryMinusAssignmentTest()
    {
        var node = TestUtils.BuildAstNode("x = -a + -b");

        node.Should().BeEquivalentTo(
            "x".Assign("a".WithUnaryMinus().Plus("b".WithUnaryMinus())));
    }
    
    [Test]
    public void UnaryMinusAssignmentWithParentheses()
    {
        var node = TestUtils.BuildAstNode("x = (-(a + b))");

        node.Should().BeEquivalentTo("x".Assign("a".Plus("b").WithUnaryMinus()));
    }
    
    [Test]
    public void UnaryMinusSequence()
    {
        var node = TestUtils.BuildAstNode("x = a - - - - - b");

        node.Should()
            .BeEquivalentTo(
                "x".Assign("a".Minus("b".WithUnaryMinus().WithUnaryMinus().WithUnaryMinus().WithUnaryMinus())));
    }
    
    [Test]
    public void UnaryPlusMinusSequence()
    {
        var node = TestUtils.BuildAstNode("x = a - + - + - b");

        node.Should()
            .BeEquivalentTo(
                "x".Assign("a".Minus("b".WithUnaryMinus().WithUnaryPlus().WithUnaryMinus().WithUnaryPlus())));
    }
    
    [Test]
    public void UnaryMinusAssignmentComplexTest()
    {
        var node = TestUtils.BuildAstNode("x = -(a + -b)");

        node.Should()
            .BeEquivalentTo(
                "x".Assign("a".Plus("b".WithUnaryMinus()).WithUnaryMinus()));
    }
    
    [Test]
    public void UnaryPlusAssignmentTest()
    {
        var node = TestUtils.BuildAstNode("x = +a + +b");

        node.Should().BeEquivalentTo("x".Assign("a".WithUnaryPlus().Plus("b".WithUnaryPlus())));
    }
    
    [Test]
    public void ParenthesesTest()
    {
        var node = TestUtils.BuildAstNode("a + ( b - c ) + d");

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
            var node = TestUtils.BuildAstNode(expression);
            node.Should().BeEquivalentTo(("a".Plus("b")).Minus("c".Plus("d")));
        }
    }
    
    [Test]
    public void AssignSimpleTest()
    {
        var node = TestUtils.BuildAstNode("a = b + c");

        node.Should().BeEquivalentTo("a".Assign("b".Plus("c")));
    }
    
    [Test]
    public void AssignSequenceTest()
    {
        var node = TestUtils.BuildAstNode("x = (y = (a = b + c))");

        node.Should().BeEquivalentTo(
            "x".Assign("y".Assign("a".Assign(
                "b".Plus("c")
            ))));
    }
    
    [Test]
    public void AssignWithParenthesesTest()
    {
        var node = TestUtils.BuildAstNode("x = a + ( b - c ) + d");

        node.Should().BeEquivalentTo(
            "x".Assign(
                "a".Plus("b".Minus("c")).Plus("d")));
    }

    [Test]
    public void DotAccessSimpleTest()
    {
        var node = TestUtils.BuildAstNode("x = a.foo.bar");
        node.Should().BeEquivalentTo(
            "x".Assign("a".GetProp("foo").GetProp("bar")));
    }
    
    [Test]
    public void DotPropertyAssignSimpleTest()
    {
        var node = TestUtils.BuildAstNode("a.foo.bar = x");
        var expected = "a".GetProp("foo").SetProp("bar", "x".ToVar());
        node.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
    }
    
    [Test]
    public void BracketsPropertySimpleTest()
    {
        var node = TestUtils.BuildAstNode("x = a['foo']");
        var expected = "x".Assign("a".GetProp("foo".ToLit()));
        node.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
    }
    
    [Test]
    public void BracketsPropertyAssignSimpleTest()
    {
        var node = TestUtils.BuildAstNode("a['foo'] = x");
        var expected = "a".SetProp("foo".ToLit(), "x".ToVar());
        node.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
    }

    
}