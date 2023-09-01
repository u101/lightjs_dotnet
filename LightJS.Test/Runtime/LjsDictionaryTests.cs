using LightJS.Runtime;

namespace LightJS.Test.Runtime;

[TestFixture]
public class LjsDictionaryTests
{

    [Test]
    public void EmptyDictionaryTest()
    {
        var runtime = CreateRuntime("{}");

        var result = runtime.Execute();
        Assert.That(result, Is.TypeOf<LjsDictionary>());

        if (result is LjsDictionary dict)
        {
            Assert.That(dict.Count, Is.EqualTo(0));
        }
    }

    [Test]
    public void CreateDictionaryTest()
    {
        var runtime = CreateRuntime("{a:1,b:2,c:true,d:'hello'}");

        var result = runtime.Execute();
        
        Assert.That(result, Is.TypeOf<LjsDictionary>());

        if (result is not LjsDictionary dict) return;
        
        CheckResult(dict.Get("a"), 1);
        CheckResult(dict.Get("b"), 2);
        CheckResult(dict.Get("c"), true);
        CheckResult(dict.Get("d"), "hello");
    }
    
}