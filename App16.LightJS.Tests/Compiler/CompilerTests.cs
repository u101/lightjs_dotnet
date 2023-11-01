using App16.LightJS.Compiler;
using App16.LightJS.Program;
using FluentAssertions;

namespace App16.LightJS.Tests.Compiler;

[TestFixture]
public class CompilerTests
{

    private static InstructionsBuilder CreateLetAbcInstructionsBuilder()
    {
        /*
        let a = 1;
        let b = 2;
        let c = 0;
         */
        var instructionsBuilder = new InstructionsBuilder();
        instructionsBuilder.DefineLocal("a");
        instructionsBuilder.DefineLocal("b");
        instructionsBuilder.DefineLocal("c");
        
        instructionsBuilder.Push(LjsInstructionCode.ConstIntOne);
        instructionsBuilder.StoreLocal("a");
        instructionsBuilder.Push(LjsInstructionCode.Discard);
        
        instructionsBuilder.Push(LjsInstructionCode.ConstInt, 2);
        instructionsBuilder.StoreLocal("b");
        instructionsBuilder.Push(LjsInstructionCode.Discard);
        
        instructionsBuilder.Push(LjsInstructionCode.ConstIntZero);
        instructionsBuilder.StoreLocal("c");
        instructionsBuilder.Push(LjsInstructionCode.Discard);

        return instructionsBuilder;
    }
    
    [Test]
    public void ForLoopTest()
    {
        const string code = """
        let a = 1;
        let b = 2;
        let c = 0;

        for (c = 1; a > b; c++) {
            c = a;
        }
        """;
        
        var compiler = LjsCompilerFactory.CreateCompiler(code);

        var program = compiler.Compile();
        
        var ib = CreateLetAbcInstructionsBuilder();
        // init loop
        ib.Push(LjsInstructionCode.ConstIntOne);
        ib.StoreLocal("c");
        // condition
        ib.LoadLocal("a").Label = "condition";
        ib.LoadLocal("b");
        ib.Push(LjsInstructionCode.Gt);
        ib.JumpIfFalse("end");
        // loop body
        ib.LoadLocal("a");
        ib.StoreLocal("c");
        // iterator
        ib.PostIncrement("c");
        ib.Jump("condition");
        ib.Push(LjsInstructionCode.Halt).Label = "end";
        

        var expected = CreateProgram(ib.Build());
        
        program.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes().WithStrictOrdering());
    }
    
    [Test]
    public void WhileTest()
    {
        const string code = """
        let a = 1;
        let b = 2;
        let c = 0;

        while (a > b) {
            c = a;
        }
        """;
        
        var compiler = LjsCompilerFactory.CreateCompiler(code);

        var program = compiler.Compile();

        var ib = CreateLetAbcInstructionsBuilder();
        ib.LoadLocal("a").Label = "start";
        ib.LoadLocal("b");
        ib.Push(LjsInstructionCode.Gt);
        ib.JumpIfFalse("end");

        ib.LoadLocal("a");
        ib.StoreLocal("c");
        ib.Jump("start");

        ib.Push(LjsInstructionCode.Halt).Label = "end";
        

        var expected = CreateProgram(ib.Build());
        
        program.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes().WithStrictOrdering());
    }
    

    [Test]
    public void IfTest()
    {
        const string code = """
        let a = 1;
        let b = 2;
        let c = 0;

        if (a > b) {
            c = a;
        }
        """;
        
        var compiler = LjsCompilerFactory.CreateCompiler(code);

        var program = compiler.Compile();
        
        var ib = CreateLetAbcInstructionsBuilder();
        ib.LoadLocal("a");
        ib.LoadLocal("b");
        ib.Push(LjsInstructionCode.Gt);
        ib.JumpIfFalse("end");
        
        ib.LoadLocal("a");
        ib.StoreLocal("c");
        
        ib.Jump("end");
        ib.Push(LjsInstructionCode.Halt).Label = "end";

        var expected = CreateProgram(ib.Build());
        
        program.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes().WithStrictOrdering());
    }

    [Test]
    public void SimpleProgramTest()
    {
        const string code = """
        let a = 1;
        let b = 2;
        let c = 0;
        c = a + b;
        """;

        var compiler = LjsCompilerFactory.CreateCompiler(code);

        var program = compiler.Compile();
        
        var ib = CreateLetAbcInstructionsBuilder();
        ib.LoadLocal("a");
        ib.LoadLocal("b");
        ib.Push(LjsInstructionCode.Add);
        ib.StoreLocal("c");
        ib.Push(LjsInstructionCode.Halt);
        

        var expected = CreateProgram(ib.Build());
        
        program.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes().WithStrictOrdering());
    }

    private static LjsProgram CreateProgram(LjsInstruction[] instructions) => new(new LjsProgramConstants(),
        new []
        {
            new LjsFunctionData(0, instructions, Array.Empty<LjsFunctionArgument>(), GetLocalVarPointers())
        }, new Dictionary<string, int>());

    private static LjsLocalVarPointer[] GetLocalVarPointers() => new[]
    {
        new LjsLocalVarPointer(0, "a", LjsLocalVarKind.Let),
        new LjsLocalVarPointer(1, "b", LjsLocalVarKind.Let),
        new LjsLocalVarPointer(2, "c", LjsLocalVarKind.Let),
    };
}