using LightJS.Runtime;

namespace LightJS.Test.Runtime;

[TestFixture]
public class LjsArrayTests
{
    [Test]
    public void EmptyArrayTest()
    {
        var runtime = CreateRuntime("[]");

        var result = runtime.Execute();
        Assert.That(result, Is.TypeOf<LjsArray>());

        if (result is LjsArray a)
        {
            Assert.That(a.Count, Is.EqualTo(0));
        }
    }
}