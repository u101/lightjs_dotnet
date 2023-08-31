namespace LightJS.Runtime;

public static class LjsTypesConverter
{
    public static bool ToBool(LjsObject obj) => obj switch
    {
        LjsValue<int> i => i.Value == 0,
        LjsValue<double> i => i.Value == 0,
        LjsValue<bool> i => i.Value,
        LjsValue<string> i => !string.IsNullOrEmpty(i.Value),
        _ => obj != LjsObject.Null && obj != LjsObject.Undefined
    };

    public static int ToInt(LjsObject obj) => obj switch
    {
        LjsValue<int> i => i.Value,
        LjsValue<double> i => (int)i.Value,
        LjsValue<bool> i => i.Value ? 1 : 0,
        LjsValue<string> i => i.Value.Length,
        _ => obj == LjsObject.Null || obj == LjsObject.Undefined ? 0 : 1
    };
    
    public static double ToDouble(LjsObject obj) => obj switch
    {
        LjsValue<int> i => i.Value,
        LjsValue<double> i => i.Value,
        LjsValue<bool> i => i.Value ? 1 : 0,
        LjsValue<string> i => i.Value.Length,
        _ => obj == LjsObject.Null || obj == LjsObject.Undefined ? 0 : 1
    };
    
    public static bool IsNumber(LjsObject o) => o is LjsValue<int> or LjsValue<double>;
    
}