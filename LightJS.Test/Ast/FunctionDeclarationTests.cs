using LightJS.Ast;
using static LightJS.Test.Ast.NodesUtils;

namespace LightJS.Test.Ast;

[TestFixture]

public class FunctionDeclarationTests
{

    [Test]
    public void EmptyFunctionTest()
    {
        var node = TestUtils.BuildAstNode("x = function() {}");

        var expected = "x".Assign(Func(LjsAstEmptyNode.Instance));
        
        Match(node, expected);
    }
    
    [Test]
    public void SimpleFunctionDeclarationTest()
    {
        var code = """
        x = function() {
            return y
        }
        """;
        
        var node = TestUtils.BuildAstNode(code);

        var expected = "x".Assign(Func(Return("y".ToVar()))); 
            
        Match(node, expected);
    }
}