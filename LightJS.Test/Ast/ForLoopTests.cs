using LightJS.Ast;
using static LightJS.Test.Ast.NodesUtils;

namespace LightJS.Test.Ast;

[TestFixture]
public class ForLoopTests
{
    [Test]
    public void EmptyLoopTest()
    {
        var node = TestUtils.BuildAstNode("for(;;) {}");

        var expected = For(Nothing,Nothing,Nothing, Nothing);
        Match(node, expected);
    }
}