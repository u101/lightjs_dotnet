using System.Reflection;

namespace LightJS.Runtime;

public static class LjsTypesConverter
{
    private static readonly Dictionary<System.Type, LjsTypeInfo> TypesMap = new();
    
    
    public static LjsObject ToLjsObject(object obj) 
    {
        if (obj == null) 
            return LjsObject.Null;

        var systemType = obj.GetType();

        if (systemType == typeof(bool))
            return (bool)obj ? LjsBoolean.True : LjsBoolean.False;
        
        if (systemType == typeof(int))
            return new LjsInteger((int)obj);
        
        if (systemType == typeof(double))
            return new LjsDouble((double)obj);

        if (systemType == typeof(string)) 
            return new LjsString((string)obj);
        
        if (TypesMap.TryGetValue(systemType, out var typeInfo))
        {
            return new ObjectAdapter(typeInfo, obj);
        }
        
        typeInfo = new LjsTypeInfo(LjsObject.TypeInfo);
        
        var fieldInfos = systemType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var fieldInfo in fieldInfos)
        {
            var ljsFieldAttr = fieldInfo.GetCustomAttribute<LjsField>();
            if (ljsFieldAttr == null) continue;
            
            typeInfo.AddMember(fieldInfo.Name,
                new FieldAdapter(fieldInfo, fieldInfo.IsStatic ? LjsMemberType.StaticMember : LjsMemberType.InstanceMember));;
            
        }

        TypesMap[systemType] = typeInfo;

        return new ObjectAdapter(typeInfo, obj);
    }

    public static object ToSystemObject(System.Type systemType, LjsObject ljsObject)
    {
        if (systemType == typeof(bool))
            return LjsTypesCoercionUtil.ToBool(ljsObject);
        
        if (systemType == typeof(int))
            return LjsTypesCoercionUtil.ToInt(ljsObject);
        
        if (systemType == typeof(double))
            return LjsTypesCoercionUtil.ToDouble(ljsObject);

        if (systemType == typeof(string))
            return ljsObject.ToString();


        if (ljsObject is ObjectAdapter adapter &&
            systemType == adapter.Target.GetType())
        {
            return adapter.Target;
        }

        throw new Exception($"types conversion fail. from {ljsObject.GetType().Name} to {systemType.Name}");


    }
    
    private sealed class ObjectAdapter : LjsObject
    {
        private readonly LjsTypeInfo _typeInfo;
        private readonly object _target;

        public ObjectAdapter(LjsTypeInfo typeInfo, object target)
        {
            _typeInfo = typeInfo;
            _target = target;
        }

        public object Target => _target;

        public override LjsTypeInfo GetTypeInfo() => _typeInfo;

        public override string ToString()
        {
            return _target.ToString() ?? string.Empty;
        }

        public override bool Equals(LjsObject? other)
        {
            return other is ObjectAdapter a && a._target.Equals(_target);
        }

        public override int GetHashCode()
        {
            return _target.GetHashCode();
        }
    }
    
    private class FieldAdapter : LjsProperty
    {
        private readonly FieldInfo _fieldInfo;

        public FieldAdapter(
            FieldInfo fieldInfo, 
            LjsMemberType memberType)
        {
            _fieldInfo = fieldInfo;
            MemberType = memberType;
        }
        
        public override LjsMemberType MemberType { get; }

        public override LjsPropertyAccessType AccessType => LjsPropertyAccessType.All;
        public override LjsObject Get(LjsObject instance)
        {
            switch (MemberType)
            {
                case LjsMemberType.InstanceMember:
                    
                    if (instance is not ObjectAdapter obj)
                        throw new ArgumentException(
                            $"instance of type {instance.GetType().Name} is not {nameof(ObjectAdapter)}");

                    var value = _fieldInfo.GetValue(obj.Target);

                    return value == null ? Null : ToLjsObject(value);

                case LjsMemberType.StaticMember:
                    var staticValue = _fieldInfo.GetValue(null);
                    
                    
                    return staticValue == null ? Null : ToLjsObject(staticValue);
                
                default:
                    throw new ArgumentOutOfRangeException(MemberType.ToString());
            }
            
            
        }

        public override void Set(LjsObject instance, LjsObject v)
        {
            if (instance is not ObjectAdapter obj)
                throw new ArgumentException(
                    $"instance of type {instance.GetType().Name} is not {nameof(ObjectAdapter)}");
            
            _fieldInfo.SetValue(obj.Target, ToSystemObject(_fieldInfo.FieldType, v));
        }
    }
    
}