using System.Globalization;

namespace LightJS.Runtime;

public static class LjsLanguageApi
{
    private static readonly LjsFunction FuncParseInt = new ParseInt();
    private static readonly LjsFunction FuncParseFloat = new ParseFloat();
    private static readonly LjsFunction FuncInt = new ConvertToInt();
    private static readonly LjsFunction FuncNumber = new ConvertToNumber();
    private static readonly LjsFunction FuncString = new ConvertToString();

    public static Dictionary<string, LjsObject> CreateApiDictionary() => new()
    {
        {"parseInt", FuncParseInt},
        {"parseFloat", FuncParseFloat},
        {"int", FuncInt},
        {"Number", FuncNumber},
        {"String", FuncString},
        {"Math", LjsMath.Instance},
    };

    private sealed class ConvertToInt : LjsFunction
    {
        public override LjsMemberType MemberType => LjsMemberType.StaticMember;
        public override int ArgumentsCount => 1;
        public override LjsObject Invoke(List<LjsObject> arguments) => 
            new LjsInteger(LjsTypesCoercionUtil.ToInt(arguments[0]));
    }
    
    private sealed class ConvertToNumber : LjsFunction
    {
        public override LjsMemberType MemberType => LjsMemberType.StaticMember;
        public override int ArgumentsCount => 1;
        public override LjsObject Invoke(List<LjsObject> arguments) => 
            new LjsDouble(LjsTypesCoercionUtil.ToDouble(arguments[0]));
    }
    
    private sealed class ConvertToString : LjsFunction
    {
        public override LjsMemberType MemberType => LjsMemberType.StaticMember;
        public override int ArgumentsCount => 1;
        public override LjsObject Invoke(List<LjsObject> arguments) => 
            arguments[0] as LjsString ?? new LjsString(arguments[0].ToString());
    }
    
    private sealed class ParseFloat : LjsFunction
    {
        public override LjsMemberType MemberType => LjsMemberType.StaticMember;
        public override int ArgumentsCount => 1;

        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            var str = arguments[0];
            
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
    
    private sealed class ParseInt : LjsFunction
    {
        public override LjsMemberType MemberType => LjsMemberType.StaticMember;
        public override int ArgumentsCount => 1;

        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            var str = arguments[0];
            
            switch (str)
            {
                case LjsString s:

                    var v = s.Value;

                    return int.TryParse(
                        v, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var result) ? 
                        new LjsInteger(result) : new LjsInteger(0);

                case LjsInteger i:
                    return new LjsInteger(i.Value);
            
                case LjsDouble d:
                    return new LjsInteger((int) d.Value);
            
                default:
                    return new LjsInteger(0);
            }
        }
    }
    
}