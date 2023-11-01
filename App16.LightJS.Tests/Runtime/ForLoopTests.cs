using static App16.LightJS.Tests.Utils.RuntimeTestUtils;

namespace App16.LightJS.Tests.Runtime;

[TestFixture]
public class ForLoopTests
{
    [Test]
    public void EmptyLoopTest()
    {
        const string code = """
        var a = 0;
        for(let i = 0; i < 10; ++i) {}
        a
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 0);
    }
    
    [Test]
    public void SimpleForLoopTest()
    {
        const string code = """
        var a = 0;
        for(let i = 0; i < 10; ++i) {
            a = i;
        }
        a
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 9);
    }
    
    [Test]
    public void SimpleForLoopWithTwoIteratorsTest()
    {
        const string code = """
        var a = 0;
        for(let i = 0, j = 100; i < 10 && j >= 0; ++i, --j) {
            a = j - i;
        }
        a
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 91-9);
    }
}