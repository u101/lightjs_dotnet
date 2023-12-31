using static App16.ALang.Js.Tests.Ast.AstTestsUtils;

namespace App16.ALang.Js.Tests.Ast;

[TestFixture]
public class VariableDeclarationsTests
{
    [Test]
    public void ConstDeclarationTest()
    {
        var node = BuildAstNode("const a = 123");

        var expected = Const("a", 123.ToLit());
        Match(node, expected);
    }
    
    [Test]
    public void SimpleVarTest()
    {
        var node = BuildAstNode("var a = 123");

        var expected = Var("a", 123.ToLit());
        Match(node, expected);
    }
    
    [Test]
    public void SimpleLetTest()
    {
        var node = BuildAstNode("let a = 123");

        var expected = Let("a", 123.ToLit());
        Match(node, expected);
    }
    
    [Test]
    public void MultipleVarTest()
    {
        var node = BuildAstNode("var a = 123, b\n, c = 'hi'");

        var expected = Sequence(Var("a", 123.ToLit()),
            Var("b"),
            Var("c", "hi".ToLit())
        );
        Match(node, expected);
    }
    
    [Test]
    public void MultipleLetTest()
    {
        var node = BuildAstNode("let a = 123, b\n, c = 'hi'");

        var expected = Sequence(Let("a", 123.ToLit()),
            Let("b"),
            Let("c", "hi".ToLit())
        );
        Match(node, expected);
    }
    
    [Test]
    public void VarDeclarationInCodeTest()
    {
        const string code = """
        function foo() {
            var a,b,c = 123
            if (a) return b
            return c
        }
        """;
        
        var node = BuildAstNode(code);

        var expected = NamedFunc("foo", Sequence(
            
            Sequence(
                Var("a"), Var("b"), Var("c", 123.ToLit())
            ),
            IfBlock("a".ToVar(), Return("b".ToVar())),
            Return("c".ToVar())
            
        ));
            
        Match(node, expected);
    }
    
}