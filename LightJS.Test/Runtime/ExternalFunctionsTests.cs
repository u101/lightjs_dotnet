using LightJS.Runtime;

namespace LightJS.Test.Runtime;

[TestFixture]
public class ExternalFunctionsTests
{

    [Test]
    public void SimpleFunctionInvocationTest()
    {
        var c = 0;
        
        var code = """
        for(var i = 0; i < 100; i++) {
            foo(1,2,3);
        }
        """;
        
        var runtime = CreateRuntime(code);

        runtime.AddExternal("foo",LjsExternalFunction.Create(() =>
        {
            c++;
        }));

        runtime.Execute();
        
        Assert.That(c, Is.EqualTo(100));
    }
    
}