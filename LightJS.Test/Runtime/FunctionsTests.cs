using static LightJS.Test.Runtime.RuntimeTestUtils;

namespace LightJS.Test.Runtime;

[TestFixture]
public class FunctionsTests
{
    [Test]
    public void FactorialTest()
    {
        var code = """
        fact(8)
        
        function fact(n) {
            if (n <= 0) return 0;
            if (n == 1) return 1;
            return n * fact(n - 1); 
        }
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 40320);
    }
    
    [Test]
    public void FactorialIterationsCountTest()
    {
        var code = """
        
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
        var code = """
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
}