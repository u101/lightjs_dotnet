namespace LightJS.Runtime;

public sealed class LjsTypeInfo
{
    private readonly LjsTypeInfo? _baseTypeInfo;
    
    private readonly Dictionary<string, LjsObject> _typeMembers;

    public bool HasBaseType => _baseTypeInfo != null;

    public LjsTypeInfo BaseTypeInfo => _baseTypeInfo ?? this;
    

    public LjsTypeInfo()
    {
        _baseTypeInfo = null;
        _typeMembers = new Dictionary<string, LjsObject>();
    }
    
    public LjsTypeInfo(Dictionary<string, LjsObject> typeMembers)
    {
        _baseTypeInfo = null;
        _typeMembers = typeMembers;
    }
    
    public LjsTypeInfo(LjsTypeInfo baseTypeInfo)
    {
        _baseTypeInfo = baseTypeInfo;
        _typeMembers = new Dictionary<string, LjsObject>();
    }
    
    public LjsTypeInfo(LjsTypeInfo baseTypeInfo, Dictionary<string, LjsObject> typeMembers)
    {
        _baseTypeInfo = baseTypeInfo;
        _typeMembers = typeMembers;
    }

    public void AddMember(string name, LjsObject member)
    {
        _typeMembers[name] = member;
    }

    public bool HasMember(string name) =>
        _typeMembers.ContainsKey(name) || (_baseTypeInfo != null && _baseTypeInfo.HasMember(name));
    
    public LjsObject GetMember(string name)
    {
        if (_typeMembers.TryGetValue(name, out var member)) return member;

        return _baseTypeInfo != null ? _baseTypeInfo.GetMember(name) : LjsObject.Undefined;
    }


}