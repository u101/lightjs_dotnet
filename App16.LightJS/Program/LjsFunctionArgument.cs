using App16.LightJS.Runtime;

namespace App16.LightJS.Program;

public sealed class LjsFunctionArgument
{
    public string Name { get; }
    public LjsObject DefaultValue { get; }

    public LjsFunctionArgument(string name, LjsObject defaultValue)
    {
        Name = name;
        DefaultValue = defaultValue;
    }
}