using FluentAssertions;
using LightJS.Outsource;

namespace LightJS.Test;

[TestFixture]
public class MatherAdvTests
{

    [Test]
    public void SimpleTest()
    {
        var node = MatherAdv.Convert(
            MatherTokensParser.Parse("a + b + 5"));
        
        node.Should().BeEquivalentTo(new MatherBinaryOpNode(
            new MatherBinaryOpNode( "a".ToVar(), "b".ToVar(), MatherTokenType.OpPlus),
            "5".ToLit(), MatherTokenType.OpPlus));
    }
    
    [Test]
    public void ParenthesesTest()
    {
        var node = MatherAdv.Convert(
            MatherTokensParser.Parse("a + ( b - c ) + d"));
        
        node.Should().BeEquivalentTo(new MatherBinaryOpNode(
            new MatherBinaryOpNode( 
                "a".ToVar(),
                new MatherBinaryOpNode("b".ToVar(), "c".ToVar(), MatherTokenType.OpMinus),
                MatherTokenType.OpPlus),
            "d".ToVar(), MatherTokenType.OpPlus));
    }
    
    [Test]
    public void AssignSimpleTest()
    {
        var node = MatherAdv.Convert(
            MatherTokensParser.Parse("a = b + c"));
        
        node.Should().BeEquivalentTo(new MatherBinaryOpNode(
            "a".ToVar(),
            new MatherBinaryOpNode("b".ToVar(), "c".ToVar(), MatherTokenType.OpMinus),
            MatherTokenType.OpAssign));
    }
    
    [Test]
    public void AssignWithParenthesesTest()
    {
        var node = MatherAdv.Convert(
            MatherTokensParser.Parse("x = a + ( b - c ) + d"));
        
        node.Should().BeEquivalentTo(
            new MatherBinaryOpNode("x".ToVar(),
            new MatherBinaryOpNode(
            new MatherBinaryOpNode( 
                "a".ToVar(),
                new MatherBinaryOpNode("b".ToVar(), "c".ToVar(), MatherTokenType.OpMinus),
                MatherTokenType.OpPlus),
            "d".ToVar(), MatherTokenType.OpPlus),
            MatherTokenType.OpAssign));
    }
    
}