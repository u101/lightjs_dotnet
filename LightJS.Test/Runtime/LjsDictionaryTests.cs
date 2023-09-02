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
        
        Match(result, Dict());
    }

    [Test]
    public void CreateDictionaryTest()
    {
        var runtime = CreateRuntime("{a:1,b:2,c:true,d:'hello'}");

        var result = runtime.Execute();
        
        Match(result, Dict()
            .With("a", 1)
            .With("b",2)
            .With("c", true)
            .With("d","hello"));
    }
    
}