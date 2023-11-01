namespace App16.LightJS.Runtime;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class LjsField : Attribute {}

[AttributeUsage(AttributeTargets.Method)]
public class LjsMethod : Attribute {}