namespace LightJS.Program;

public sealed class LjsInstructionsList
{
    private readonly List<LjsInstruction> _instructions = new();
    
    public IReadOnlyList<LjsInstruction> Instructions => _instructions;
    
    public int InstructionsCount => _instructions.Count;

    public void AddInstruction(LjsInstruction instruction)
    {
        _instructions.Add(instruction);
    }

    public void SetInstructionAt(LjsInstruction instruction, int index)
    {
        _instructions[index] = instruction;
    }

    public LjsInstruction LastInstruction => 
        _instructions.Count > 0 ? _instructions[^1] : default;
}