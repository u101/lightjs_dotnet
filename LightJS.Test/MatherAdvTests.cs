using FluentAssertions;
using LightJS.Outsource;
using LightJS.Tokenizer;

namespace LightJS.Test;

[TestFixture]
public class MatherAdvTests
{

    [Test]
    public void SimpleTest()
    {
        var node = MatherAdv.Convert("a + b + 5");
        
        node.Should().BeEquivalentTo(new MatherBinaryOpNode(
            new MatherBinaryOpNode( "a".ToVar(), "b".ToVar(), LjsTokenType.OpPlus),
            "5".ToLit(), LjsTokenType.OpPlus));
    }

    [Test]
    public void IncrementSimpleTest()
    {
        var a = MatherAdv.Convert("++a");
        a.Should().BeEquivalentTo("a".ToVar().WithIncrement());
        
        a = MatherAdv.Convert("a++");
        a.Should().BeEquivalentTo("a".ToVar().WithIncrement());
    }

    [Test]
    public void UnaryMinusSimpleTest()
    {
        var node = MatherAdv.Convert("-a + b");
        
        node.Should().BeEquivalentTo(new MatherBinaryOpNode(
            new MatherUnaryOpNode("a".ToVar(), LjsTokenType.OpMinus),
            "b".ToVar(), LjsTokenType.OpPlus));
    }
    
    [Test]
    public void UnaryMinusAssignmentTest()
    {
        var node = MatherAdv.Convert("x = -a + -b");

        var expect = "x".ToVar().Assign(
            "a".ToVar().WithUnaryMinus().Plus("b".ToVar().WithUnaryMinus()));
        
        node.Should().BeEquivalentTo(expect);
    }
    
    [Test]
    public void UnaryMinusAssignmentWithParentheses()
    {
        var node = MatherAdv.Convert("x = (-(a + b))");

        var expect = "x".ToVar().Assign(
            "a".ToVar().Plus("b".ToVar()).WithUnaryMinus());
        
        node.Should().BeEquivalentTo(expect);
    }
    
    [Test]
    public void UnaryMinusSequence()
    {
        var node = MatherAdv.Convert("x = a - - - - - b");

        var expect = "x".ToVar().Assign(
            "a".ToVar().Minus("b".ToVar().WithUnaryMinus().WithUnaryMinus().WithUnaryMinus().WithUnaryMinus()));
        
        node.Should().BeEquivalentTo(expect);
    }
    
    [Test]
    public void UnaryPlusMinusSequence()
    {
        var node = MatherAdv.Convert("x = a - + - + - b");

        var expect = "x".ToVar().Assign(
            "a".ToVar().Minus("b".ToVar().WithUnaryMinus().WithUnaryPlus().WithUnaryMinus().WithUnaryPlus()));
        
        node.Should().BeEquivalentTo(expect);
    }
    
    [Test]
    public void UnaryMinusAssignmentComplexTest()
    {
        var node = MatherAdv.Convert("x = -(a + -b)");

        var expect = "x".ToVar().Assign(
            "a".ToVar().Plus("b".ToVar().WithUnaryMinus()).WithUnaryMinus());
        
        node.Should().BeEquivalentTo(expect);
    }
    
    [Test]
    public void UnaryPlusAssignmentTest()
    {
        var node = MatherAdv.Convert("x = +a + +b");

        var expect = "x".ToVar().Assign(
            "a".ToVar().WithUnaryPlus().Plus("b".ToVar().WithUnaryPlus()));
        
        node.Should().BeEquivalentTo(expect);
    }
    
    [Test]
    public void ParenthesesTest()
    {
        var node = MatherAdv.Convert("a + ( b - c ) + d");
        
        node.Should().BeEquivalentTo(new MatherBinaryOpNode(
            new MatherBinaryOpNode( 
                "a".ToVar(),
                new MatherBinaryOpNode("b".ToVar(), "c".ToVar(), LjsTokenType.OpMinus),
                LjsTokenType.OpPlus),
            "d".ToVar(), LjsTokenType.OpPlus));
    }
    
    [Test]
    public void AssignSimpleTest()
    {
        var node = MatherAdv.Convert("a = b + c");
        
        node.Should().BeEquivalentTo(new MatherBinaryOpNode(
            "a".ToVar(),
            new MatherBinaryOpNode("b".ToVar(), "c".ToVar(), LjsTokenType.OpMinus),
            LjsTokenType.OpAssign));
    }
    
    [Test]
    public void AssignSequenceTest()
    {
        var node = MatherAdv.Convert("x = y = a = b + c");

        node.Should().BeEquivalentTo("x".ToVar().Assign("y".ToVar().Assign("a".ToVar().Assign(
            "b".ToVar().Plus("c".ToVar())))));
    }
    
    [Test]
    public void AssignWithParenthesesTest()
    {
        var node = MatherAdv.Convert("x = a + ( b - c ) + d");
        
        node.Should().BeEquivalentTo(
            new MatherBinaryOpNode("x".ToVar(),
            new MatherBinaryOpNode(
            new MatherBinaryOpNode( 
                "a".ToVar(),
                new MatherBinaryOpNode("b".ToVar(), "c".ToVar(), LjsTokenType.OpMinus),
                LjsTokenType.OpPlus),
            "d".ToVar(), LjsTokenType.OpPlus),
            LjsTokenType.OpAssign));
    }

}