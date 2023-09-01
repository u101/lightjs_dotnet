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
        
        Match(result, new LjsArray());
    }
}