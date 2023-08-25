using LightJS.Compiler;
using LightJS.Runtime;

namespace LightJS.Test.Runtime;

public static class RuntimeTestUtils
{
    public static LjsRuntime CreateRuntime(string sourceCode)
    {
        var compiler = new LjsCompiler(sourceCode);
        var program = compiler.Compile();
        return new LjsRuntime(program);
    }
}