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

    [Test]
    public void SimpleBlockWithBracketsTest()
    {
        var code = """
        while(a) {
            x++
        }
        """;

        var node = TestUtils.BuildAstNode(code);

        var expected = While(
                "a".ToVar(), "x".ToVar().WithPostfixIncrement());
            
        Match(node, expected);
    }
    
    [Test]
    public void SimpleBlockWithoutBracketsTest()
    {

        var node = TestUtils.BuildAstNode("while(a) x++");

        var expected = While(
                "a".ToVar(), "x".ToVar().WithPostfixIncrement());
            
        Match(node, expected);
    }

}