namespace LightJS.Runtime;

public abstract class LjsFunction: LjsObject
{
    public abstract LjsMemberType MemberType { get; } 
    
    public abstract int ArgumentsCount { get; }

    public abstract LjsObject Invoke(List<LjsObject> arguments);
}