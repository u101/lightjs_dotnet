namespace LightJS.Test.Runtime;

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
    
}