using static App16.LightJS.Tests.Utils.RuntimeTestUtils;

namespace App16.LightJS.Tests.Runtime;

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
    public void SquareBracketsCharAccess()
    {
        var runtime = CreateRuntime("('hello_world')[5]");
        var result = runtime.Execute();
        
        CheckResult(result, "_");
    }

    [Test]
    public void StringSplitTest()
    {
        var runtime = CreateRuntime("'he,ll,ow,or,ld'.split(',')");
        var result = runtime.Execute();
        
        CheckResult(result, Arr("he", "ll", "ow", "or", "ld"));
    }
    
    [Test]
    public void StringSplitWithLimitTest()
    {
        var runtime = CreateRuntime("'he,ll,ow,or,ld'.split(',',2)");
        var result = runtime.Execute();
        
        CheckResult(result, Arr("he", "ll"));
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