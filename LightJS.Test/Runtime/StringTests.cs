using LightJS.Runtime;

namespace LightJS.Test.Runtime;

[TestFixture]
public class StringTests
{
    [Test]
    public void SimpleAdd()
    {
        var runtime = RuntimeTestUtils.CreateRuntime("'hello_' + 'world'");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<string>("hello_world")));
    }
}