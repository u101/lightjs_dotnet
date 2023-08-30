

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
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 4);
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
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 70);
    }
    
    [Test]
    public void WhileLoopWithStringConcatTest()
    {
        const string code = """
        var a = '', i = 10;
        while(i >= 0) {
            a += i
            --i
        }
        a
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, "109876543210");
    }
}