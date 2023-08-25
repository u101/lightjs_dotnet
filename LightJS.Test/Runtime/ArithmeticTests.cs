using LightJS.Runtime;

namespace LightJS.Test.Runtime;

[TestFixture]
public class ArithmeticTests
{

    [Test]
    public void SimpleDivIntegers()
    {
        var runtime = RuntimeTestUtils.CreateRuntime("4/2");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<int>(2)));
    }

    [Test]
    public void ExpressionTest()
    {
        const double expected = 3.14 * (1 / 2.2 + 1e-9) - 1;
        var runtime = RuntimeTestUtils.CreateRuntime("3.14 * (1 / 2.2 + 1e-9) - 1");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<double>(expected)));
    }
    
}