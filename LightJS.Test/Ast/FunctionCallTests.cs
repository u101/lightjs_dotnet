namespace LightJS.Test.Ast;

[TestFixture]
public class FunctionCallTests
{

    [Test]
    public void SimpleFuncCallWithArgs()
    {
        var node = BuildAstNode("foo(x, y + 2, z + 1)");
        var expected = "foo".FuncCall("x".ToVar(), "y".Plus(2), "z".Plus(1));
        
        Match(node, expected);
    }
    
    [Test]
    public void FuncCallWithArrayArg()
    {
        var node = BuildAstNode("foo([1,2,3])");
        var expected = "foo".FuncCall(ArrayLit(1,2,3));
        Match(node, expected);
    }
    
    [Test]
    public void FuncCallWithObjectArg()
    {
        var node = BuildAstNode("foo({a:1,b:2})");
        var expected = "foo".FuncCall(ObjectLit().AddProp("a", 1).AddProp("b", 2));
        Match(node, expected);
    }
    
    [Test]
    public void NestedFuncCallsTest()
    {
        var node = BuildAstNode("x = foo(a, bar(x, buzz(a1,a2,a3)))");
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
        var node = BuildAstNode("x = a() + (b() + c())");
        var expected = "x".Assign(
            "a".FuncCall().Plus("b".FuncCall().Plus("c".FuncCall())));
        
        Match(node, expected);
    }
    
    [Test]
    public void SimpleFuncCallWithAssignment()
    {
        var node = BuildAstNode("x = foo(a, c-(a+b))");
        var expected = "x".Assign("foo".FuncCall(
            "a".ToVar(),
            "c".Minus("a".Plus("b"))
        ));
        Match(node, expected);
    }
    
    [Test]
    public void FuncCallWithPropertyAccess()
    {
        var node = BuildAstNode("x = foo.bar(a.name, b.age, c[0].toString())");
        var expected = "x".Assign("foo".GetProp("bar").FuncCall(
            "a".GetProp("name"),
            "b".GetProp("age"),
            "c".GetProp(0.ToLit()).GetProp("toString").FuncCall())
        );
        Match(node, expected);
    }
}