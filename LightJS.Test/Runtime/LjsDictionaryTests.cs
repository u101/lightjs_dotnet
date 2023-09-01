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
        
        Match(result, new LjsDictionary());
    }

    [Test]
    public void CreateDictionaryTest()
    {
        var runtime = CreateRuntime("{a:1,b:2,c:true,d:'hello'}");

        var result = runtime.Execute();
        
        Match(result, new LjsDictionary(new Dictionary<string, LjsObject>()
        {
            {"a", new LjsInteger(1)},
            {"b", new LjsInteger(2)},
            {"c", LjsBoolean.True},
            {"d", new LjsString("hello")},
        }));
    }
    
}