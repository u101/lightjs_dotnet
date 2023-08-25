using LightJS.Runtime;

namespace LightJS.Test.Runtime;

[TestFixture]
public class VariablesTest
{
    [Test]
    public void SimpleVarDeclarationTest()
    {
        const string code = """
        var a = 123;
        a
        """;
        
        var runtime = RuntimeTestUtils.CreateRuntime(code);
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<int>(123)));
    }
    
    [Test]
    public void MultipleVarDeclarationsTest()
    {
        const string code = """
        var a = 123, b = 456, c=789
        a + b + c
        """;
        
        var runtime = RuntimeTestUtils.CreateRuntime(code);
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<int>(123+456+789)));
    }
    
}