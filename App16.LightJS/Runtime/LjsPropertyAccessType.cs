namespace App16.LightJS.Runtime;

[Flags]
public enum LjsPropertyAccessType
{
    None = 0,
    Read = 1,
    Write = 2,
    
    All = Read | Write
}