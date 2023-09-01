namespace LightJS.Test.Runtime;

[TestFixture]
public class StringMethodsTests
{

    [Test]
    public void ChartAtTest()
    {
        var runtime = CreateRuntime("String('hello').charAt(1)");

        var result = runtime.Execute();
        
        CheckResult(result, "e");
    }

    [Test]
    public void LengthTest()
    {
        var runtime = CreateRuntime("String('hello').length");

        var result = runtime.Execute();
        
        CheckResult(result, "hello".Length);
    }
    
    
}