namespace LightJS.Runtime;

public static class LjsTypesConverter
{
    public static bool ToBool(LjsObject obj) => obj switch
    {
        LjsValue<int> i => i.Value == 0,
        LjsValue<double> i => i.Value == 0,
        LjsValue<bool> i => i.Value,
        LjsString i => !string.IsNullOrEmpty(i.Value),
        _ => obj != LjsObject.Null && obj != LjsObject.Undefined
    };

    public static int ToInt(LjsObject obj) => obj switch
    {
        LjsValue<int> i => i.Value,
        LjsValue<double> i => (int)i.Value,
        LjsValue<bool> i => i.Value ? 1 : 0,
        _ => 0
    };
    
    public static double ToDouble(LjsObject obj) => obj switch
    {
        LjsValue<int> i => i.Value,
        LjsValue<double> i => i.Value,
        LjsValue<bool> i => i.Value ? 1 : 0,
        _ => double.NaN
    };
    
    public static bool IsNumber(LjsObject o) => o is LjsValue<int> or LjsValue<double>;
    
}