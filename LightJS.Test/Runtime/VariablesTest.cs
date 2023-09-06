using LightJS.Compiler;
using LightJS.Errors;
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
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        CheckResult(result, 123);
    }
    
    [Test]
    public void ConstDeclarationTest()
    {
        const string code = """
        const a = 123;
        a
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        CheckResult(result, 123);
    }
    
    [Test]
    public void LetDeclarationTest()
    {
        const string code = """
        let a = 123;
        a
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        CheckResult(result, 123);
    }
    
    [Test]
    public void ConstBadAssignmentTest()
    {
        const string code = """
        const a = 123;
        a = 456;
        """;

        Assert.Throws<LjsCompilerError>(() => CreateRuntime(code));
    }
    
    [Test]
    public void MultipleVarDeclarationsTest()
    {
        const string code = """
        var a = 123, b = 456, c=789
        a + b + c
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 123+456+789);
    }
    
    [Test]
    public void PlusAssignmentTest()
    {
        const string code = """
        var a = 123
        a += 321
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 123+321);
    }
    
    [Test]
    public void MinusAssignmentTest()
    {
        const string code = """
        var a = 123
        a -= 321
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 123-321);
    }
    
    [Test]
    public void MulAssignmentTest()
    {
        const string code = """
        var a = 123
        a *= 111
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 123*111);
    }
    
    [Test]
    public void OrAssignmentTest()
    {
        const string code = """
        var a = true
        a ||= false
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, LjsBoolean.True);
    }
    
    [Test]
    public void AndAssignmentTest()
    {
        const string code = """
        var a = true
        a &&= false
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, LjsBoolean.False);
    }
    
    [Test]
    public void BitOrAssignmentTest()
    {
        const string code = """
        var a = 0b010101
        a |= 0b000111
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        CheckResult(result, 0b010101 | 0b000111);
    }
    
    [Test]
    public void BitAndAssignmentTest()
    {
        const string code = """
        var a = 0b010101
        a &= 0b000111
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 0b010101 & 0b000111);
    }
    
}