using static LightJS.Test.Ast.NodesUtils;

namespace LightJS.Test.Ast;

[TestFixture]
public class VariableDeclarationsTests
{

    [Test]
    public void SimpleVarTest()
    {
        var node = TestUtils.BuildAstNode("var a = 123");

        var expected = Var("a", 123.ToLit());
        Match(node, expected);
    }
    
}