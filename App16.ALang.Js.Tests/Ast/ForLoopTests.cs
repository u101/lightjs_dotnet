using static App16.ALang.Js.Tests.Ast.AstTestsUtils;

namespace App16.ALang.Js.Tests.Ast;

[TestFixture]
public class ForLoopTests
{
    [Test]
    public void EmptyLoopTest()
    {
        var node = BuildAstNode("for(;;) {}");

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
        
        var node = BuildAstNode(code);

        var expected = For(
            Var("i", 0.ToLit()),
            "i".LessThen("ln"),
            "i".WithPostfixIncrement(), 
            "foo".FuncCall("i".ToVar()));
        
        Match(node, expected);
    }
    
    [Test]
    public void LoopWithTwoIteratorsTest()
    {
        var code = """
        for (var i = 0, j = 0; i < ln; i++, j++) {
            foo(i)
        }
        """;
        
        var node = BuildAstNode(code);

        var expected = For(
            Sequence(Var("i", 0.ToLit()), Var("j", 0.ToLit())),
            "i".LessThen("ln"),
            Sequence("i".WithPostfixIncrement(), "j".WithPostfixIncrement()), 
            "foo".FuncCall("i".ToVar()));
        
        Match(node, expected);
    }
}