using App16.ALang.Ast;
using static App16.ALang.Js.Tests.Ast.AstTestsUtils;

namespace App16.ALang.Js.Tests.Ast;

[TestFixture]
public class FunctionDeclarationTests
{
    [Test]
    public void SimpleArrowFunctionTest()
    {
        var node = BuildAstNode(
            "x = (y) => 1");
        var expected = "x".Assign(
            Func("y", Return(1.ToLit()))
        );
        Match(node, expected);
    }
    
    [Test]
    public void SimpleArrowFunctionWithSquareBracketsTest()
    {
        var node = BuildAstNode(
            "x = (y) => { console.log(y); }");
        var expected = "x".Assign(
            Func("y", "console".Dot("log").FuncCall("y".ToVar()))
        );
        Match(node, expected);
    }[Test]
    public void ArrowFunctionDeclarationInFunctionCallTest()
    {
        var node = BuildAstNode(
            "foo(x,y, () => 1)");
        var expected = "foo".FuncCall(
            "x".ToVar(),
            "y".ToVar(),
            Func(Return(1.ToLit()))
        );
        Match(node, expected);
    }
    
    [Test]
    public void FunctionAsParameterInFunctionCallTest()
    {
        var node = BuildAstNode(
            "x = foo(a, b, function(x) { return x+x })");
        var expected = "x".Assign("foo".FuncCall(
            "a".ToVar(),
            "b".ToVar(),
            Func("x", Return("x".Plus("x")))
        ));
        Match(node, expected);
    }
    
    

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
        
        var node = BuildAstNode(code);

        var expected = NamedFunc("foo",Return(
            Func("a", "b", Return("a".Plus("b")))
            ));
        
        Match(node, expected);
    }

    [Test]
    public void TernaryIfFunctionAssign()
    {
        
        var node = BuildAstNode(
            "x = y ? function(a) {return a+1} : function(b) {return b-1}");

        var expected = "x".Assign( "y".Tif(
            Func("a", Return("a".Plus(1))),
            Func("b", Return("b".Minus(1.ToLit())))
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
        
        var node = BuildAstNode(code);

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
        
        var node = BuildAstNode(code);

        var expected = NamedFunc("foo",Return("y".ToVar()));
        
        Match(node, expected);
    }
    

    [Test]
    public void EmptyFunctionTest()
    {
        var node = BuildAstNode("x = function() {}");

        var expected = "x".Assign(Func(AstEmptyNode.Instance));
        
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
        
        var node = BuildAstNode(code);

        var expected = "x".Assign(Func(Return("y".ToVar()))); 
            
        Match(node, expected);
    }
    
    [Test]
    public void NestedFunctionDeclarationTest()
    {
        var code = """
        function foo() {
            return nested(123);

            function nested(x) {
                return x + 1;
            }
        }
        """;
        
        var node = BuildAstNode(code);

        var expected = NamedFunc("foo", Sequence(
            Return("nested".FuncCall(123.ToLit())),
            NamedFunc("nested", "x", Return("x".Plus(1)))
        ));
            
        Match(node, expected);
    }
}