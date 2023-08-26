using LightJS.Runtime;

namespace LightJS.Program;

public class LjsFunction : LjsObject
{
    public LjsInstructionsList InstructionsList { get; } = new();
    public List<LjsFunctionArg> Args { get; } = new();
}