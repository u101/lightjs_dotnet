using LightJS.Runtime;

namespace LightJS.Test.Runtime;

[TestFixture]
public class RuntimeFunctionsInvocationTests
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
        """;
        
        var runtime = CreateRuntime(code);
        runtime.Execute();

        var factArray = new[] { 0, 1, 2, 6, 24, 120, 720 };

        for (var i = 1; i <= 6; i++)
        {
            var result = runtime.Invoke("fact", i); // 1, 2, 6, 24, 120, 720
            CheckResult(result, factArray[i]);
        }
    }

    
    
    [Test]
    public void InvokeFunctionWithOneArgumentTest()
    {
        var code = """
        function foo(a) {
            return a;
        }
        """;
        
        var runtime = CreateRuntime(code);
        runtime.Execute();
        
        var invocationResult = runtime.Invoke("foo", "hi");
        
        CheckResult(invocationResult, "hi");

    }
    
    [Test]
    public void InvokeFunctionWithTwoArgumentsTest()
    {
        var code = """
        function foo(a, b) {
            return a + b;
        }
        """;
        
        var runtime = CreateRuntime(code);
        runtime.Execute();
        
        var invocationResult = runtime.Invoke("foo", "hello_", "world");
        
        CheckResult(invocationResult, "hello_world");

    }
    
    [Test]
    public void InvokeFunctionWithThreeArgumentsTest()
    {
        var code = """
        function foo(a, b, c) {
            return a + b + c;
        }
        """;
        
        var runtime = CreateRuntime(code);
        runtime.Execute();
        
        var invocationResult = runtime.Invoke(
            "foo", "hello_", "world", "!!!");
        
        CheckResult(invocationResult, "hello_world!!!");

    }
    
    
}