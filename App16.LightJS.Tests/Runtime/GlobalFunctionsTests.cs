using static App16.LightJS.Tests.Utils.RuntimeTestUtils;

namespace App16.LightJS.Tests.Runtime;

[TestFixture]
public class GlobalFunctionsTests
{
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