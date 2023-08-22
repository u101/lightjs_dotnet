using LightJS.Ast;
using static LightJS.Test.Ast.NodesUtils;

namespace LightJS.Test.Ast;

[TestFixture]

public class FunctionDeclarationTests
{

    [Test]
    public void NestedFunctionsTest()
    {
        var code = """
        function foo() {
            return function(a,b) {
                return a + b
            }
        }
        """;
        
        var node = TestUtils.BuildAstNode(code);

        var expected = NamedFunc("foo",Return(
            Func("a", "b", Return("a".Plus("b")))
            ));
        
        Match(node, expected);
    }
    
    [Test]
    public void ComplexNestedFunctionsReturnTest()
    {
        var code = """
        function foo() {
            if (x) {
                return function(a,b) {
                    return a + b
                }
            }
            
            return function(a,b) {
                return a - b
            }
            
        }
        """;
        
        var node = TestUtils.BuildAstNode(code);

        var expected = NamedFunc("foo", Sequence(
            
            IfBlock("x".ToVar(), Return(Func("a", "b", Return("a".Plus("b"))))),
            Return(Func("a", "b", Return("a".Minus("b"))))
            ));
        
        Match(node, expected);
    }
    
    [Test]
    public void NamedFunctionTest()
    {
        var code = """
        function foo() {
            return y
        }
        """;
        
        var node = TestUtils.BuildAstNode(code);

        var expected = NamedFunc("foo",Return("y".ToVar()));
        
        Match(node, expected);
    }
    

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