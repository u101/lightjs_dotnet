using LightJS.Runtime;
using static LightJS.Test.Runtime.RuntimeTestUtils;

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

        var setResult = runtime.SetLocal(
            "x", new LjsValue<int>(123));
        
        Assert.That(setResult, Is.True);
        
        CheckResult(runtime.GetLocal("x"), 123);
    }
    
}