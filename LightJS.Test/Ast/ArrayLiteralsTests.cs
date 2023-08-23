using static LightJS.Test.Ast.NodesUtils;

namespace LightJS.Test.Ast;

[TestFixture]
public class ArrayLiteralsTests
{

    [Test]
    public void EmptyArrayLiteralTest()
    {
        var node = TestUtils.BuildAstNode("x = []");

        var expected = "x".Assign(ArrayLit());
        Match(node, expected);
    }
    
}