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
    
    [Test]
    public void SimpleLoopTest()
    {
        var code = """
        for (var i = 0; i < ln; i++) {
            foo(i)
        }
        """;
        
        var node = TestUtils.BuildAstNode(code);

        var expected = For(
            Var("i", 0.ToLit()),
            "i".LessThen("ln"),
            "i".WithPostfixIncrement(), 
            "foo".FuncCall("i".ToVar()));
        
        Match(node, expected);
    }
}