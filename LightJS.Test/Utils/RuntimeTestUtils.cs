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

    public static void CheckResult(LjsObject result, LjsObject expectedValue)
    {
        Assert.That(result, Is.EqualTo(expectedValue));
    }
    
    public static void CheckResult(LjsObject result, int expectedValue)
    {
        Assert.That(result, Is.EqualTo(new LjsValue<int>(expectedValue)));
    }
    
    public static void CheckResult(LjsObject result, double expectedValue)
    {
        Assert.That(result, Is.EqualTo(new LjsValue<double>(expectedValue)));
    }
    
    public static void CheckResult(LjsObject result, bool expectedValue)
    {
        Assert.That(result, Is.EqualTo(new LjsValue<bool>(expectedValue)));
    }
    
    public static void CheckResult(LjsObject result, string expectedValue)
    {
        Assert.That(result, Is.EqualTo(new LjsString(expectedValue)));
    }

}