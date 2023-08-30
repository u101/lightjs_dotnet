namespace LightJS.Test.Ast;

[TestFixture]
public class ArrayLiteralsTests
{

    [Test]
    public void BinaryOperationTest()
    {
        var node = BuildAstNode("x = [1,2,3] + [4,5,6]");

        var expected = "x".Assign(ArrayLit(1,2,3).Plus(ArrayLit(4,5,6)));
        Match(node, expected);
    }
    
    [Test]
    public void EmptyArrayLiteralTest()
    {
        var node = BuildAstNode("x = []");

        var expected = "x".Assign(ArrayLit());
        Match(node, expected);
    }


    [Test]
    public void SimpleArrayLiteralTest()
    {
        
        var node = BuildAstNode("x = [1,2,3]");

        var expected = "x".Assign(ArrayLit(1,2,3));
        Match(node, expected);
    }
    
    [Test]
    public void NestedArrayLiteralTest()
    {
        var node = BuildAstNode("x = [1,2,'hi', [], [true, true, false] ]");

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