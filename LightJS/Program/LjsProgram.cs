namespace LightJS.Program;

public sealed class LjsProgram
{

    private readonly Dictionary<string, LjsFunction> _functions = new(); 

    public LjsInstructionsList InstructionsList { get; } = new();

    public void AddFunction(string name, LjsFunction func)
    {
        _functions[name] = func;
    }

    public (string name, LjsFunction func)[] Functions => 
        _functions.Select(p => (p.Key, p.Value)).ToArray();

    public LjsProgramConstants Constants { get; } = new();

}