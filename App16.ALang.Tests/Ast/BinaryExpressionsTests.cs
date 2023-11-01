using static App16.ALang.Tests.Ast.AstTestsUtils;

namespace App16.ALang.Tests.Ast;

[TestFixture]
public class BinaryExpressionsTests
{
    
    [Test]
    public void SimpleExpressionTest4()
    {
        var result = BuildAstNode("x = (a+ b)*c");
        var expected = "x".Assign("a".Plus("b").Mul("c"));
        Match(result, expected);
    }
    
    [Test]
    public void SimpleExpressionTest4_()
    {
        var result = BuildAstNode("y = x = (a+ b)*c");
        var expected = "y".Assign("x".Assign("a".Plus("b").Mul("c")));
        Match(result, expected);
    }
    
    [Test]
    public void SimpleExpressionTest5()
    {
        var result = BuildAstNode("y += x = (a+ b)*c");
        var expected = "y".PlusAssign("x".Assign("a".Plus("b").Mul("c")));
        Match(result, expected);
    }
    
    
    [Test]
    public void SimpleExpressionTest3()
    {
        var result = BuildAstNode("(a+ b)*c");
        var expected = "a".Plus("b").Mul("c");
        Match(result, expected);
    }
    
    [Test]
    public void SimpleExpressionTest2()
    {
        var result = BuildAstNode("a+ b*c");
        var expected = "a".Plus("b".Mul("c"));
        Match(result, expected);
    }
    
    [Test]
    public void SimpleExpressionTest()
    {
        var result = BuildAstNode("a+b");
        var expected = "a".Plus("b");
        Match(result, expected);
    }
    

    [Test]
    [TestCase("xxx")]
    [TestCase("a")]
    [TestCase("u1234_")]
    public void IdTest(string id)
    {
        var result = BuildAstNode(id);
        var expected = id.ToVar();
        Match(result, expected);
    }
}