using LightJS.Ast;
using static LightJS.Test.Ast.NodesUtils;

namespace LightJS.Test.Ast;

[TestFixture]
public class WhileLoopTests
{

    [Test]
    public void EmptyLoopTest()
    {
        var node = TestUtils.BuildAstNode("while(true) {}");

        var expected = While(True, Nothing);
        Match(node, expected);
    }
    
}