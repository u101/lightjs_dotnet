using App16.ALang.Ast;
using App16.ALang.Js.Errors;
using FluentAssertions;
using static App16.ALang.Js.Tests.Ast.AstTestsUtils;

namespace App16.ALang.Js.Tests.Ast;

[TestFixture]
public class ExpressionsTests
{
    [Test]
    public void NanLiteralTest()
    {
        var rootNode = BuildAstNode("a = NaN");

        var expected = "a".Assign(NaN);
        
        Match(rootNode, expected);
    }

    [Test]
    public void ExponentOperationTest()
    {
        var rootNode = BuildAstNode("a = b **c");
        var expected = "a".Assign("b".Exp("c"));
        
        Match(rootNode, expected);
    }
    
    [Test]
    public void BuildPostfixIncrementExpression()
    {
        var rootNode = BuildAstNode("a++ + b--");

        var expected = "a".WithPostfixIncrement().Plus("b".WithPostfixDecrement());
        
        Match(rootNode, expected);
    }
    
    [Test]
    public void PrefixIncrementExpression()
    {
        var rootNode = BuildAstNode("++a + --b");
        
        Match(rootNode, "a".WithPrefixIncrement().Plus("b".WithPrefixDecrement()));
    }
    
    [Test]
    public void UnaryMinusExpression()
    {
        var rootNode = BuildAstNode("a + -b");

        Match(rootNode, "a".Plus("b".WithUnaryMinus()));
    }
    
    [Test]
    public void UnaryMinusAssignment()
    {
        var rootNode = BuildAstNode("a = -b");

        Match(rootNode, "a".Assign("b".WithUnaryMinus()));
    }
    
    [Test]
    public void SimpleVarAssignTest()
    {
        var rootNode = BuildAstNode("a = b + c");

        Match(rootNode, "a".Assign("b".Plus("c")));
    }

    [Test]
    public void SimplePropertyAssignTest()
    {
        var rootNode = BuildAstNode("a.foo = b + c");

        Match(rootNode, "a".Dot("foo").Assign("b".Plus("c")));
    }

    [Test]
    public void SimpleSquareBracketsPropertyAssignTest()
    {
        var rootNode = BuildAstNode("a[0] = b + c");

        Match(rootNode, "a".Sqb(0.ToLit()).Assign("b".Plus("c")));
    }
    
    [Test]
    public void BuildSimpleExpressionWithParentheses()
    {
        Check("(a+b)-(c+d)");
        Check("((a+b)-(c+d))");
        Check("(((a+b)-(c+d)))");
        Check("(((a+b)-((c+d))))");
        
        void Check(string expression)
        {
            var rootNode = BuildAstNode(expression);
            var expected = "a".Plus("b").Minus("c".Plus("d"));

            Match(rootNode,expected);
        }
    }
    
    
    
    
    [Test]
    public void BuildSimpleLiteral()
    {
        ValidLiteralTest("123456789", 123456789);
        ValidLiteralTest("0xff1267", 0xff1267);
        ValidLiteralTest("0b010101", 0b010101);
        
        ValidLiteralTest("3.14", 3.14);
        ValidLiteralTest("3.14e+3", 3.14e+3);
        
        ValidLiteralTest("true", true);
        ValidLiteralTest("false", false);
        
        ValidLiteralTest<string>("\"Hello world\"", "Hello world");
        ValidLiteralTest<string>("'Hello world'", "Hello world");
    }

    private static void ValidLiteralTest<TLiteralType>(
        string literal, TLiteralType expectedValue)
    {
        var rootNode = BuildAstNode(literal);

        rootNode.Should().BeOfType<AstValueLiteral<TLiteralType>>();
        Match(rootNode,new AstValueLiteral<TLiteralType>(expectedValue));
    }
    
    [Test]
    public void SimpleTest()
    {
        var node = BuildAstNode("a + b + 5");

        Match(node, "a".ToVar().Plus("b".ToVar()).Plus(5.ToLit()));
    }

    [Test]
    public void IncrementSimpleTest()
    {
        var a = BuildAstNode("++a");
        Match(a, "a".WithPrefixIncrement());
        
        a = BuildAstNode("a++");
        Match(a,"a".WithPostfixIncrement());
    }

    [Test]
    public void UnaryMinusSimpleTest()
    {
        var node = BuildAstNode("-a + b");

        Match(node,"a".WithUnaryMinus().Plus("b"));
    }
    
    [Test]
    public void UnaryMinusAssignmentTest()
    {
        var node = BuildAstNode("x = -a + -b");

        Match(node,"x".Assign("a".WithUnaryMinus().Plus("b".WithUnaryMinus())));
    }
    
    [Test]
    public void UnaryMinusAssignmentWithParentheses()
    {
        var node = BuildAstNode("x = (-(a + b))");

        Match(node,"x".Assign("a".Plus("b").WithUnaryMinus()));
    }
    
    [Test]
    public void UnaryMinusSequence()
    {
        var node = BuildAstNode("x = a - - b");

        Match(node,"x".Assign("a".Minus("b".WithUnaryMinus())));
    }
    
    [Test]
    public void UnaryPlusMinusSequence()
    {
        var node = BuildAstNode("x = a - + b");

        Match(node,"x".Assign("a".Minus("b".WithUnaryPlus())));
    }
    
    [Test]
    public void UnaryMinusAssignmentComplexTest()
    {
        var node = BuildAstNode("x = -(a + -b)");

        node.Should()
            .BeEquivalentTo(
                "x".Assign("a".Plus("b".WithUnaryMinus()).WithUnaryMinus()));
    }
    
    [Test]
    public void UnaryPlusAssignmentTest()
    {
        var node = BuildAstNode("x = +a + +b");

        Match(node,"x".Assign("a".WithUnaryPlus().Plus("b".WithUnaryPlus())));
    }
    
    [Test]
    public void ParenthesesTest()
    {
        var node = BuildAstNode("a + ( b - c ) + d");

        Match(node,
            "a".Plus("b".Minus("c")).Plus("d"));
    }

    [Test]
    public void ParenthesesSimpleExpressionTest2()
    {
        var node = BuildAstNode("((a)+(b))");
        Match(node,("a".Plus("b")));
    }
    
    [Test]
    public void ParenthesesSimpleExpressionTest()
    {
        Check("(a+b)-(c+d)");
        Check("((a+b)-(c+d))");
        Check("(((a+b)-(c+d)))");
        Check("(((a+b)-((c+d))))");

        void Check(string expression)
        {
            var node = BuildAstNode(expression);
            Match(node,("a".Plus("b")).Minus("c".Plus("d")));
        }
    }
    
    [Test]
    public void AssignSimpleTest()
    {
        var node = BuildAstNode("a = b + c");

        Match(node,"a".Assign("b".Plus("c")));
    }
    
    [Test]
    public void AssignSequenceTest()
    {
        var node = BuildAstNode("x = y = a = b + c");

        Match(node,
            "x".Assign("y".Assign("a".Assign(
                "b".Plus("c")
            ))));
    }
    
    [Test]
    public void AssignWithParenthesesTest()
    {
        var node = BuildAstNode("x = a + ( b - c ) + d");

        Match(node,
            "x".Assign(
                "a".Plus("b".Minus("c")).Plus("d")));
    }

    [Test]
    public void DotAccessSimpleTest()
    {
        var node = BuildAstNode("x = a.foo.bar");
        Match(node,
            "x".Assign("a".Dot("foo").Dot("bar")));
    }
    
    [Test]
    public void DotPropertyAssignSimpleTest()
    {
        var node = BuildAstNode("a.foo.bar = x");
        var expected = "a".Dot("foo").Dot("bar").Assign("x".ToVar());
        Match(node,expected);
    }
    
    [Test]
    public void BracketsPropertySimpleTest()
    {
        var node = BuildAstNode("x = a['foo']");
        var expected = "x".Assign("a".Sqb("foo".ToLit()));
        Match(node,expected);
    }
    
    [Test]
    public void BracketsPropertyNestedSimpleTest()
    {
        var node = BuildAstNode("x = a[foo[0]]");
        var expected = "x".Assign("a".Sqb("foo".Sqb(0.ToLit())));
        Match(node,expected);
    }
    
    [Test]
    public void BracketsPropertyAssignSimpleTest()
    {
        var node = BuildAstNode("a['foo'] = x");
        var expected = "a".Sqb("foo".ToLit()).Assign("x".ToVar());
        Match(node,expected);
    }
    
    [Test]
    public void BuildSimpleTernaryIfExpression()
    {
        var node = BuildAstNode("a ? b : c");
        var expected = "a".Tif("b".ToVar(), "c".ToVar());
        Match(node,expected);
    }
    
    [Test]
    public void BuildNestedInParenthesesTernaryIfExpression()
    {
        var node = BuildAstNode("a ? (x?y:z) : c");
        var expected = "a".Tif(
            "x".Tif("y".ToVar(), "z".ToVar()), 
            "c".ToVar());
        Match(node,expected);
    }
    
    [Test]
    public void BuildNestedTernaryIfExpression()
    {
        var node = BuildAstNode("a ? b ? b1 : b2 : c");
        
        var expected = "a".Tif(
            "b".Tif("b1".ToVar(), "b2".ToVar()), 
            "c".ToVar());
        
        Match(node,expected);
    }
    
    [Test]
    public void BuildTernaryIfExpressionWithAssignment()
    {
        var node = BuildAstNode("x = a ? b : c");
        var expected = "x".Assign("a".Tif("b".ToVar(), "c".ToVar()));
        
        Match(node,expected);
    }
    
    [Test]
    public void BuildSimpleExpressionWithOperatorsPriority()
    {
        var rootNode = BuildAstNode("a + b * c + d");

        var expectedResult = "a".Plus("b".Mul("c")).Plus("d");

        Match(rootNode, expectedResult);
    }
    
    [Test]
    public void BuildSimpleLogicalExpression()
    {
        var rootNode = BuildAstNode("a + b != c + d");
        
        rootNode.Should().BeOfType<AstBinaryOperation>();

        Match(rootNode, ("a".Plus("b")).NotEq("c".Plus("d")));
    }
    
    [Test]
    public void BuildInvalidParenthesesExpression()
    {
        BuildInvalidExpression("(a+b))");
        BuildInvalidExpression("(a+b");
    }
    
    [Test]
    public void BuildInvalidOperatorsExpression()
    {
        BuildInvalidExpression("a * * * b");
    }

    private static void BuildInvalidExpression(string expression)
    {
        Assert.Throws<JsSyntaxError>(() => BuildAstNode(expression));
    }
}