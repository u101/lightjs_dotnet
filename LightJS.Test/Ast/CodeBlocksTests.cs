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
    
    [Test]
    public void BuildAssignMultilineExpression()
    {
        CheckVariant("a = b; \n y = ++x");
        CheckVariant("a = b;y = ++ x");
        CheckVariant("a = b\ny =++ x");
        CheckVariant("a\n = b\ny =++ x");
        CheckVariant("a = \nb\ny =++ x");
        CheckVariant("a = \nb\ny =\n++ x");

        void CheckVariant(string expression)
        {
            var node = TestUtils.BuildAstNode(expression);
            var expected = Sequence(
                "a".Assign("b".ToVar()),
                "y".Assign("x".ToVar().WithPrefixIncrement())
            );
            Match(node, expected);
        }
        
    }
    
    [Test]
    public void BuildSimpleMultilineExpression()
    {
        CheckVariant("a + b; \n ++x * y");
        CheckVariant("a + b;++x * y");
        CheckVariant("a + b\n++x * y");
        CheckVariant("a\n + b\n++x * y");
        CheckVariant("a + \nb\n++x * y");
        
        void CheckVariant(string expression)
        {
            var node = TestUtils.BuildAstNode(expression);
            var expected = Sequence(
                "a".Plus("b"),
                "x".ToVar().WithPrefixIncrement().MultiplyBy("y"));
            Match(node, expected);
        }
    }
    
}