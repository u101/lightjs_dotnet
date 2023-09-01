using LightJS.Errors;

namespace LightJS.Runtime;

public sealed class LjsArray : LjsObject
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
    
    public LjsArray(List<LjsObject> list)
    {
        _list = list ?? throw new ArgumentNullException(nameof(list));
    }

    public void Add(LjsObject o)
    {
        _list.Add(o);
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
        public override LjsPropertyAccessType AccessType => LjsPropertyAccessType.Write;
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