using LightJS.Errors;

namespace LightJS.Runtime;

public sealed class LjsArray : LjsObject, ILjsArray
{
    private static readonly LjsTypeInfo _TypeInfo = new(
        LjsObject.TypeInfo,
        new Dictionary<string, LjsObject>
        {
            { "length", new PropLength() },
            { "indexOf", new FuncIndexOf() },
            { "concat", new FuncConcat() },
            { "push", new FuncPush() },
            { "shift", new FuncShift() },
            { "pop", new FuncPop() },
            { "unshift", new FuncUnshift() },
            { "slice", new FuncSlice() },
        });

    public override LjsTypeInfo GetTypeInfo() => _TypeInfo;

    public IEnumerable<LjsObject> Elements => _list;

    private readonly List<LjsObject> _list;

    public int Count => _list.Count;

    public LjsArray()
    {
        _list = new List<LjsObject>();
    }
    
    public LjsArray(int size)
    {
        _list = new List<LjsObject>(size);
        
        for (var i = 0; i < size; i++)
        {
            _list.Add(Undefined);
        }
    }

    public LjsArray(IEnumerable<LjsObject> values)
    {
        _list = new List<LjsObject>(values);
    }
    
    internal LjsArray(List<LjsObject> list)
    {
        _list = list ?? throw new ArgumentNullException(nameof(list));
    }

    public void Add(LjsObject o)
    {
        _list.Add(o);
    }

    public void Insert(int index, LjsObject value)
    {
        if (index < 0 || index > _list.Count)
            throw new IndexOutOfRangeException($"index {index} out of range [0..{_list.Count}]");
        _list.Insert(index, value);
    }

    public LjsObject RemoveFirst()
    {
        if (_list.Count == 0)
            throw new Exception("list is empty");
        
        var result = _list[0];
        
        _list.RemoveAt(0);

        return result;
    }
    
    public LjsObject RemoveLast()
    {
        if (_list.Count == 0)
            throw new Exception("list is empty");

        var i = _list.Count - 1;
        var result = _list[i];
        
        _list.RemoveAt(i);

        return result;
    }

    public void Fill(int newSize, LjsObject v)
    {
        while (_list.Count < newSize)
        {
            _list.Add(v);
        }
    }
    
    public void Fill(LjsObject v)
    {
        for (var i = 0; i < _list.Count; i++)
        {
            _list[i] = v;
        }
    }

    public int IndexOf(LjsObject v, int startIndex = 0) => _list.IndexOf(v, startIndex);

    public LjsObject this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    public LjsObject Get(int index)
    {
        return (index >= 0 && index < _list.Count) ? 
            _list[index] : LjsObject.Undefined;
    }

    public void Set(int index, LjsObject value)
    {
        if (index < 0 || index > _list.Count) return;
        
        if (index < _list.Count)
        {
            _list[index] = value;
        }
        else if (index == _list.Count)
        {
            _list.Add(value);
        }
    }

    // METHODS AND PROPS

    private static LjsArray CheckThisArgument(LjsObject obj)
    {
        if (obj is not LjsArray a)
            throw new LjsRuntimeError("'this' value is not LjsArray");
        return a;
    }
    
    private sealed class PropLength : LjsProperty
    {
        public override LjsMemberType MemberType => LjsMemberType.InstanceMember;
        public override LjsPropertyAccessType AccessType => LjsPropertyAccessType.Read;
        public override LjsObject Get(LjsObject instance)
        {
            var s = CheckThisArgument(instance);
            return s.Count;
        }

        public override void Set(LjsObject instance, LjsObject v)
        {
            throw new NotImplementedException();
        }
    }
    
    private sealed class FuncIndexOf  : LjsFunction
    {
        public override LjsMemberType MemberType => LjsMemberType.InstanceMember;
        public override int ArgumentsCount => 3;
        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            var a = CheckThisArgument(arguments[0]);
            var searchValue = arguments[1];
            var startIndex = Math.Max(LjsTypesCoercionUtil.ToInt(arguments[2]), 0);

            if (a.Count == 0 || startIndex >= a.Count) return -1;

            for (var i = startIndex; i < a.Count; i++)
            {
                var e = a[i];
                if (e.Equals(searchValue)) return i;
            }

            return -1;
        }
    }
    
    private sealed class FuncConcat  : LjsFunction
    {
        public override LjsMemberType MemberType => LjsMemberType.InstanceMember;
        public override int ArgumentsCount => 5; // concat(this, a1, a2(opt), a3(opt), a4(opt))
        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            var a = CheckThisArgument(arguments[0]);

            var result = new LjsArray(a.Elements);

            for (var argumentIndex = 1; argumentIndex < arguments.Count; argumentIndex++)
            {
                var other = arguments[argumentIndex];
                if (other is LjsArray otherArray)
                {
                    for (var i = 0; i < otherArray.Count; i++)
                    {
                        result.Add(otherArray[i]);
                    }
                }
                else
                {
                    break;
                }
            }

            return result;
        }
    }
    
    private sealed class FuncPush  : LjsFunction
    {
        public override LjsMemberType MemberType => LjsMemberType.InstanceMember;
        public override int ArgumentsCount => 5; // push(this, v1, v2(opt), v3(opt), v4(opt))
        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            var a = CheckThisArgument(arguments[0]);

            for (var argumentIndex = 1; argumentIndex < arguments.Count; argumentIndex++)
            {
                var other = arguments[argumentIndex];
                if (other is not LjsUndefined)
                {
                    a.Add(other);
                }
                else
                {
                    break;
                }
            }

            return a.Count;
        }
    }
    
    private sealed class FuncShift  : LjsFunction
    {
        public override LjsMemberType MemberType => LjsMemberType.InstanceMember;
        public override int ArgumentsCount => 1;
        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            var a = CheckThisArgument(arguments[0]);

            return a.Count == 0 ? Undefined : a.RemoveFirst();
        }
    }
    
    private sealed class FuncPop  : LjsFunction
    {
        public override LjsMemberType MemberType => LjsMemberType.InstanceMember;
        public override int ArgumentsCount => 1;
        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            var a = CheckThisArgument(arguments[0]);

            return a.Count == 0 ? Undefined : a.RemoveLast();
        }
    }
    
    private sealed class FuncUnshift  : LjsFunction
    {
        public override LjsMemberType MemberType => LjsMemberType.InstanceMember;
        public override int ArgumentsCount => 5; // unshift(this, v1, v2(opt), v3(opt), v4(opt))
        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            var a = CheckThisArgument(arguments[0]);
            
            for (var argumentIndex = 1; argumentIndex < arguments.Count; argumentIndex++)
            {
                var v = arguments[argumentIndex];
                if (v is not LjsUndefined)
                {
                    a.Insert( argumentIndex - 1, v);
                }
                else
                {
                    break;
                }
            }

            return a.Count;
        }
    }
    
    private sealed class FuncSlice  : LjsFunction
    {
        public override LjsMemberType MemberType => LjsMemberType.InstanceMember;
        public override int ArgumentsCount => 3;
        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            var a = CheckThisArgument(arguments[0]);

            if (a.Count == 0) return new LjsArray();

            var startIndex = Math.Clamp(arguments[1] is LjsNumber nStart ? nStart.IntegerValue : 0, 0, a.Count - 1);
            var endIndex = Math.Clamp(
                arguments[2] is LjsNumber nEnd ? nEnd.IntegerValue : a.Count, 
                startIndex, a.Count);

            var ln = endIndex - startIndex;
            
            if (ln <= 0) return new LjsArray();

            var resultList = new List<LjsObject>(ln);

            for (var i = 0; i < ln; i++)
            {
                resultList.Add(a[startIndex + i]);
            }

            return new LjsArray(resultList);
        }
    }
    
}