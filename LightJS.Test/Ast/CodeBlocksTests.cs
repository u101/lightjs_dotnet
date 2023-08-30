namespace LightJS.Test.Ast;

[TestFixture]
public class CodeBlocksTests
{

    [Test]
    public void SimpleExpressionsSequenceTest()
    {
        var node = BuildAstNode("a = b + c;\nx = y - z");
        
        var expected = Sequence(
            "a".Assign("b".Plus("c")),
            "x".Assign("y".Minus("z"))
        );
        Match(node, expected);
    }
    
    [Test]
    public void SimpleExpressionsSequenceWithRedundantSemicolonsTest()
    {
        var node = BuildAstNode(";a = b + c;;;\n;\n;;x = y - z;;;");
        
        var expected = Sequence(
            "a".Assign("b".Plus("c")),
            "x".Assign("y".Minus("z"))
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
            var node = BuildAstNode(expression);
            var expected = Sequence(
                "a".Assign("b".ToVar()),
                "y".Assign("x".WithPrefixIncrement())
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
            var node = BuildAstNode(expression);
            var expected = Sequence(
                "a".Plus("b"),
                "x".WithPrefixIncrement().MultiplyBy("y"));
            Match(node, expected);
        }
    }
    
}