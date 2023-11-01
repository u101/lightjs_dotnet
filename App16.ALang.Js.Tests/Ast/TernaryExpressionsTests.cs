using static App16.ALang.Js.Tests.Ast.AstTestsUtils;

namespace App16.ALang.Js.Tests.Ast;

[TestFixture]
public class TernaryExpressionsTests
{
    
    [Test]
    public void ExpressionTest_4()
    {
        var result = BuildAstNode("x = a1 + a2 ? !b0 - z ? b1 * b0 : b2 - b1 : c++");
        var expected = "x".Assign(
            "a1".Plus("a2").Tif(
                "b0".WithUnaryNot().Minus("z").Tif("b1".Mul("b0"), "b2".Minus("b1")),
                "c".WithPostfixIncrement()));
        Match(result, expected);
    }
    
    
    [Test]
    public void ExpressionTest_3()
    {
        var result = BuildAstNode("x = a ? (b ? b1 : b2) : c");
        var expected = "x".Assign("a".Tif("b".Tif("b1", "b2"),"c".ToVar()));
        Match(result, expected);
    }
    
    
    [Test]
    public void ExpressionTest_2()
    {
        var result = BuildAstNode("x = a ? b ? b1 : b2 : c");
        var expected = "x".Assign("a".Tif("b".Tif("b1", "b2"),"c".ToVar()));
        Match(result, expected);
    }
    
    [Test]
    public void ExpressionTest_1()
    {
        var result = BuildAstNode("x = a ? b : c");
        var expected = "x".Assign("a".Tif("b","c"));
        Match(result, expected);
    }
    
    [Test]
    public void ExpressionTest_0()
    {
        var result = BuildAstNode("a ? b : c");
        var expected = "a".Tif("b","c");
        Match(result, expected);
    }
}