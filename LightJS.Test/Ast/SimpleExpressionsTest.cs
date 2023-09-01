using LightJS.Ast;
using FluentAssertions;
using LightJS.Errors;

namespace LightJS.Test.Ast;

[TestFixture]
public class SimpleExpressionsTest
{
    [Test]
    public void BuildPostfixIncrementExpression()
    {
        var rootNode = BuildAstNode("a++ + b--");

        var expected = "a".WithPostfixIncrement().Plus("b".WithPostfixDecrement());
        
        Match(rootNode, expected);
    }
    
    [Test]
    public void BuildPrefixIncrementExpression()
    {
        var rootNode = BuildAstNode("++a + --b");

        var expected = "a".WithPrefixIncrement().Plus("b".WithPrefixDecrement());
        
        Match(rootNode, expected);
    }
    
    [Test]
    public void BuildUnaryMinusExpression()
    {
        var rootNode = BuildAstNode("a + -b");

        Match(rootNode,
            new LjsAstBinaryOperation(
                new LjsAstGetVar("a"), 
                new LjsAstUnaryOperation(new LjsAstGetVar("b"), LjsAstUnaryOperationType.Minus), 
                LjsAstBinaryOperationType.Plus));
    }
    
    [Test]
    public void BuildSimpleUnaryMinusAssignment()
    {
        var rootNode = BuildAstNode("a = -b");

        Match(rootNode,
            new LjsAstSetVar(
                "a", 
                new LjsAstUnaryOperation(new LjsAstGetVar("b"), LjsAstUnaryOperationType.Minus), 
                LjsAstAssignMode.Normal));
    }

    [Test]
    public void BuildSimpleExpression()
    {
        var rootNode = BuildAstNode("a + b");

        Match(rootNode,
            new LjsAstBinaryOperation(
                new LjsAstGetVar("a"), 
                new LjsAstGetVar("b"), 
                LjsAstBinaryOperationType.Plus));
    }
    
    [Test]
    public void SimpleVarAssignTest()
    {
        var rootNode = BuildAstNode("a = b + c");

        Match(rootNode,
            new LjsAstSetVar("a",
                new LjsAstBinaryOperation(
                    new LjsAstGetVar("b"),
                    new LjsAstGetVar("c"),
                    LjsAstBinaryOperationType.Plus),
                LjsAstAssignMode.Normal));
    }

    [Test]
    public void SimplePropertyAssignTest()
    {
        var rootNode = BuildAstNode("a.foo = b + c");

        Match(rootNode,
            new LjsAstSetNamedProperty("foo", new LjsAstGetVar("a"),
                new LjsAstBinaryOperation(
                    new LjsAstGetVar("b"),
                    new LjsAstGetVar("c"),
                    LjsAstBinaryOperationType.Plus),
                LjsAstAssignMode.Normal));
    }

    [Test]
    public void SimpleSquareBracketsPropertyAssignTest()
    {
        var rootNode = BuildAstNode("a[0] = b + c");

        Match(rootNode,
            new LjsAstSetProperty(new LjsAstLiteral<int>(0), new LjsAstGetVar("a"),
                new LjsAstBinaryOperation(
                    new LjsAstGetVar("b"),
                    new LjsAstGetVar("c"),
                    LjsAstBinaryOperationType.Plus),
                LjsAstAssignMode.Normal));
    }
    
    
    
    
    
    /*[Test]
    public void SimpleNodesPositionsInSourceCodeCheck()
    {
        var astBuilder = new LjsAstBuilder("a + b");
        var model = astBuilder.Build();

        var rootNode = model.RootNode;

        rootNode.Should().BeOfType<LjsAstBinaryOperation>();

        var binOp = (LjsAstBinaryOperation)rootNode;
        
        Assert.That(model.HasTokenPositionForNode(binOp), Is.True);
        Assert.That(model.GetTokenPositionForNode(binOp), Is.EqualTo(new LjsTokenPosition(2,0,2)));
        
        Assert.That(model.HasTokenPositionForNode(binOp.LeftOperand), Is.True);
        Assert.That(model.HasTokenPositionForNode(binOp.RightOperand), Is.True);
        
        Assert.That(model.GetTokenPositionForNode(binOp.LeftOperand), Is.EqualTo(new LjsTokenPosition(0,0,0)));
        Assert.That(model.GetTokenPositionForNode(binOp.RightOperand), Is.EqualTo(new LjsTokenPosition(4,0,4)));
        
    }*/
    
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
        
            rootNode.Should().BeOfType<LjsAstBinaryOperation>();

            Match(rootNode,
                new LjsAstBinaryOperation(
                    new LjsAstBinaryOperation(new LjsAstGetVar("a"), new LjsAstGetVar("b"), LjsAstBinaryOperationType.Plus),
                    new LjsAstBinaryOperation(new LjsAstGetVar("c"), new LjsAstGetVar("d"), LjsAstBinaryOperationType.Plus),
                    LjsAstBinaryOperationType.Minus));
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

        rootNode.Should().BeOfType<LjsAstLiteral<TLiteralType>>();
        Match(rootNode,new LjsAstLiteral<TLiteralType>(expectedValue));
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
            "x".Assign("a".GetProp("foo").GetProp("bar")));
    }
    
    [Test]
    public void DotPropertyAssignSimpleTest()
    {
        var node = BuildAstNode("a.foo.bar = x");
        var expected = "a".GetProp("foo").SetProp("bar", "x".ToVar());
        Match(node,expected);
    }
    
    [Test]
    public void BracketsPropertySimpleTest()
    {
        var node = BuildAstNode("x = a['foo']");
        var expected = "x".Assign("a".GetProp("foo".ToLit()));
        Match(node,expected);
    }
    
    [Test]
    public void BracketsPropertyNestedSimpleTest()
    {
        var node = BuildAstNode("x = a[foo[0]]");
        var expected = "x".Assign("a".GetProp("foo".GetProp(0.ToLit())));
        Match(node,expected);
    }
    
    [Test]
    public void BracketsPropertyAssignSimpleTest()
    {
        var node = BuildAstNode("a['foo'] = x");
        var expected = "a".SetProp("foo".ToLit(), "x".ToVar());
        Match(node,expected);
    }
    
    [Test]
    public void BuildSimpleTernaryIfExpression()
    {
        var node = BuildAstNode("a ? b : c");
        var expected = "a".ToVar().TernaryIf("b".ToVar(), "c".ToVar());
        Match(node,expected);
    }
    
    [Test]
    public void BuildNestedInParenthesesTernaryIfExpression()
    {
        var node = BuildAstNode("a ? (x?y:z) : c");
        var expected = "a".ToVar().TernaryIf(
            "x".ToVar().TernaryIf("y".ToVar(), "z".ToVar()), 
            "c".ToVar());
        Match(node,expected);
    }
    
    [Test]
    public void BuildNestedTernaryIfExpression()
    {
        var node = BuildAstNode("a ? b ? b1 : b2 : c");
        
        var expected = "a".ToVar().TernaryIf(
            "b".ToVar().TernaryIf("b1".ToVar(), "b2".ToVar()), 
            "c".ToVar());
        
        Match(node,expected);
    }
    
    [Test]
    public void BuildTernaryIfExpressionWithAssignment()
    {
        var node = BuildAstNode("x = a ? b : c");
        var expected = "x".Assign("a".ToVar().TernaryIf("b".ToVar(), "c".ToVar()));
        
        Match(node,expected);
    }
    
    [Test]
    public void BuildSimpleExpressionWithOperatorsPriority()
    {
        var rootNode = BuildAstNode("a + b/c + d");
        
        rootNode.Should().BeOfType<LjsAstBinaryOperation>();

        var expectedResult = new LjsAstBinaryOperation(
            new LjsAstBinaryOperation(
                new LjsAstGetVar("a"),
                new LjsAstBinaryOperation(new LjsAstGetVar("b"), new LjsAstGetVar("c"), LjsAstBinaryOperationType.Div),
                LjsAstBinaryOperationType.Plus),
            new LjsAstGetVar("d"),
            LjsAstBinaryOperationType.Plus);

        Match(rootNode, expectedResult);
    }
    
    [Test]
    public void BuildSimpleLogicalExpression()
    {
        var rootNode = BuildAstNode("a + b != c + d");
        
        rootNode.Should().BeOfType<LjsAstBinaryOperation>();
        
        var expectedResult = new LjsAstBinaryOperation(
            new LjsAstBinaryOperation(new LjsAstGetVar("a"), new LjsAstGetVar("b"), LjsAstBinaryOperationType.Plus),
            new LjsAstBinaryOperation(new LjsAstGetVar("c"), new LjsAstGetVar("d"), LjsAstBinaryOperationType.Plus),
            LjsAstBinaryOperationType.NotEqual);

        Match(rootNode, expectedResult);
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
        Assert.Throws<LjsSyntaxError>(() => BuildAstNode(expression));
    }

    
}