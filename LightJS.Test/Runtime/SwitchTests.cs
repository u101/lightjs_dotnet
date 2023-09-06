namespace LightJS.Test.Runtime;

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
    
}