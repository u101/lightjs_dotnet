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


    [Test]
    public void SimpleArrayLiteralTest()
    {
        
        var node = TestUtils.BuildAstNode("x = [1,2,3]");

        var expected = "x".Assign(ArrayLit(1,2,3));
        Match(node, expected);
    }
    
    [Test]
    public void NestedArrayLiteralTest()
    {
        var node = TestUtils.BuildAstNode("x = [1,2,'hi', [], [true, true, false] ]");

        var expected = "x".Assign(ArrayLit(
            1.ToLit(),
            2.ToLit(),
            "hi".ToLit(),
            ArrayLit(),
            ArrayLit(True, True, False)
            ));
        Match(node, expected);
    }
}