using static LightJS.Test.Ast.NodesUtils;

namespace LightJS.Test.Ast;

[TestFixture]
public class ObjectLiteralsTests
{
    [Test]
    public void EmptyObjectLiteralTest()
    {
        var node = TestUtils.BuildAstNode("x = {}");

        var expected = "x".Assign(ObjectLit());
        Match(node, expected);
    }
    
    [Test]
    public void BinaryOperationTest()
    {
        var node = TestUtils.BuildAstNode("x = {x:1,y:2} + {x:3,y:4}");

        var expected = "x".Assign(
            ObjectLit().AddProp("x",1).AddProp("y",2).Plus(
                ObjectLit().AddProp("x",3).AddProp("y",4)
                )
            );
        Match(node, expected);
    }

    [Test]
    public void SimpleObjectLiteralTest()
    {
        var node = TestUtils.BuildAstNode("x = { a:1, b:2, c:3, 'd':'hi'}");

        var expected = "x".Assign(
            ObjectLit()
                .AddProp("a", 1)
                .AddProp("b", 2)
                .AddProp("c", 3)
                .AddProp("d", "hi")
            );
        
        Match(node, expected);
    }
    
    [Test]
    public void NestedObjectLiteralTest()
    {
        var code = """
        x = {
            'a':[ {x:1,y:2}, {x:3,y:4} ],
            b: {
                name:'xxx',
                val:true
            }
        }
        """;
        
        var node = TestUtils.BuildAstNode(code);

        var expected = "x".Assign(
            ObjectLit(
                    "a", ArrayLit(
                        ObjectLit("x", 1.ToLit()).AddProp("y", 2.ToLit()),
                        ObjectLit("x", 3.ToLit()).AddProp("y", 4.ToLit())
                        )
                    )
                .AddProp("b", 
                    ObjectLit("name", "xxx".ToLit()).AddProp("val", True)
                    ));
        
        Match(node, expected);
    }

    [Test]
    public void NestedObjectWithFunctionProp()
    {
        var code = """
        x = {
            age:123,
            getAge:function() {
                return this.age
            }
        }
        """;
        
        var node = TestUtils.BuildAstNode(code);

        var expected = "x".Assign(
            ObjectLit()
                .AddProp("age", 123)
                .AddProp("getAge", Func(Return(This.GetProp("age"))))
            );
        
        Match(node, expected);
    }

}