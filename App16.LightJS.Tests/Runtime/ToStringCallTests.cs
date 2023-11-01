using static App16.LightJS.Tests.Utils.RuntimeTestUtils;

namespace App16.LightJS.Tests.Runtime;

[TestFixture]
public class ToStringCallTests
{

    [Test]
    public void StringToStringTest()
    {
        var runtime = CreateRuntime("('hello_world').toString()");

        var result = runtime.Execute();
        
        CheckResult(result, "hello_world");
    }
    
    [Test]
    public void IntegerToStringTest()
    {
        var runtime = CreateRuntime("(123456).toString()");

        var result = runtime.Execute();
        
        CheckResult(result, "123456");
    }
    
    [Test]
    public void DoubleToStringTest()
    {
        var runtime = CreateRuntime("(3.1415).toString()");

        var result = runtime.Execute();
        
        CheckResult(result, "3.1415");
    }
    
    [Test]
    public void BoolToStringTest()
    {
        var runtime = CreateRuntime("(false).toString()");

        var result = runtime.Execute();
        
        CheckResult(result, "false");
    }

    [Test]
    public void CompositeToStringTest()
    {
        var code = """
        var a = 123, b = 456, c = 789;
        function foo(a,b,c) {
            return a.toString() + b.toString() + c.toString();
        }
        foo(a,b,c)
        """;
        var runtime = CreateRuntime(code);

        var result = runtime.Execute();
        
        CheckResult(result, "123456789");
    }
    
    
}