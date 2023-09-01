using LightJS.Errors;

namespace LightJS.Runtime;

public sealed class LjsString : LjsObject
{
    public static readonly LjsString Empty = new("");

    public static readonly LjsTypeInfo TypeInfo = new(
        LjsObject.TypeInfo,
        new Dictionary<string, LjsObject>()
        {
            { "length", new PropLength() },
            { "charAt", new FuncCharAt() },
            { "indexOf", new FuncIndexOf() },
        });

    public override LjsTypeInfo GetTypeInfo() => TypeInfo;

    public string Value { get; }

    public LjsString(string value)
    {
        Value = value ?? 
                throw new ArgumentNullException(nameof(value), "null string value is not supported");
    }

    public override bool Equals(LjsObject? other)
    {
        return other is LjsString s && s.Value == Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }
    
    // METHODS AND PROPS

    private static LjsString CheckThisArgument(LjsObject obj)
    {
        if (obj is not LjsString s)
            throw new LjsRuntimeError("'this' value is not LjsString");
        return s;
    }
    
    private sealed class FuncCharAt : LjsFunction
    {
        public override LjsMemberType MemberType => LjsMemberType.InstanceMember;
        public override int ArgumentsCount => 2;
        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            var s = CheckThisArgument(arguments[0]);
            var i = LjsTypesConverter.ToInt(arguments[1]);
            var str = s.Value;
            
            return (i >= 0 && i < str.Length) ? new LjsString(str[i].ToString()) : Empty;
        }
    }
    
    private sealed class FuncIndexOf  : LjsFunction
    {
        public override LjsMemberType MemberType => LjsMemberType.InstanceMember;
        public override int ArgumentsCount => 3;
        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            var s = CheckThisArgument(arguments[0]);
            var searchValue = arguments[1].ToString();
            var startIndex = LjsTypesConverter.ToInt(arguments[2]);
            var str = s.Value;

            var index = str.IndexOf(searchValue, startIndex, StringComparison.Ordinal);

            return index;
        }
    }
    
    private sealed class PropLength : LjsProperty
    {
        public override LjsMemberType MemberType => LjsMemberType.InstanceMember;
        public override LjsPropertyAccessType AccessType => LjsPropertyAccessType.Read;
        public override LjsObject Get(LjsObject instance)
        {
            var s = CheckThisArgument(instance);
            return s.Value.Length;
        }

        public override void Set(LjsObject instance, LjsObject v) {}
    }
    
}