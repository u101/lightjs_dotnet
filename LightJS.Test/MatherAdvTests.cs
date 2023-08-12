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