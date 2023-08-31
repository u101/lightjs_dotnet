using LightJS.Runtime;

namespace LightJS.Test.Runtime;

[TestFixture]
public class RuntimeLocalsAccessTests
{

    [Test]
    public void HasLocalTest()
    {
        var code = """
        var a = -123
        var b = true
        var c = 'hi'
        """;
        
        var runtime = CreateRuntime(code);
        
        runtime.Execute();
        
        Assert.That(runtime.HasLocal("a"), Is.True);
        Assert.That(runtime.HasLocal("b"), Is.True);
        Assert.That(runtime.HasLocal("c"), Is.True);
        Assert.That(runtime.HasLocal("x"), Is.False);
    }
    
    [Test]
    public void GetLocalTest()
    {
        var code = """
        var x = -123
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, LjsObject.Undefined);

        var x = runtime.GetLocal("x");
        
        CheckResult(x, -123);

        var y = runtime.GetLocal("y");
        
        CheckResult(y, LjsObject.Undefined);
    }
    
    [Test]
    public void SetLocalTest()
    {
        var code = """
        var x = 456
        """;
        
        var runtime = CreateRuntime(code);
        runtime.Execute();

        var x = runtime.GetLocal("x");
        
        CheckResult(x, 456);

        var setResult = runtime.SetLocal("x", 123);
        
        Assert.That(setResult, Is.True);
        
        CheckResult(runtime.GetLocal("x"), 123);
    }

    [Test]
    public void InvokeFunctionTest()
    {
        var code = """
        var x = 123

        function foo() {
            x++;
            return x;
        }
        """;
        
        var runtime = CreateRuntime(code);
        runtime.Execute();
        
        CheckResult(runtime.GetLocal("x"), 123);

        for (var i = 0; i < 100; i++)
        {
            var invocationResult = runtime.Invoke("foo");
        
            CheckResult(invocationResult, 124 + i);
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