using static App16.ALang.Tests.Ast.AstTestsUtils;

namespace App16.ALang.Tests.Ast;

[TestFixture]
public class UnaryExpressionsTests
{
    
    [Test]
    public void ExpressionTest_5()
    {
        var result = BuildAstNode("x = ++y - a++");
        var expected = "x".Assign("y".WithPrefixIncrement().Minus("a".WithPostfixIncrement()));
        Match(result, expected);
    }
    
    [Test]
    public void ExpressionTest_4()
    {
        var result = BuildAstNode("x = y++ - ++a");
        var expected = "x".Assign("y".WithPostfixIncrement().Minus("a".WithPrefixIncrement()));
        Match(result, expected);
    }
    
    [Test]
    public void ExpressionTest_3()
    {
        var result = BuildAstNode("x = y - - - -(a++)");
        var expected = "x".Assign("y".Minus("a".WithPostfixIncrement().WithUnaryMinus().WithUnaryMinus().WithUnaryMinus() ));
        Match(result, expected);
    }
    
    
    [Test]
    public void ExpressionTest_2()
    {
        var result = BuildAstNode("x = y - - - -a");
        var expected = "x".Assign("y".Minus("a".WithUnaryMinus().WithUnaryMinus().WithUnaryMinus() ));
        Match(result, expected);
    }
    
    [Test]
    public void ExpressionTest_1()
    {
        var result = BuildAstNode("x = -y + -a");
        var expected = "x".Assign("y".WithUnaryMinus().Plus("a".WithUnaryMinus()));
        Match(result, expected);
    }
    

    [Test]
    public void ExpressionTest_0()
    {
        var result = BuildAstNode("x = -y");
        var expected = "x".Assign("y".WithUnaryMinus());
        Match(result, expected);
    }
}