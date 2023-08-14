using FluentAssertions;
using LightJS.Outsource;

namespace LightJS.Test;

[TestFixture]
public class MatherAdvTests
{

    [Test]
    public void SimpleTest()
    {
        var node = MatherAdv.Convert("a + b + 5");

        node.Should().BeEquivalentTo("a".ToVar().Plus("b".ToVar()).Plus(5.ToLit()));
    }

    [Test]
    public void IncrementSimpleTest()
    {
        var a = MatherAdv.Convert("++a");
        a.Should().BeEquivalentTo("a".ToVar().WithPrefixIncrement());
        
        a = MatherAdv.Convert("a++");
        a.Should().BeEquivalentTo("a".ToVar().WithPostfixIncrement());
    }

    [Test]
    public void UnaryMinusSimpleTest()
    {
        var node = MatherAdv.Convert("-a + b");

        node.Should().BeEquivalentTo("a".WithUnaryMinus().Plus("b"));
    }
    
    [Test]
    public void UnaryMinusAssignmentTest()
    {
        var node = MatherAdv.Convert("x = -a + -b");

        node.Should().BeEquivalentTo(
            "x".Assign("a".WithUnaryMinus().Plus("b".WithUnaryMinus())));
    }
    
    [Test]
    public void UnaryMinusAssignmentWithParentheses()
    {
        var node = MatherAdv.Convert("x = (-(a + b))");

        node.Should().BeEquivalentTo("x".Assign("a".Plus("b").WithUnaryMinus()));
    }
    
    [Test]
    public void UnaryMinusSequence()
    {
        var node = MatherAdv.Convert("x = a - - - - - b");

        node.Should()
            .BeEquivalentTo(
                "x".Assign("a".Minus("b".WithUnaryMinus().WithUnaryMinus().WithUnaryMinus().WithUnaryMinus())));
    }
    
    [Test]
    public void UnaryPlusMinusSequence()
    {
        var node = MatherAdv.Convert("x = a - + - + - b");

        node.Should()
            .BeEquivalentTo(
                "x".Assign("a".Minus("b".WithUnaryMinus().WithUnaryPlus().WithUnaryMinus().WithUnaryPlus())));
    }
    
    [Test]
    public void UnaryMinusAssignmentComplexTest()
    {
        var node = MatherAdv.Convert("x = -(a + -b)");

        node.Should()
            .BeEquivalentTo(
                "x".Assign("a".Plus("b".WithUnaryMinus()).WithUnaryMinus()));
    }
    
    [Test]
    public void UnaryPlusAssignmentTest()
    {
        var node = MatherAdv.Convert("x = +a + +b");

        node.Should().BeEquivalentTo("x".Assign("a".WithUnaryPlus().Plus("b".WithUnaryPlus())));
    }
    
    [Test]
    public void ParenthesesTest()
    {
        var node = MatherAdv.Convert("a + ( b - c ) + d");

        node.Should().BeEquivalentTo(
            "a".Plus("b".Minus("c")).Plus("d"));
    }
    
    [Test]
    public void AssignSimpleTest()
    {
        var node = MatherAdv.Convert("a = b + c");

        node.Should().BeEquivalentTo("a".Assign("b".Plus("c")));
    }
    
    [Test]
    public void AssignSequenceTest()
    {
        var node = MatherAdv.Convert("x = (y = (a = b + c))");

        node.Should().BeEquivalentTo(
            "x".Assign("y".Assign("a".Assign(
                "b".Plus("c")
            ))));
    }
    
    [Test]
    public void AssignWithParenthesesTest()
    {
        var node = MatherAdv.Convert("x = a + ( b - c ) + d");

        node.Should().BeEquivalentTo(
            "x".Assign(
                "a".Plus("b".Minus("c")).Plus("d")));
    }

    [Test]
    public void DotAccessSimpleTest()
    {
        var node = MatherAdv.Convert("x = a.foo.bar");
        node.Should().BeEquivalentTo(
            "x".Assign("a".GetProp("foo").GetProp("bar")));
    }
    
    [Test]
    public void DotPropertyAssignSimpleTest()
    {
        var node = MatherAdv.Convert("a.foo.bar = x");
        var expected = "a".GetProp("foo").SetProp("bar", "x".ToVar());
        node.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
    }

    [Test]
    public void SimpleFuncCall()
    {
        var node = MatherAdv.Convert("x = foo(a, c-(a+b))");
        var expected = "x".Assign("foo".FuncCall(
            "a".ToVar(),
            "c".Minus("a".Plus("b"))
        ));
        node.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
    }

}