using FluentAssertions;
using LightJS.Ast;
using LightJS.Tokenizer;

namespace LightJS.Test;

[TestFixture]
public class LjsAstBuilderTest
{

    [Test]
    public void SimpleVarAssignTest()
    {
        var astBuilder = new LjsAstBuilder("a = b + c");
        var model = astBuilder.Build();

        var rootNode = model.RootNode;

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
        var astBuilder = new LjsAstBuilder("a.foo = b + c");
        var model = astBuilder.Build();

        var rootNode = model.RootNode;

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
        var astBuilder = new LjsAstBuilder("a[0] = b + c");
        var model = astBuilder.Build();

        var rootNode = model.RootNode;

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
        var astBuilder = new LjsAstBuilder("foo.bar(a,b)");
        var model = astBuilder.Build();

        var rootNode = model.RootNode;

        rootNode.Should().BeEquivalentTo(
            new LjsAstFunctionCall(
                new LjsAstGetNamedProperty("bar", new LjsAstGetVar("foo")),
                new LjsAstGetVar("a"), new LjsAstGetVar("b")
                ));
    }
    
    
    
    [Test]
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
        
    }
    

    [Test]
    public void BuildSimpleMultilineExpression()
    {
        BuildSimpleMultilineExpression("a + b; \n ++x * y");
        BuildSimpleMultilineExpression("a + b;++x * y");
        BuildSimpleMultilineExpression("a + b\n++x * y");
        BuildSimpleMultilineExpression("a\n + b\n++x * y");
        BuildSimpleMultilineExpression("a + \nb\n++x * y");
    }
    
    private static void BuildSimpleMultilineExpression(string expression)
    {
        var astBuilder = new LjsAstBuilder(expression);
        var rootNode = astBuilder.Build().RootNode;

        rootNode.Should().BeEquivalentTo(new LjsAstSequence(
            new LjsAstBinaryOperation(new LjsAstGetVar("a"), new LjsAstGetVar("b"), LjsAstBinaryOperationType.Plus),
            new LjsAstBinaryOperation(new LjsAstUnaryOperation(
                new LjsAstGetVar("x"), LjsAstUnaryOperationType.PrefixIncrement), new LjsAstGetVar("y"), LjsAstBinaryOperationType.Multiply)
        ));
    }
    
    [Test]
    public void BuildAssignMultilineExpression()
    {
        BuildAssignMultilineExpression("a = b; \n y = ++x");
        BuildAssignMultilineExpression("a = b;y = ++ x");
        BuildAssignMultilineExpression("a = b\ny =++ x");
        BuildAssignMultilineExpression("a\n = b\ny =++ x");
        BuildAssignMultilineExpression("a = \nb\ny =++ x");
        BuildAssignMultilineExpression("a = \nb\ny =\n++ x");
    }
    
    private static void BuildAssignMultilineExpression(string expression)
    {
        var astBuilder = new LjsAstBuilder(expression);
        var rootNode = astBuilder.Build().RootNode;

        rootNode.Should().BeEquivalentTo(new LjsAstSequence(
            new LjsAstSetVar("a", new LjsAstGetVar("b"), LjsAstAssignMode.Normal),
            new LjsAstSetVar("y", new LjsAstUnaryOperation(
                new LjsAstGetVar("x"), LjsAstUnaryOperationType.PrefixIncrement), LjsAstAssignMode.Normal)
        ));
    }
    
    
    [Test]
    public void BuildPostfixIncrementExpression()
    {
        var astBuilder = new LjsAstBuilder("a++ + b--");
        var rootNode = astBuilder.Build().RootNode;
        
        rootNode.Should().BeOfType<LjsAstBinaryOperation>();

        rootNode.Should().BeEquivalentTo(
            new LjsAstBinaryOperation(
                new LjsAstUnaryOperation(new LjsAstGetVar("a"), LjsAstUnaryOperationType.PostfixIncrement), 
                new LjsAstUnaryOperation(new LjsAstGetVar("b"), LjsAstUnaryOperationType.PostfixIncrement), 
                LjsAstBinaryOperationType.Plus));
    }
    
    [Test]
    public void BuildPrefixIncrementExpression()
    {
        var astBuilder = new LjsAstBuilder("++a + --b");
        var rootNode = astBuilder.Build().RootNode;
        
        rootNode.Should().BeOfType<LjsAstBinaryOperation>();

        rootNode.Should().BeEquivalentTo(
            new LjsAstBinaryOperation(
                new LjsAstUnaryOperation(new LjsAstGetVar("a"), LjsAstUnaryOperationType.PrefixIncrement), 
                new LjsAstUnaryOperation(new LjsAstGetVar("b"), LjsAstUnaryOperationType.PrefixDecrement), 
                LjsAstBinaryOperationType.Plus));
    }
    
    [Test]
    public void BuildUnaryMinusExpression()
    {
        var astBuilder = new LjsAstBuilder("a + -b");
        var rootNode = astBuilder.Build().RootNode;

        rootNode.Should().BeEquivalentTo(
            new LjsAstBinaryOperation(
                new LjsAstGetVar("a"), 
                new LjsAstUnaryOperation(new LjsAstGetVar("b"), LjsAstUnaryOperationType.Minus), 
                LjsAstBinaryOperationType.Plus));
    }
    
    [Test]
    public void BuildSimpleUnaryMinusAssignment()
    {
        var astBuilder = new LjsAstBuilder("a = -b");
        var rootNode = astBuilder.Build().RootNode;

        rootNode.Should().BeEquivalentTo(
            new LjsAstSetVar(
                "a", 
                new LjsAstUnaryOperation(new LjsAstGetVar("b"), LjsAstUnaryOperationType.Minus), 
                LjsAstAssignMode.Normal));
    }

    [Test]
    public void BuildSimpleExpression()
    {
        var astBuilder = new LjsAstBuilder("a + b");
        var rootNode = astBuilder.Build().RootNode;
        
        rootNode.Should().BeOfType<LjsAstBinaryOperation>();

        rootNode.Should().BeEquivalentTo(
            new LjsAstBinaryOperation(
                new LjsAstGetVar("a"), 
                new LjsAstGetVar("b"), 
                LjsAstBinaryOperationType.Plus));
    }

    [Test]
    public void BuildSimpleTernaryIfExpression()
    {
        var astBuilder = new LjsAstBuilder("a ? b : c");
        var rootNode = astBuilder.Build().RootNode;
        
        rootNode.Should().BeOfType<LjsAstTernaryIfOperation>();

        rootNode.Should().BeEquivalentTo(
            new LjsAstTernaryIfOperation(
                new LjsAstGetVar("a"),
                new LjsAstGetVar("b"),
                new LjsAstGetVar("c")));
    }
    
    [Test]
    public void BuildTernaryIfExpressionWithAssignment()
    {
        var astBuilder = new LjsAstBuilder("x = a ? b : c");
        var rootNode = astBuilder.Build().RootNode;

        rootNode.Should().BeEquivalentTo(
            new LjsAstSetVar("x",
            new LjsAstTernaryIfOperation(
                new LjsAstGetVar("a"),
                new LjsAstGetVar("b"),
                new LjsAstGetVar("c")), LjsAstAssignMode.Normal));
    }
    
    
    [Test]
    public void BuildNestedTernaryIfExpression()
    {
        var astBuilder = new LjsAstBuilder("a ? b ? b1 : b2 : c");
        var rootNode = astBuilder.Build().RootNode;
        
        rootNode.Should().BeOfType<LjsAstTernaryIfOperation>();

        rootNode.Should().BeEquivalentTo(
            new LjsAstTernaryIfOperation(
                new LjsAstGetVar("a"),
                new LjsAstTernaryIfOperation(
                    new LjsAstGetVar("b"),
                    new LjsAstGetVar("b1"),
                    new LjsAstGetVar("b2")),
                new LjsAstGetVar("c")));
    }

    [Test]
    public void BuildSimpleExpressionWithParentheses()
    {
        BuildSimpleExpressionWithParentheses("(a+b)-(c+d)");
        BuildSimpleExpressionWithParentheses("((a+b)-(c+d))");
        BuildSimpleExpressionWithParentheses("(((a+b)-(c+d)))");
        BuildSimpleExpressionWithParentheses("(((a+b)-((c+d))))");
    }
    
    private static void BuildSimpleExpressionWithParentheses(string expression)
    {
        var astBuilder = new LjsAstBuilder(expression);
        var rootNode = astBuilder.Build().RootNode;
        
        rootNode.Should().BeOfType<LjsAstBinaryOperation>();

        rootNode.Should().BeEquivalentTo(
            new LjsAstBinaryOperation(
                new LjsAstBinaryOperation(new LjsAstGetVar("a"), new LjsAstGetVar("b"), LjsAstBinaryOperationType.Plus),
                new LjsAstBinaryOperation(new LjsAstGetVar("c"), new LjsAstGetVar("d"), LjsAstBinaryOperationType.Plus),
                LjsAstBinaryOperationType.Minus));
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
        var astBuilder = new LjsAstBuilder(expression);
        Assert.Throws<LjsSyntaxError>(() => astBuilder.Build());
    }
    
    [Test]
    public void BuildSimpleExpressionWithOperatorsPriority()
    {
        var astBuilder = new LjsAstBuilder("a + b/c + d");
        var rootNode = astBuilder.Build().RootNode;
        
        rootNode.Should().BeOfType<LjsAstBinaryOperation>();

        var expectedResult = new LjsAstBinaryOperation(
            new LjsAstBinaryOperation(
                new LjsAstGetVar("a"),
                new LjsAstBinaryOperation(new LjsAstGetVar("b"), new LjsAstGetVar("c"), LjsAstBinaryOperationType.Div),
                LjsAstBinaryOperationType.Plus),
            new LjsAstGetVar("d"),
            LjsAstBinaryOperationType.Plus);

        rootNode.Should().BeEquivalentTo(expectedResult);
    }
    
    [Test]
    public void BuildSimpleLogicalExpression()
    {
        var astBuilder = new LjsAstBuilder("a + b != c + d");
        var rootNode = astBuilder.Build().RootNode;
        
        rootNode.Should().BeOfType<LjsAstBinaryOperation>();
        
        var expectedResult = new LjsAstBinaryOperation(
            new LjsAstBinaryOperation(new LjsAstGetVar("a"), new LjsAstGetVar("b"), LjsAstBinaryOperationType.Plus),
            new LjsAstBinaryOperation(new LjsAstGetVar("c"), new LjsAstGetVar("d"), LjsAstBinaryOperationType.Plus),
            LjsAstBinaryOperationType.NotEqual);

        rootNode.Should().BeEquivalentTo(expectedResult);
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
    
}