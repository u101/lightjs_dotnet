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

    [Test]
    public void ConcatTwoArrays()
    {
        var code = """
        var a1 = [1,2,3]
        var a2 = [4,5,6]
        a1.concat(a2)
        """;
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, Arr(1,2,3,4,5,6));
    }
    
    [Test]
    public void ConcatThreeArrays()
    {
        var code = """
        var a1 = [1,2,3]
        var a2 = [4,5,6]
        var a3 = [7,8,9]
        a1.concat(a2, a3)
        """;
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, Arr(1,2,3,4,5,6,7,8,9));
    }
    
    [Test]
    public void ConcatFourArrays()
    {
        var code = """
        var a1 = [1,2,3]
        var a2 = [4,5,6]
        var a3 = [7,8,9]
        var a4 = ["hi","there"]
        a1.concat(a2, a3, a4)
        """;
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, Arr(1,2,3,4,5,6,7,8,9, "hi", "there"));
    }
    
    
    [Test]
    public void PushOneValue()
    {
        var code = """
        var a = [1,2,3]
        a.push(8);
        a
        """;
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, Arr(1,2,3,8));
    }
    
    [Test]
    public void PushTwoValues()
    {
        var code = """
        var a = [1,2,3]
        a.push(8,9);
        a
        """;
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, Arr(1,2,3,8,9));
    }
    
    [Test]
    public void PushThreeValues()
    {
        var code = """
        var a = [1,2,3]
        a.push(8,9,123);
        a
        """;
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, Arr(1,2,3,8,9,123));
    }
    
    [Test]
    public void PushFourValues()
    {
        var code = """
        var a = [1,2,3]
        a.push(8,9,123,-555);
        a
        """;
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, Arr(1,2,3,8,9,123,-555));
    }
    
    [Test]
    public void Shift()
    {
        var code = """
        var a = [111,222,333];
        a.shift()
        """;
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 111);
    }
    
}