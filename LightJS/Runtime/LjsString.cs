using LightJS.Errors;

namespace LightJS.Runtime;

public sealed class LjsString : LjsObject, ILjsCollection
{
    public static readonly LjsString Empty = new("");

    private static readonly LjsTypeInfo _TypeInfo = new(
        LjsObject.TypeInfo,
        new Dictionary<string, LjsObject>()
        {
            { "length", new PropLength() },
            { "charAt", new FuncCharAt() },
            { "indexOf", new FuncIndexOf() },
            { "substring", new FuncSubstring() },
            { "split", new FuncSplit() },
        });

    public override LjsTypeInfo GetTypeInfo() => _TypeInfo;

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

    public LjsObject Get(LjsObject elementId)
    {
        var index = LjsTypesConverter.ToInt(elementId);

        return (index >= 0 && index < Value.Length) ? 
            Value[index].ToString() : LjsObject.Undefined;
    }

    public void Set(LjsObject elementId, LjsObject value)
    {
        // TODO throw exception ??
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
    
    private sealed class FuncSubstring : LjsFunction
    {
        public override LjsMemberType MemberType => LjsMemberType.InstanceMember;
        public override int ArgumentsCount => 3;
        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            var arg1 = arguments[1];
            var arg2 = arguments[2];
            
            var s = CheckThisArgument(arguments[0]);
            var str = s.Value;
            var strLength = str.Length;
            
            if (strLength == 0) return Empty;
            
            
            var i = LjsRuntimeUtils.Clamp(LjsTypesConverter.ToInt(arg1), 0, strLength - 1);
            var j = LjsRuntimeUtils.Clamp(arg2 is LjsNumber n ? (int) n.NumericValue : strLength, i, strLength);

            if (i == j) return Empty;

            var result = str.Substring(i, j - i);

            return new LjsString(result);
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
    
    private sealed class FuncSplit  : LjsFunction
    {
        public override LjsMemberType MemberType => LjsMemberType.InstanceMember;
        public override int ArgumentsCount => 3;
        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            var s = CheckThisArgument(arguments[0]);
            var separator = arguments[1].ToString();
            var limit = arguments[2] is LjsNumber ? 
                LjsTypesConverter.ToInt(arguments[2]) : int.MaxValue;

            if (limit <= 0) return new LjsArray();
            
            var str = s.Value;
            

            var strings = str.Split(separator);
            var ln = Math.Min(limit, strings.Length);

            var list = new List<LjsObject>(ln);

            for (var i = 0; i < ln; i++)
            {
                list.Add(strings[i]);
            }

            return new LjsArray(list);
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