using LightJS.Ast;
using FluentAssertions;

namespace LightJS.Test;

[TestFixture]
public class LjsAstSimpleExpressionsTest
{
    [Test]
    public void BuildPostfixIncrementExpression()
    {
        var rootNode = TestUtils.BuildAstNode("a++ + b--");

        rootNode.Should().BeEquivalentTo(
            new LjsAstBinaryOperation(
                new LjsAstUnaryOperation(new LjsAstGetVar("a"), LjsAstUnaryOperationType.PostfixIncrement), 
                new LjsAstUnaryOperation(new LjsAstGetVar("b"), LjsAstUnaryOperationType.PostfixIncrement), 
                LjsAstBinaryOperationType.Plus));
    }
    
    [Test]
    public void BuildPrefixIncrementExpression()
    {
        var rootNode = TestUtils.BuildAstNode("++a + --b");

        rootNode.Should().BeEquivalentTo(
            new LjsAstBinaryOperation(
                new LjsAstUnaryOperation(new LjsAstGetVar("a"), LjsAstUnaryOperationType.PrefixIncrement), 
                new LjsAstUnaryOperation(new LjsAstGetVar("b"), LjsAstUnaryOperationType.PrefixDecrement), 
                LjsAstBinaryOperationType.Plus));
    }
    
    [Test]
    public void BuildUnaryMinusExpression()
    {
        var rootNode = TestUtils.BuildAstNode("a + -b");

        rootNode.Should().BeEquivalentTo(
            new LjsAstBinaryOperation(
                new LjsAstGetVar("a"), 
                new LjsAstUnaryOperation(new LjsAstGetVar("b"), LjsAstUnaryOperationType.Minus), 
                LjsAstBinaryOperationType.Plus));
    }
    
    [Test]
    public void BuildSimpleUnaryMinusAssignment()
    {
        var rootNode = TestUtils.BuildAstNode("a = -b");

        rootNode.Should().BeEquivalentTo(
            new LjsAstSetVar(
                "a", 
                new LjsAstUnaryOperation(new LjsAstGetVar("b"), LjsAstUnaryOperationType.Minus), 
                LjsAstAssignMode.Normal));
    }

    [Test]
    public void BuildSimpleExpression()
    {
        var rootNode = TestUtils.BuildAstNode("a + b");

        rootNode.Should().BeEquivalentTo(
            new LjsAstBinaryOperation(
                new LjsAstGetVar("a"), 
                new LjsAstGetVar("b"), 
                LjsAstBinaryOperationType.Plus));
    }
    
    [Test]
    public void SimpleVarAssignTest()
    {
        var rootNode = TestUtils.BuildAstNode("a = b + c");

        rootNode.Should().BeEquivalentTo(
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
        var rootNode = TestUtils.BuildAstNode("a.foo = b + c");

        rootNode.Should().BeEquivalentTo(
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
        var rootNode = TestUtils.BuildAstNode("a[0] = b + c");

        rootNode.Should().BeEquivalentTo(
            new LjsAstSetProperty(new LjsAstLiteral<int>(0), new LjsAstGetVar("a"),
                new LjsAstBinaryOperation(
                    new LjsAstGetVar("b"),
                    new LjsAstGetVar("c"),
                    LjsAstBinaryOperationType.Plus),
                LjsAstAssignMode.Normal));
    }
    
    [Test]
    public void SimpleFunctionCallTest()
    {
        var rootNode = TestUtils.BuildAstNode("foo.bar(a,b)");

        rootNode.Should().BeEquivalentTo(
            new LjsAstFunctionCall(
                new LjsAstGetNamedProperty("bar", new LjsAstGetVar("foo")),
                new LjsAstGetVar("a"), new LjsAstGetVar("b")
                ));
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
            var rootNode = TestUtils.BuildAstNode(expression);
        
            rootNode.Should().BeOfType<LjsAstBinaryOperation>();

            rootNode.Should().BeEquivalentTo(
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
        var astBuilder = new LjsAstBuilder(literal);
        var rootNode = astBuilder.Build().RootNode;

        rootNode.Should().BeOfType<LjsAstLiteral<TLiteralType>>();
        rootNode.Should().BeEquivalentTo(new LjsAstLiteral<TLiteralType>(expectedValue));
    }
    
    [Test]
    public void SimpleTest()
    {
        var node = TestUtils.BuildAstNode("a + b + 5");

        node.Should().BeEquivalentTo("a".ToVar().Plus("b".ToVar()).Plus(5.ToLit()));
    }

    [Test]
    public void IncrementSimpleTest()
    {
        var a = TestUtils.BuildAstNode("++a");
        a.Should().BeEquivalentTo("a".ToVar().WithPrefixIncrement());
        
        a = TestUtils.BuildAstNode("a++");
        a.Should().BeEquivalentTo("a".ToVar().WithPostfixIncrement());
    }

    [Test]
    public void UnaryMinusSimpleTest()
    {
        var node = TestUtils.BuildAstNode("-a + b");

        node.Should().BeEquivalentTo("a".WithUnaryMinus().Plus("b"));
    }
    
    [Test]
    public void UnaryMinusAssignmentTest()
    {
        var node = TestUtils.BuildAstNode("x = -a + -b");

        node.Should().BeEquivalentTo(
            "x".Assign("a".WithUnaryMinus().Plus("b".WithUnaryMinus())));
    }
    
    [Test]
    public void UnaryMinusAssignmentWithParentheses()
    {
        var node = TestUtils.BuildAstNode("x = (-(a + b))");

        node.Should().BeEquivalentTo("x".Assign("a".Plus("b").WithUnaryMinus()));
    }
    
    [Test]
    public void UnaryMinusSequence()
    {
        var node = TestUtils.BuildAstNode("x = a - - - - - b");

        node.Should()
            .BeEquivalentTo(
                "x".Assign("a".Minus("b".WithUnaryMinus().WithUnaryMinus().WithUnaryMinus().WithUnaryMinus())));
    }
    
    [Test]
    public void UnaryPlusMinusSequence()
    {
        var node = TestUtils.BuildAstNode("x = a - + - + - b");

        node.Should()
            .BeEquivalentTo(
                "x".Assign("a".Minus("b".WithUnaryMinus().WithUnaryPlus().WithUnaryMinus().WithUnaryPlus())));
    }
    
    [Test]
    public void UnaryMinusAssignmentComplexTest()
    {
        var node = TestUtils.BuildAstNode("x = -(a + -b)");

        node.Should()
            .BeEquivalentTo(
                "x".Assign("a".Plus("b".WithUnaryMinus()).WithUnaryMinus()));
    }
    
    [Test]
    public void UnaryPlusAssignmentTest()
    {
        var node = TestUtils.BuildAstNode("x = +a + +b");

        node.Should().BeEquivalentTo("x".Assign("a".WithUnaryPlus().Plus("b".WithUnaryPlus())));
    }
    
    [Test]
    public void ParenthesesTest()
    {
        var node = TestUtils.BuildAstNode("a + ( b - c ) + d");

        node.Should().BeEquivalentTo(
            "a".Plus("b".Minus("c")).Plus("d"));
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
            var node = TestUtils.BuildAstNode(expression);
            node.Should().BeEquivalentTo(("a".Plus("b")).Minus("c".Plus("d")));
        }
    }
    
    [Test]
    public void AssignSimpleTest()
    {
        var node = TestUtils.BuildAstNode("a = b + c");

        node.Should().BeEquivalentTo("a".Assign("b".Plus("c")));
    }
    
    [Test]
    public void AssignSequenceTest()
    {
        var node = TestUtils.BuildAstNode("x = y = a = b + c");

        node.Should().BeEquivalentTo(
            "x".Assign("y".Assign("a".Assign(
                "b".Plus("c")
            ))));
    }
    
    [Test]
    public void AssignWithParenthesesTest()
    {
        var node = TestUtils.BuildAstNode("x = a + ( b - c ) + d");

        node.Should().BeEquivalentTo(
            "x".Assign(
                "a".Plus("b".Minus("c")).Plus("d")));
    }

    [Test]
    public void DotAccessSimpleTest()
    {
        var node = TestUtils.BuildAstNode("x = a.foo.bar");
        node.Should().BeEquivalentTo(
            "x".Assign("a".GetProp("foo").GetProp("bar")));
    }
    
    [Test]
    public void DotPropertyAssignSimpleTest()
    {
        var node = TestUtils.BuildAstNode("a.foo.bar = x");
        var expected = "a".GetProp("foo").SetProp("bar", "x".ToVar());
        node.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
    }
    
    [Test]
    public void BracketsPropertySimpleTest()
    {
        var node = TestUtils.BuildAstNode("x = a['foo']");
        var expected = "x".Assign("a".GetProp("foo".ToLit()));
        node.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
    }
    
    [Test]
    public void BracketsPropertyNestedSimpleTest()
    {
        var node = TestUtils.BuildAstNode("x = a[foo[0]]");
        var expected = "x".Assign("a".GetProp("foo".GetProp(0.ToLit())));
        node.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
    }
    
    [Test]
    public void BracketsPropertyAssignSimpleTest()
    {
        var node = TestUtils.BuildAstNode("a['foo'] = x");
        var expected = "a".SetProp("foo".ToLit(), "x".ToVar());
        node.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
    }
    
    [Test]
    public void BuildSimpleTernaryIfExpression()
    {
        var node = TestUtils.BuildAstNode("a ? b : c");
        var expected = "a".ToVar().TernaryIf("b".ToVar(), "c".ToVar());
        node.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
    }
    
    [Test]
    public void BuildNestedInParenthesesTernaryIfExpression()
    {
        var node = TestUtils.BuildAstNode("a ? (x?y:z) : c");
        var expected = "a".ToVar().TernaryIf(
            "x".ToVar().TernaryIf("y".ToVar(), "z".ToVar()), 
            "c".ToVar());
        node.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
    }
    
    [Test]
    public void BuildNestedTernaryIfExpression()
    {
        var node = TestUtils.BuildAstNode("a ? b ? b1 : b2 : c");
        
        var expected = "a".ToVar().TernaryIf(
            "b".ToVar().TernaryIf("b1".ToVar(), "b2".ToVar()), 
            "c".ToVar());
        
        node.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
    }
    
    [Test]
    public void BuildTernaryIfExpressionWithAssignment()
    {
        var node = TestUtils.BuildAstNode("x = a ? b : c");
        var expected = "x".Assign("a".ToVar().TernaryIf("b".ToVar(), "c".ToVar()));
        
        node.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
    }

    
}