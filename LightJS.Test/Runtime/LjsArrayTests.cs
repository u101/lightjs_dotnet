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
    public void ShiftTest()
    {
        var code = """
        var a = [111,222,333];
        a.shift()
        """;
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 111);
    }
    
    [Test]
    public void PopTest()
    {
        var code = """
        var a = [111,222,333];
        a.pop()
        """;
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 333);
    }
    
    [Test]
    public void UnshiftOneElementInEmptyArray()
    {
        var code = """
        var a = [];
        a.unshift(111);
        a
        """;
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, Arr(111));
    }
    
    [Test]
    public void UnshiftTwoElementsInEmptyArray()
    {
        var code = """
        var a = [];
        a.unshift(111, 222);
        a
        """;
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, Arr(111, 222));
    }
    
    [Test]
    public void UnshiftThreeElementsInEmptyArray()
    {
        var code = """
        var a = [];
        a.unshift(111, 222, 333);
        a
        """;
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, Arr(111, 222, 333));
    }
    
    
    [Test]
    public void UnshiftOneElement()
    {
        var code = """
        var a = [7,8,9];
        a.unshift(111);
        a
        """;
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, Arr(111,7,8,9));
    }
    
    [Test]
    public void UnshiftTwoElements()
    {
        var code = """
        var a = [7,8,9];
        a.unshift(111,222);
        a
        """;
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, Arr(111,222,7,8,9));
    }

    [Test]
    public void SliceTest_WithStartIndex()
    {
        var code = """
        var animals = ['ant', 'bison', 'camel', 'duck', 'elephant'];
        animals.slice(2)
        """;
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, Arr("camel", "duck", "elephant"));
    }
    
    [Test]
    public void SliceTest_WithStartIndexAndEndIndex()
    {
        var code = """
        var animals = ['ant', 'bison', 'camel', 'duck', 'elephant'];
        animals.slice(2, 4)
        """;
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, Arr("camel", "duck"));
    }
    
    [Test]
    public void SliceTest_WithoutParameters()
    {
        var code = """
        var animals = ['ant', 'bison', 'camel', 'duck', 'elephant'];
        animals.slice()
        """;
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, Arr("ant", "bison", "camel", "duck", "elephant"));
    }
    
}