using static App16.LightJS.Tests.Utils.RuntimeTestUtils;

namespace App16.LightJS.Tests.Runtime;

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

    [Test]
    public void SinTest()
    {
        var runtime = CreateRuntime("Math.sin(Math.PI/2)");

        var result = runtime.Execute();
        
        CheckResult(result, 1.0);
    }
    
    [Test]
    public void CosTest()
    {
        var runtime = CreateRuntime("Math.cos(Math.PI)");

        var result = runtime.Execute();
        
        CheckResult(result, -1.0);
    }

    [Test]
    public void FloorTest()
    {
        var runtime = CreateRuntime("Math.floor(Math.PI)");

        var result = runtime.Execute();
        
        CheckResult(result, 3);
    }
    
    [Test]
    public void CeilTest()
    {
        var runtime = CreateRuntime("Math.ceil(Math.PI)");

        var result = runtime.Execute();
        
        CheckResult(result, 4);
    }
}