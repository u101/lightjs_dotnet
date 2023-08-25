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
    
    [Test]
    public void PlusAssignmentTest()
    {
        const string code = """
        var a = 123
        a += 321
        """;
        
        var runtime = RuntimeTestUtils.CreateRuntime(code);
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<int>(123+321)));
    }
    
    [Test]
    public void MinusAssignmentTest()
    {
        const string code = """
        var a = 123
        a -= 321
        """;
        
        var runtime = RuntimeTestUtils.CreateRuntime(code);
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<int>(123-321)));
    }
    
    [Test]
    public void MulAssignmentTest()
    {
        const string code = """
        var a = 123
        a *= 111
        """;
        
        var runtime = RuntimeTestUtils.CreateRuntime(code);
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<int>(123*111)));
    }
    
    [Test]
    public void OrAssignmentTest()
    {
        const string code = """
        var a = true
        a ||= false
        """;
        
        var runtime = RuntimeTestUtils.CreateRuntime(code);
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<bool>(true)));
    }
    
    [Test]
    public void AndAssignmentTest()
    {
        const string code = """
        var a = true
        a &&= false
        """;
        
        var runtime = RuntimeTestUtils.CreateRuntime(code);
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<bool>(false)));
    }
    
    [Test]
    public void BitOrAssignmentTest()
    {
        const string code = """
        var a = 0b010101
        a |= 0b000111
        """;
        
        var runtime = RuntimeTestUtils.CreateRuntime(code);
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<int>(0b010101 | 0b000111)));
    }
    
    [Test]
    public void BitAndAssignmentTest()
    {
        const string code = """
        var a = 0b010101
        a &= 0b000111
        """;
        
        var runtime = RuntimeTestUtils.CreateRuntime(code);
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<int>(0b010101 & 0b000111)));
    }
    
}