using static LightJS.Test.Ast.NodesUtils;

namespace LightJS.Test.Ast;

[TestFixture]
public class ObjectLiteralsTests
{
    [Test]
    public void EmptyObjectLiteralTest()
    {
        var node = TestUtils.BuildAstNode("x = {}");

        var expected = "x".Assign(ObjectLit());
        Match(node, expected);
    }
}