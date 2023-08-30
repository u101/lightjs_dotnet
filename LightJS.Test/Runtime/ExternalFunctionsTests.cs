namespace LightJS.Test.Runtime;

[TestFixture]
public class ExternalFunctionsTests
{

    [Test]
    public void SimpleFunctionInvocationTest()
    {
        var code = """
        var c = 0;
        for(var i = 0; i < 100; i++) {
            c++;
        }
        c;
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 100);
    }
    
}