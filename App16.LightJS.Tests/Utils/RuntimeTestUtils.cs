

using App16.LightJS.Compiler;
using App16.LightJS.Runtime;

namespace App16.LightJS.Tests.Utils;

public static class RuntimeTestUtils
{
    public static LjsArray Arr(params LjsObject[] values) => new(values);
    public static LjsDictionary Dict(params KeyValuePair<string,LjsObject>[] values) => new(values);
    public static LjsDictionary Dict() => new();

    public static LjsDictionary With(this LjsDictionary d, string propName, LjsObject propValue)
    {
        d.Set(propName, propValue);
        return d;
    }
    
    
    public static LjsRuntime CreateRuntime(string sourceCode)
    {
        var compiler = LjsCompilerFactory.CreateCompiler(sourceCode);
        var program = compiler.Compile();
        return new LjsRuntime(program);
    }

    public static void CheckResult(LjsObject result, int[] expectedValue)
    {
        CheckResult(result, new LjsArray(
            expectedValue.Select(i => new LjsInteger(i))));
    }
    
    public static void CheckResult(LjsObject result, string[] expectedValue)
    {
        CheckResult(result, new LjsArray(
            expectedValue.Select(i => new LjsString(i))));
    }

    public static void CheckResult(LjsObject result, LjsObject expectedValue)
    {
        switch (expectedValue)
        {
            case LjsArray expectedArray:

                Assert.That(result, Is.TypeOf<LjsArray>());

                if (result is LjsArray a)
                {
                    var ln = a.Count;
                    
                    Assert.That(ln, Is.EqualTo(expectedArray.Count));

                    for (var i = 0; i < ln; i++)
                    {
                        CheckResult(a[i], expectedArray[i]);
                    }
                }
                
                break;
            case LjsDictionary expectedDict:
                
                Assert.That(result, Is.TypeOf<LjsDictionary>());

                if (result is LjsDictionary d)
                {
                    var ln = d.Count;
                    
                    Assert.That(ln, Is.EqualTo(expectedDict.Count));

                    var expectedDictKeys = expectedDict.Keys;

                    foreach (var key in expectedDictKeys)
                    {
                        Assert.That(d.ContainsKey(key), Is.True);
                        CheckResult(d[key],expectedDict[key]);
                    }
                }
                
                break;
            default:
                Assert.That(result, Is.EqualTo(expectedValue));
                break;
        }
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