namespace LightJS.Test.Runtime;

[TestFixture]
public class MathFunctionsTests
{

    [Test]
    public void MathAbsTest()
    {
        var runtime = CreateRuntime("Math.abs(-500)");

        var result = runtime.Execute();
        
        CheckResult(result, 500);
    }
    
    [Test]
    public void MathSqrtTest()
    {
        var runtime = CreateRuntime("Math.sqrt(121*121)");

        var result = runtime.Execute();
        
        CheckResult(result, 121.0);
    }
}