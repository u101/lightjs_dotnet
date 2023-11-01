using static App16.LightJS.Tests.Utils.RuntimeTestUtils;

namespace App16.LightJS.Tests.Runtime;

[TestFixture]
public class IfBlockTests
{
    [Test]
    public void SimpleIfTest()
    {
        const string code = """
            var a = 0
            if (a + 500 > 100) {
                a = 123
            }
            a;
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 123);

    }
    
    [Test]
    public void SimpleElseTest()
    {
        const string code = """
            var a = 0
            if (a + 500 == 100) {
                a = 123
            } else {
                a = 987
            }
            a;
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 987);

    }
    
    [Test]
    public void SimpleElseIfTest()
    {
        const string code = """
            var a = 0
            if (a == 100) {
                a = 123
            } 
            else if (a == 200) {
                a = 321
            }
            else if (a == 300) {
                a = 231
            }
            else if (a == 0) {
                a = 111
            }
            else {
                a = 987
            }
            a;
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 111);

    }
}