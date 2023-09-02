using LightJS.Errors;

namespace LightJS.Runtime;

public sealed class LjsArray : LjsObject, ILjsCollection
{
    private static readonly LjsTypeInfo _TypeInfo = new(
        LjsObject.TypeInfo,
        new Dictionary<string, LjsObject>
        {
            { "length", new PropLength() }
        });

    public override LjsTypeInfo GetTypeInfo() => _TypeInfo;
    
    private readonly List<LjsObject> _list;

    public IReadOnlyList<LjsObject> List => _list;

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
    
    public LjsArray(List<LjsObject> list)
    {
        _list = list ?? throw new ArgumentNullException(nameof(list));
    }

    public void Add(LjsObject o)
    {
        _list.Add(o);
    }

    public void Fill(int newSize, LjsObject v)
    {
        while (_list.Count < newSize)
        {
            _list.Add(v);
        }
    }

    public LjsObject this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    public LjsObject Get(LjsObject elementId)
    {
        var index = LjsTypesConverter.ToInt(elementId);

        return (index >= 0 && index < _list.Count) ? 
            _list[index] : LjsObject.Undefined;
    }

    public void Set(LjsObject elementId, LjsObject value)
    {
        var index = LjsTypesConverter.ToInt(elementId);

        if (index >= 0)
        {
            if (index < _list.Count)
            {
                _list[index] = value;
            }
            else if (index == _list.Count)
            {
                _list.Add(value);
            }
        }
        // TODO throw exception ??
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
    
}