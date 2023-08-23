using static LightJS.Test.Ast.NodesUtils;

namespace LightJS.Test.Ast;

[TestFixture]
public class FunctionCallTests
{

    [Test]
    public void SimpleFuncCallWithArgs()
    {
        var node = TestUtils.BuildAstNode("foo(x, y + 2, z + 1)");
        var expected = "foo".FuncCall("x".ToVar(), "y".Plus(2), "z".Plus(1));
        
        Match(node, expected);
    }
    
    [Test]
    public void FuncCallWithArrayArg()
    {
        var node = TestUtils.BuildAstNode("foo([1,2,3])");
        var expected = "foo".FuncCall(ArrayLit(1,2,3));
        Match(node, expected);
    }
    
    [Test]
    public void NestedFuncCallsTest()
    {
        var node = TestUtils.BuildAstNode("x = foo(a, bar(x, buzz(a1,a2,a3)))");
        var expected = "x".Assign(
            "foo".FuncCall(
                "a".ToVar(), "bar".FuncCall(
                    "x".ToVar(), "buzz".FuncCall(
                        "a1".ToVar(), "a2".ToVar(), "a3".ToVar()
            ))));
        
        Match(node, expected);
    }
    
    [Test]
    public void SimpleFuncCallWithoutArgs()
    {
        var node = TestUtils.BuildAstNode("x = a() + (b() + c())");
        var expected = "x".Assign(
            "a".FuncCall().Plus("b".FuncCall().Plus("c".FuncCall())));
        
        Match(node, expected);
    }
    
    [Test]
    public void SimpleFuncCallWithAssignment()
    {
        var node = TestUtils.BuildAstNode("x = foo(a, c-(a+b))");
        var expected = "x".Assign("foo".FuncCall(
            "a".ToVar(),
            "c".Minus("a".Plus("b"))
        ));
        Match(node, expected);
    }
}