using static App16.ALang.Js.Tests.Ast.AstTestsUtils;

namespace App16.ALang.Js.Tests.Ast;

[TestFixture]
public class SqbPropertyExpressionsTests
{
    [Test]
    public void ExpressionTest_0()
    {
        var result = BuildAstNode("x = a[foo] + b[bar]");
        var expected = "x".Assign("a".Sqb("foo").Plus("b".Sqb("bar")));
        Match(result, expected);
    }
}