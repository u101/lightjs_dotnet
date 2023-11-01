using static App16.ALang.Tests.Ast.AstTestsUtils;

namespace App16.ALang.Tests.Ast;

[TestFixture]
public class DotPropertyExpressionsTests
{
    
    [Test]
    public void ExpressionTest_1()
    {
        var result = BuildAstNode("x.buz = -a.foo + b.bar++");
        var expected = "x".Dot("buz").Assign("a".Dot("foo").WithUnaryMinus().Plus("b".Dot("bar").WithPostfixIncrement()));
        Match(result, expected);
    }
    
    [Test]
    public void ExpressionTest_0()
    {
        var result = BuildAstNode("x = a.foo + b.bar");
        var expected = "x".Assign("a".Dot("foo").Plus("b".Dot("bar")));
        Match(result, expected);
    }
    
}