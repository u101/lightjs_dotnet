using App16.LightJS.Program;

namespace App16.LightJS.Compiler;

internal sealed class LjsInstructionsList
{
    private readonly List<LjsInstruction> _instructions = new();
    
    public IReadOnlyList<LjsInstruction> Instructions => _instructions;
    
    public int Count => _instructions.Count;

    public void Add(LjsInstruction instruction)
    {
        _instructions.Add(instruction);
    }

    public void SetAt(LjsInstruction instruction, int index)
    {
        _instructions[index] = instruction;
    }

    public LjsInstruction LastInstruction => 
        _instructions.Count > 0 ? _instructions[^1] : default;
}