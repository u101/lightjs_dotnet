using LightJS.Runtime;

namespace LightJS.Test.Runtime;

[TestFixture]
public class ArithmeticTests
{

    [Test]
    public void SimpleAddIntegers()
    {
        var runtime = RuntimeTestUtils.CreateRuntime("4/2");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<int>(2)));
    }
    
}