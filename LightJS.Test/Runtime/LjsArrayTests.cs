namespace LightJS.Test.Runtime;

[TestFixture]
public class LjsArrayTests
{
    [Test]
    public void CreateEmptyArrayTest()
    {
        var runtime = CreateRuntime("[]");

        var result = runtime.Execute();
        
        Match(result, Arr());
    }
    
    [Test]
    public void CreateNonEmptyArrayTest()
    {
        var runtime = CreateRuntime("['a','b','c', 1,2,3, true, false]");

        var result = runtime.Execute();
        
        Match(result, Arr(
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
        
        Match(result, "hi!");
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
        
        Match(result, "hi!");
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
        
        Match(result, 3);
    }
}