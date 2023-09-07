namespace LightJS.Test.Runtime;

[TestFixture]
public class RuntimeFunctionsInvocationTests
{
    
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