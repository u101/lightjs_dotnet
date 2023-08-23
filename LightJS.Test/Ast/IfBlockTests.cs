using LightJS.Ast;
using static LightJS.Test.Ast.NodesUtils;

namespace LightJS.Test.Ast;

[TestFixture]
public class IfBlockTests
{
    [Test]
    public void SimpleIfBlockTest()
    {
        CheckVariant("""
        if (a) {
            x = b
        } else {
            x = c
        }
        """);
        
        CheckVariant("""
        if (a)
            x = b
        else
            x = c
        """);
        
        CheckVariant("""
        if (a) x = b
        else
            x = c
        """);
        
        CheckVariant("""
        if (a) x = b
        else x = c
        """);
        
        CheckVariant("if (a) x = b; else x = c");
        CheckVariant("if (a) {x = b} else x = c");

        void CheckVariant(string code)
        {
            var node = TestUtils.BuildAstNode(code);

            var expected = IfBlock(
                    "a".ToVar(), "x".Assign("b".ToVar())).
            
                Else("x".Assign("c".ToVar()));
            
            Match(node, expected);
        }
        
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
        
        var node = TestUtils.BuildAstNode(code);

        var expected =
            IfBlock(
                    "a".ToVar(),
                    IfBlock(
                        "a".GetProp("a"), 
                        "x".Assign("a".GetProp("a")))
                )
                .Else(IfBlock(
                    "a".GetProp("b"), 
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
        
        var node = TestUtils.BuildAstNode(code);

        var expected =
            IfBlock(
                    "a".ToVar(),
                    IfBlock(
                        "a".GetProp("a"),
                        "x".Assign("a".GetProp("a")))
                )
                .ElseIf(
                    "b".ToVar(),
                    IfBlock(
                        "a".GetProp("c"),
                        "x".Assign("a".GetProp("c")))
                )
                .Else(IfBlock(
                    "a".GetProp("b"),
                    "x".Assign("b".ToVar()))
                );
        Match(node, expected);
    }
}