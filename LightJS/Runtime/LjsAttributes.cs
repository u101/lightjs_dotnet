namespace LightJS.Runtime;

[AttributeUsage(AttributeTargets.Field)]
public class LjsField : Attribute {}

[AttributeUsage(AttributeTargets.Method)]
public class LjsMethod : Attribute {}