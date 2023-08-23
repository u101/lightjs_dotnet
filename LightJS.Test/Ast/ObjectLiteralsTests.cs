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

    [Test]
    public void SimpleObjectLiteralTest()
    {
        var node = TestUtils.BuildAstNode("x = { a:1, b:2, c:3}");

        var expected = "x".Assign(
            ObjectLit()
                .AddProp("a", 1)
                .AddProp("b", 2)
                .AddProp("c", 3));
        
        Match(node, expected);
    }
    
}