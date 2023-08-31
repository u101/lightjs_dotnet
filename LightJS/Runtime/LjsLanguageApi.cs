using System.Globalization;

namespace LightJS.Runtime;

public static class LjsLanguageApi
{
    private static readonly LjsExternalFunction FuncParseInt = LjsExternalFunction.Create(ParseInt);
    private static readonly LjsExternalFunction FuncParseFloat = LjsExternalFunction.Create(ParseFloat);
    private static readonly LjsExternalFunction FuncInt = LjsExternalFunction.Create(ConvertToInt);
    private static readonly LjsExternalFunction FuncNumber = LjsExternalFunction.Create(ConvertToNumber);
    private static readonly LjsExternalFunction FuncString = LjsExternalFunction.Create(ConvertToString);

    public static Dictionary<string, LjsObject> CreateApiDictionary() => new()
    {
        {"parseInt", FuncParseInt},
        {"parseFloat", FuncParseFloat},
        {"int", FuncInt},
        {"Number", FuncNumber},
        {"String", FuncString},
    };


    private static LjsObject ConvertToInt(LjsObject v) => new LjsInteger(LjsTypesConverter.ToInt(v));
    private static LjsObject ConvertToNumber(LjsObject v) => new LjsDouble(LjsTypesConverter.ToDouble(v));
    private static LjsObject ConvertToString(LjsObject v) => new LjsString(v.ToString());

    private static LjsObject ParseInt(LjsObject str)
    {
        switch (str)
        {
            case LjsString s:

                var v = s.Value;

                if (int.TryParse(v, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var result))
                {
                    return new LjsInteger(result);
                }

                return new LjsInteger(0);
            
            case LjsInteger i:
                return new LjsInteger(i.Value);
            
            case LjsDouble d:
                return new LjsInteger((int) d.Value);
            
            default:
                return new LjsInteger(0);
        }
    } 
    
    private static LjsObject ParseFloat(LjsObject str)
    {
        switch (str)
        {
            case LjsString s:
                // great!

                var v = s.Value;

                if (double.TryParse(v, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var result))
                {
                    return new LjsDouble(result);
                }

                return new LjsDouble(double.NaN);
            
            case LjsInteger i:
                return new LjsDouble(i.Value);
            
            case LjsDouble d:
                return new LjsDouble(d.Value);
            
            default:
                return new LjsDouble(double.NaN);
        }
    } 
    
}