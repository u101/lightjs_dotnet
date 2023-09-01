using FluentAssertions;
using LightJS.Compiler;
using LightJS.Runtime;

namespace LightJS.Test.Utils;

public static class RuntimeTestUtils
{
    public static LjsRuntime CreateRuntime(string sourceCode)
    {
        var compiler = new LjsCompiler(sourceCode);
        var program = compiler.Compile();
        return new LjsRuntime(program);
    }

    public static void Match(LjsObject result, LjsObject expectedValue)
    {
        result.Should().BeEquivalentTo(
            expectedValue, 
            options => options.RespectingRuntimeTypes().WithoutStrictOrdering().ComparingByMembers(expectedValue.GetType()));
    }

    public static void CheckResult(LjsObject result, LjsObject expectedValue)
    {
        Assert.That(result, Is.EqualTo(expectedValue));
    }
    
    public static void CheckResult(LjsObject result, int expectedValue)
    {
        Assert.That(result, Is.EqualTo(new LjsInteger(expectedValue)));
    }
    
    public static void CheckResult(LjsObject result, double expectedValue)
    {
        Assert.That(result, Is.EqualTo(new LjsDouble(expectedValue)));
    }
    
    public static void CheckResult(LjsObject result, bool expectedValue)
    {
        Assert.That(result, Is.EqualTo(expectedValue ? LjsBoolean.True : LjsBoolean.False));
    }
    
    public static void CheckResult(LjsObject result, string expectedValue)
    {
        Assert.That(result, Is.EqualTo(new LjsString(expectedValue)));
    }

}