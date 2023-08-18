using static LightJS.Test.Ast.NodesUtils;

namespace LightJS.Test.Ast;

[TestFixture]

public class FunctionDeclarationTests
{
    [Test]
    public void SimpleFunctionDeclarationTest()
    {
        var code = """
        x = function() {
            return y
        }
        """;
        
        var node = TestUtils.BuildAstNode(code);

        var expected = Func(Return("y".ToVar())); 
            
        Match(node, expected);
    }
}