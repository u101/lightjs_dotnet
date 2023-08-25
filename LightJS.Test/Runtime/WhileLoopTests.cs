using LightJS.Runtime;

namespace LightJS.Test.Runtime;

[TestFixture]
public class WhileLoopTests
{
    [Test]
    public void SimpleWhileLoopTest()
    {
        const string code = """
        var a = 0;
        while(a < 4) {
            a += 2
        }
        a
        """;
        
        var runtime = RuntimeTestUtils.CreateRuntime(code);
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<int>(4)));
    }
    
    [Test]
    public void SimpleWhileLoopWithBreakTest()
    {
        const string code = """
        var a = 0;
        while(a < 100) {
            a += 10
            if (a == 70) break
        }
        a
        """;
        
        var runtime = RuntimeTestUtils.CreateRuntime(code);
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<int>(70)));
    }
}