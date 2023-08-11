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
        
        Assert.That(node, Is.EqualTo(new MatherBinaryOpNode(
            new MatherGetVarNode("a"),
            new MatherBinaryOpNode(
                new MatherGetVarNode("b"),
                new MatherLiteralNode("5"), MatherBinaryOp.Plus),
            MatherBinaryOp.Plus)));
    }
    
}