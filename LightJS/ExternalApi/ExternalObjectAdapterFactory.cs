using System.Reflection;
using LightJS.Runtime;

namespace LightJS.ExternalApi;

public static class ExternalObjectAdapterFactory
{

    private static readonly Dictionary<System.Type, LjsTypeInfo> _typesMap = new();


    public static LjsObject CreateObjectAdapter(object obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        var systemType = obj.GetType();

        if (_typesMap.TryGetValue(systemType, out var typeInfo))
        {
            return new ExternalObjectAdapter(typeInfo, obj);
        }

        typeInfo = new LjsTypeInfo(LjsObject.TypeInfo);
        
        var fieldInfos = systemType.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var fieldInfo in fieldInfos)
        {
            var ljsFieldAttr = fieldInfo.GetCustomAttribute<LjsField>();
            if (ljsFieldAttr == null) continue;
            
            typeInfo.AddMember(fieldInfo.Name,
                new FieldAdapter(fieldInfo, LjsMemberType.InstanceMember, new ExternalTypesConverter()));;
            
        }

        _typesMap[systemType] = typeInfo;

        return new ExternalObjectAdapter(typeInfo, obj);
    }
    
    private class FieldAdapter : LjsProperty
    {
        private readonly FieldInfo _fieldInfo;
        private readonly ExternalTypesConverter _typesConverter;

        public FieldAdapter(
            FieldInfo fieldInfo, 
            LjsMemberType memberType,
            ExternalTypesConverter typesConverter)
        {
            _fieldInfo = fieldInfo;
            _typesConverter = typesConverter;
            MemberType = memberType;
        }
        
        public override LjsMemberType MemberType { get; }

        public override LjsPropertyAccessType AccessType => LjsPropertyAccessType.All;
        public override LjsObject Get(LjsObject instance)
        {
            if (instance is not ExternalObjectAdapter obj)
                throw new ArgumentException(
                    $"instance of type {instance.GetType().Name} is not {nameof(ExternalObjectAdapter)}");

            var value = _fieldInfo.GetValue(obj.Target);

            if (value == null) return Null;

            return _typesConverter.ToLjsObject(_fieldInfo.FieldType, value);
        }

        public override void Set(LjsObject instance, LjsObject v)
        {
            if (instance is not ExternalObjectAdapter obj)
                throw new ArgumentException(
                    $"instance of type {instance.GetType().Name} is not {nameof(ExternalObjectAdapter)}");
            
            _fieldInfo.SetValue(obj.Target, _typesConverter.ToSystemObject(_fieldInfo.FieldType, v));
        }
    }


}