using BenchmarkDotNet.Attributes;
using LightJS.Ast;

namespace LightJS.Benchmarks;

[MemoryDiagnoser(true)]
public class AstBenchmarks
{

    [Benchmark]
    public LjsAstModel BuildAstModel()
    {
        var code = """
        var a = 1, b = 1;
        
        function foo(x,y) {
            return x + y;
        }
        
        for (;a < 100; a++) {
            b += a;
        }
        
        var z = foo(a,b)

        """;

        var astBuilder = new LjsAstBuilder(code);

        var astModel = astBuilder.Build();

        return astModel;
    }
    
}