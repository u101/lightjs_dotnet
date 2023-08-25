using LightJS.Compiler;
using LightJS.Program;

namespace LightJS.Test.Compiler;

[TestFixture]
public class CompilerTests
{

    [Test]
    public void SimpleCompilerTest()
    {
        var ljsCompiler = new LjsCompiler("123");
        var p = ljsCompiler.Compile();
        var expected = CreateExpectedProgram();
        
        Assert.That(p.GetProgramString(), Is.EqualTo(expected.GetProgramString()));
        
        
        LjsProgram CreateExpectedProgram()
        {
            var prg = new LjsProgram();

            prg.AddInstruction(new LjsInstruction(LjsInstructionCodes.ConstInt, 0));
            prg.AddIntegerConstant(123);
            
            return prg;
        }
    }
    
}