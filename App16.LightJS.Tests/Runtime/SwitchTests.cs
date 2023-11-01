using static App16.LightJS.Tests.Utils.RuntimeTestUtils;

namespace App16.LightJS.Tests.Runtime;

[TestFixture]
public class SwitchTests
{

    [Test]
    public void EmptySwitchTest()
    {
        const string code = """
        var a = 0;
        switch(a) {}
        a
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 0);
    }
    
    [Test]
    public void SimpleSwitchTest()
    {
        const string code = """
        let a = 123;
        let b = 'fail'
        switch(a) {
            case 1:
                b = 'not really';
                break;
            case 2:
                b = 'oh no';
                break;
            case 123:
                b = 'yes';
                break;
        }
        b
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, "yes");
    }
    
    [Test]
    public void SwitchTestWithDefault()
    {
        const string code = """
        let a = 1234;
        let b = 'fail'
        switch(a) {
            case 1:
                b = 'not really';
                break;
            case 2:
                b = 'oh no';
                break;
            case 123:
                b = 'yes';
                break;
            default:
                b = 'OK';
        }
        b
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, "OK");
    }
    
    [Test]
    public void SwitchTestWithMultipleCases()
    {
        const string code = """
        let a = 1234;
        let b = 'fail'
        switch(a) {
            case 1:
            case 1234:
                b = 'super';
                break;
            case 123:
                b = 'yes';
                break;
            default:
                b = 'OK';
        }
        b
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, "super");
    }
    
    [Test]
    public void SwitchTestInFunction()
    {
        const string code = """
        
        function foo(x) {
            switch(x) {
                case 1:
                case 1234:
                    return 'super';
                case 123:
                    return 'yes';
                default:
                    return 'OK';
            }
        }
        
        foo(1234)
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, "super");
    }
    
    
    [Test]
    public void SwitchTestInFunctionDefaultCase()
    {
        const string code = """
        
        function foo(x) {
            switch(x) {
                case 1:
                case 1234:
                    return 'super';
                case 123:
                    return 'yes';
                default:
                    return 'OK';
            }
        }
        
        foo(987456)
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, "OK");
    }
    
}