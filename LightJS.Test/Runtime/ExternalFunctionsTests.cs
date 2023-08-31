using LightJS.Runtime;

namespace LightJS.Test.Runtime;

[TestFixture]
public class ExternalFunctionsTests
{

    [Test]
    public void SimpleFunctionInvocationTest()
    {
        var c = 0;
        
        var code = """
        for(var i = 0; i < 100; i++) {
            foo(1,2,3);
        }
        """;
        
        var runtime = CreateRuntime(code);

        runtime.AddExternal("foo",LjsExternalFunction.Create(() =>
        {
            c++;
        }));

        runtime.Execute();
        
        Assert.That(c, Is.EqualTo(100));
    }

    [Test]
    public void ParseIntTest()
    {
        var runtime = CreateRuntime("parseInt('123')");

        var result = runtime.Execute();
        
        CheckResult(result, 123);
    }
    
    [Test]
    public void ParseFloatTest()
    {
        var runtime = CreateRuntime("parseFloat('3.1415')");

        var result = runtime.Execute();
        
        CheckResult(result, 3.1415);
    }
    
    [Test]
    public void ParseFloatWithInvalidArgumentTest()
    {
        
        var runtime = CreateRuntime("parseFloat('asdf')");

        var result = runtime.Execute();
        
        CheckResult(result, double.NaN);
    }
    
    [Test]
    public void ConvertToIntTest()
    {
        var runtime = CreateRuntime("int(3.1415)");

        var result = runtime.Execute();
        
        CheckResult(result, 3);
    }
    
    [Test]
    public void ConvertToNumberTest()
    {
        
        var runtime = CreateRuntime("Number(true)");

        var result = runtime.Execute();
        
        CheckResult(result, 1.0);
    }
    
    [Test]
    public void ConvertBoolToStringTest()
    {
        
        var runtime = CreateRuntime("String(true)");

        var result = runtime.Execute();
        
        CheckResult(result, "true");
    }
    
    [Test]
    public void ConvertDoubleToStringTest()
    {
        var runtime = CreateRuntime("String(3.1415)");

        var result = runtime.Execute();
        
        CheckResult(result, "3.1415");
    }
    
}