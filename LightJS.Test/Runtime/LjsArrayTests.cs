using LightJS.Runtime;

namespace LightJS.Test.Runtime;

[TestFixture]
public class LjsArrayTests
{
    [Test]
    public void CreateEmptyArrayTest()
    {
        var runtime = CreateRuntime("[]");

        var result = runtime.Execute();
        
        CheckResult(result, Arr());
    }
    
    [Test]
    public void CreateNonEmptyArrayTest()
    {
        var runtime = CreateRuntime("['a','b','c', 1,2,3, true, false]");

        var result = runtime.Execute();
        
        CheckResult(result, Arr(
            "a", "b", "c", 1, 2, 3, true, false
        ));
    }
    
    [Test]
    public void GetElementTest()
    {
        var code = """
        var a = ['a','hi!','c'];
        a[1];
        """;
        
        var runtime = CreateRuntime(code);

        var result = runtime.Execute();
        
        CheckResult(result, "hi!");
    }
    
    [Test]
    public void SetElementTest()
    {
        var code = """
        var a = ['a','b','c'];
        a[1] = "hi!";
        """;
        
        var runtime = CreateRuntime(code);

        var result = runtime.Execute();
        
        CheckResult(result, "hi!");
    }
    
    [Test]
    public void LengthTest()
    {
        var code = """
        var a = ['a','b','c'];
        a.length
        """;
        
        var runtime = CreateRuntime(code);

        var result = runtime.Execute();
        
        CheckResult(result, 3);
    }

    [Test]
    public void IndexOf_WhenValueNotFound()
    {
        var runtime = CreateRuntime("[1,2,3,4,5].indexOf(123)");
        var result = runtime.Execute();
        
        CheckResult(result, -1);
    }
    
    [Test]
    public void IndexOf()
    {
        var runtime = CreateRuntime("[1,2,3,4,5].indexOf(3)");
        var result = runtime.Execute();
        
        CheckResult(result, 2);
    }
    
    [Test]
    public void IndexOf_WithStartIndex()
    {
        var runtime = CreateRuntime("[1,2,3,4,5,1,2,3].indexOf(3,5)");
        var result = runtime.Execute();
        
        CheckResult(result, 7);
    }
    
    [Test]
    public void IndexOf_WithStartIndex_WhenValueNotFound()
    {
        var runtime = CreateRuntime("[1,2,3,4,5,1,2,3].indexOf(123,5)");
        var result = runtime.Execute();
        
        CheckResult(result, -1);
    }
}