using LightJS.Runtime;

namespace LightJS.Program;

public class LjsFunctionArg
{
    public string Name { get; }
    public LjsObject DefaultValue { get; }

    public LjsFunctionArg(string name, LjsObject defaultValue)
    {
        Name = name;
        DefaultValue = defaultValue;
    }
}