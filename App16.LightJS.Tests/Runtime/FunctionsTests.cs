using static App16.LightJS.Tests.Utils.RuntimeTestUtils;

namespace App16.LightJS.Tests.Runtime;

[TestFixture]
public class FunctionsTests
{
    [Test]
    public void FactorialTest()
    {
        const string code = """
        function fact(n) {
            if (n <= 0) return 0;
            if (n == 1) return 1;
            return n * fact(n - 1); 
        }
        fact(8);
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 40320);
    }
    
    [Test]
    public void ArrowFunctionTest()
    {
        const string code = """
        var f = (x,y) => x ** y;
        f(2,8)
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 256.0);
    }
    
    [Test]
    public void FactorialIterationsCountTest()
    {
        const string code = """
        
        var iterations = 0
        
        fact(8)
        
        function fact(n) {
            ++iterations;
            
            if (n <= 0) return 0;
            if (n == 1) return 1;
            
            return n * fact(n - 1); 
        }
        
        iterations
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 8);
    }
    
    [Test]
    public void FactorialTest2()
    {
        const string code = """
        var fact = function(n) {
            if (n <= 0) return 0;
            if (n == 1) return 1;
            return n * fact(n - 1); 
        }
        fact(8)
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 40320);
    }
    
    [Test]
    public void NestedFunctionsTest()
    {
        const string code = """
        function bar(y) { return 12345; }
        function foo(n) {
            return bar(n + 1);
            function bar(x) { return x * 2; }
        }
        foo(1)
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 4);
    }
}