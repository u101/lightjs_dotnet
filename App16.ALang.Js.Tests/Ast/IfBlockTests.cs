using static App16.ALang.Js.Tests.Ast.AstTestsUtils;

namespace App16.ALang.Js.Tests.Ast;

[TestFixture]
public class IfBlockTests
{
    [Test]
    public void ExpressionTest_0()
    {
        const string code = """
        if (a) {
            x = b
        } else {
            x = c
        }
        """;
        var node = BuildAstNode(code);

        var expected = IfBlock(
                "a".ToVar(), "x".Assign("b".ToVar())).
            
            Else("x".Assign("c".ToVar()));
            
        Match(node, expected);
    }
    
    [Test]
    public void ExpressionTest_1()
    {
        const string code = """
        if (a)
            x = b
        else
            x = c
        """;
        var node = BuildAstNode(code);

        var expected = IfBlock(
                "a".ToVar(), "x".Assign("b".ToVar())).
            
            Else("x".Assign("c".ToVar()));
            
        Match(node, expected);
    }
    
    [Test]
    public void ExpressionTest_2()
    {
        const string code = """
        if (a) x = b
        else
            x = c
        """;
        var node = BuildAstNode(code);

        var expected = IfBlock(
                "a".ToVar(), "x".Assign("b".ToVar())).
            
            Else("x".Assign("c".ToVar()));
            
        Match(node, expected);
    }
    
    [Test]
    public void ExpressionTest_3()
    {
        const string code = """
        if (a) x = b
        else x = c
        """;
        var node = BuildAstNode(code);

        var expected = IfBlock(
                "a".ToVar(), "x".Assign("b".ToVar())).
            
            Else("x".Assign("c".ToVar()));
            
        Match(node, expected);
    }
    
    [Test]
    public void ExpressionTest_4()
    {
        var node = BuildAstNode("if (a) x = b; else x = c");

        var expected = IfBlock(
                "a".ToVar(), "x".Assign("b".ToVar())).
            
            Else("x".Assign("c".ToVar()));
            
        Match(node, expected);
    }
    
    [Test]
    public void ExpressionTest_5()
    {
        var node = BuildAstNode("if (a) {x = b} else x = c");

        var expected = IfBlock(
                "a".ToVar(), "x".Assign("b".ToVar())).
            
            Else("x".Assign("c".ToVar()));
            
        Match(node, expected);
    }
    
    [Test]
    public void NestedIfBlockTest()
    {
        var code = """
        if (a) {
            if (a.a) x = a.a
        } else {
            if (a.b) x = b
        }
        """;
        
        var node = BuildAstNode(code);

        var expected =
            IfBlock(
                    "a".ToVar(),
                    IfBlock(
                        "a".Dot("a"), 
                        "x".Assign("a".Dot("a")))
                )
                .Else(IfBlock(
                    "a".Dot("b"), 
                    "x".Assign("b".ToVar()))
                );
        Match(node, expected);
    }

    [Test]
    public void AltIfBlockTest()
    {
        var code = """
        if (a) {
            if (a.a) x = a.a
        } 
        else if (b) {
            if (a.c) x = a.c
        } else {
            if (a.b) x = b
        }
        """;
        
        var node = BuildAstNode(code);

        var expected =
            IfBlock(
                    "a".ToVar(),
                    IfBlock(
                        "a".Dot("a"),
                        "x".Assign("a".Dot("a")))
                )
                .ElseIf(
                    "b".ToVar(),
                    IfBlock(
                        "a".Dot("c"),
                        "x".Assign("a".Dot("c")))
                )
                .Else(IfBlock(
                    "a".Dot("b"),
                    "x".Assign("b".ToVar()))
                );
        Match(node, expected);
    }
}