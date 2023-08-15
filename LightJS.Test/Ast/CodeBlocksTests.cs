using static LightJS.Test.Ast.NodesUtils;

namespace LightJS.Test.Ast;

[TestFixture]
public class CodeBlocksTests
{

    [Test]
    public void SimpleExpressionsSequenceTest()
    {
        var node = TestUtils.BuildAstNode("a = b + c;\nx = y - z");
        
        var expected = Sequence(
            "a".Assign("b".Plus("c")),
            "x".Assign("y".Minus("z"))
        );
        Match(node, expected);
    }
    
    [Test]
    public void SimpleExpressionsSequenceWithRedundantSemicolonsTest()
    {
        var node = TestUtils.BuildAstNode(";a = b + c;;;\n;\n;;x = y - z;;;");
        
        var expected = Sequence(
            "a".Assign("b".Plus("c")),
            "x".Assign("y".Minus("z"))
        );
        Match(node, expected);
    }
    
    [Test]
    public void SimpleIfBlockTest()
    {
        var code = """
        if (a)
            x = b
        else
            x = c
        """;
        
        var node = TestUtils.BuildAstNode(code);

        var expected = IfBlock(
            "a".ToVar(), "x".Assign("b".ToVar())).
            
            Else("x".Assign("c".ToVar()));
            
        Match(node, expected);
    }
    
}