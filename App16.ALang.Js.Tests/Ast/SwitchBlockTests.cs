using static App16.ALang.Js.Tests.Ast.AstTestsUtils;

namespace App16.ALang.Js.Tests.Ast;

[TestFixture]
public class SwitchBlockTests
{
    [Test]
    public void EmptySwitchBlockTest()
    {
        const string code = """
        switch (x) {}
        """;
        
        var node = BuildAstNode(code);

        var expected = Switch("x".ToVar(), Sequence());
            
        Match(node, expected);
    }
    
    [Test]
    public void SimpleSwitchBlockTest()
    {
        const string code = """
        switch (x) {
            case 0:
                x++;
            case 1:
                x++;
            default:
                x = a+b
        }
        """;
        
        var node = BuildAstNode(code);

        var expected = Switch("x".ToVar(), Sequence(
            Case(0),
            "x".WithPostfixIncrement(),
            Case(1),
            "x".WithPostfixIncrement(),
            Default,
            "x".Assign("a".Plus("b"))
        ));
            
        Match(node, expected);
    }
    
    [Test]
    public void SwitchBlockWithNestedBlocksTest()
    {
        const string code = """
        switch (x) {
            case 'abc':
                if (foo  <bar) foo = bar;
            case 1:
                switch(p) {
                    case p+1: return 8;
                    case p -2: return NaN;
                }
                break;
        }
        """;
        
        var node = BuildAstNode(code);

        var expected = Switch("x".ToVar(), Sequence(
            Case("abc"),
            IfBlock("foo".LessThen("bar"), "foo".Assign("bar".ToVar())),
            Case(1),
            Switch("p".ToVar(), Sequence(
                Case("p".Plus(1)),
                Return(8.ToLit()),
                Case("p".Minus(2)),
                Return(NaN)
            )),
            Break
        ));
            
        Match(node, expected);
    }

}