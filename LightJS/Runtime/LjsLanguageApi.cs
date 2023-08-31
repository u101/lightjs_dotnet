using System.Globalization;

namespace LightJS.Runtime;

public static class LjsLanguageApi
{
    private static readonly LjsExternalFunction FuncParseInt = LjsExternalFunction.Create(ParseInt);
    private static readonly LjsExternalFunction FuncParseFloat = LjsExternalFunction.Create(ParseFloat);

    public static Dictionary<string, LjsObject> CreateApiDictionary() => new()
    {
        {"parseInt", FuncParseInt},
        {"parseFloat", FuncParseFloat},
    };


    private static LjsObject ParseInt(LjsObject str)
    {
        switch (str)
        {
            case LjsValue<string> s:
                // great!

                var v = s.Value;

                if (int.TryParse(v, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var result))
                {
                    return new LjsValue<int>(result);
                }

                return new LjsValue<int>(0);
            
            case LjsValue<int> i:
                return new LjsValue<int>(i.Value);
            
            case LjsValue<double> d:
                return new LjsValue<int>((int) d.Value);
            
            default:
                return new LjsValue<int>(0);
        }
    } 
    
    private static LjsObject ParseFloat(LjsObject str)
    {
        switch (str)
        {
            case LjsValue<string> s:
                // great!

                var v = s.Value;

                if (double.TryParse(v, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var result))
                {
                    return new LjsValue<double>(result);
                }

                return new LjsValue<double>(double.NaN);
            
            case LjsValue<int> i:
                return new LjsValue<double>(i.Value);
            
            case LjsValue<double> d:
                return new LjsValue<double>(d.Value);
            
            default:
                return new LjsValue<double>(double.NaN);
        }
    } 
    
}