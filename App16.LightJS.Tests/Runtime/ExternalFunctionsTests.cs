using App16.LightJS.Runtime;
using static App16.LightJS.Tests.Utils.RuntimeTestUtils;

namespace App16.LightJS.Tests.Runtime;

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

        runtime.AddExternal("foo",LjsFunctionsFactory.CreateStatic(() =>
        {
            c++;
        }));

        runtime.Execute();
        
        Assert.That(c, Is.EqualTo(100));
    }
    
}