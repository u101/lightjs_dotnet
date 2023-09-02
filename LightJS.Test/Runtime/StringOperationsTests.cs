namespace LightJS.Test.Runtime;

[TestFixture]
public class StringOperationsTests
{
    [Test]
    public void SimpleAdd()
    {
        var runtime = CreateRuntime("'hello_' + 'world'");
        var result = runtime.Execute();
        
        CheckResult(result, "hello_world");
    }
    

    [Test]
    public void ChartAtTest()
    {
        var runtime = CreateRuntime("String('hello').charAt(1)");

        var result = runtime.Execute();
        
        CheckResult(result, "e");
    }

    [Test]
    public void LengthTest()
    {
        var runtime = CreateRuntime("String('hello').length");

        var result = runtime.Execute();
        
        CheckResult(result, "hello".Length);
    }

    [Test]
    public void IndexOfTest()
    {
        var runtime = CreateRuntime("'hello'.indexOf('ll')");

        var result = runtime.Execute();
        
        CheckResult(result, ("hello").IndexOf("ll", StringComparison.Ordinal));
    }
    
    [Test]
    public void SubstringTest()
    {
        var code = """
        const s = 'hello_world_again'
        var startIndex = s.indexOf('_') + 1
        var endIndex = s.indexOf('_', startIndex)
        s.substring(startIndex, endIndex) // startIndex pointing to _ symbol (inclusive)
        """;
        
        var runtime = CreateRuntime(code);

        var result = runtime.Execute();
        
        CheckResult(result, "world");
    }
    
    
}