namespace LightJS.Runtime;

public static class LjsTypesConverter
{
    public static bool ToBool(LjsObject obj) => obj switch
    {
        LjsInteger i => i.Value != 0,
        LjsDouble i => i.Value != 0,
        LjsBoolean i => i.Value,
        LjsString i => !string.IsNullOrEmpty(i.Value),
        LjsNull _ => false,
        LjsUndefined _ => false,
        _ => true
    };

    public static int ToInt(LjsObject obj) => obj switch
    {
        LjsInteger i => i.Value,
        LjsDouble i => (int)i.Value,
        LjsBoolean i => i.Value ? 1 : 0,
        _ => 0
    };
    
    public static double ToDouble(LjsObject obj) => obj switch
    {
        LjsInteger i => i.Value,
        LjsDouble i => i.Value,
        LjsBoolean i => i.Value ? 1 : 0,
        LjsNull _ => 0,
        LjsUndefined _ => 0,
        _ => double.NaN
    };
    
}