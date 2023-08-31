namespace LightJS.Test.Runtime;

[TestFixture]
public class StringTests
{
    [Test]
    public void SimpleAdd()
    {
        var runtime = CreateRuntime("'hello_' + 'world'");
        var result = runtime.Execute();
        
        CheckResult(result, "hello_world");
    }
}