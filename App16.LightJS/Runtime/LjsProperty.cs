namespace App16.LightJS.Runtime;

public abstract class LjsProperty : LjsObject
{
    public abstract LjsMemberType MemberType { get; } 
    public abstract LjsPropertyAccessType AccessType { get; }

    public abstract LjsObject Get(LjsObject instance);
    public abstract void Set(LjsObject instance, LjsObject v);
}