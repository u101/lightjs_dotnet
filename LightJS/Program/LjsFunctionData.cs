namespace LightJS.Program;

public class LjsFunctionData 
{
    public LjsInstructionsList InstructionsList { get; } = new();
    public List<LjsFunctionArg> Args { get; } = new();

    public int LocalsCount { get; set; } = 0;
}