using LightJS.Runtime;

namespace LightJS.Test.Runtime;

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
        
        var runtime = RuntimeTestUtils.CreateRuntime(code);
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<int>(123)));

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
        
        var runtime = RuntimeTestUtils.CreateRuntime(code);
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<int>(987)));

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
        
        var runtime = RuntimeTestUtils.CreateRuntime(code);
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<int>(111)));

    }
}