using LightJS.Errors;

namespace LightJS.Runtime.PropertyProviders;

public sealed class LjsObjectPropertiesProviderDefault : ILjsObjectPropertiesProvider
{
    public static readonly ILjsObjectPropertiesProvider Instance = new LjsObjectPropertiesProviderDefault();
    
    private readonly Dictionary<string, ILjsObjectProperty> _properties = new()
    {
        {"toString", LjsObjectFunctionFactory.Create((o) => o.ToString())}
    };
    
    private LjsObjectPropertiesProviderDefault() {}


    public bool HasProperty(LjsObject target, string propertyName)
    {
        return _properties.ContainsKey(propertyName);
    }

    public LjsPropertyAccessType GetPropertyAccessType(LjsObject target, string propertyName)
    {
        return _properties.ContainsKey(propertyName) ? LjsPropertyAccessType.Read : LjsPropertyAccessType.None;
    }

    public LjsObject GetProperty(LjsObject target, string propertyName)
    {
        if (_properties.TryGetValue(propertyName, out var p))
        {
            return p.GetProperty(target);
        }
        
        return LjsObject.Undefined;
    }

    public void SetProperty(LjsObject target, string propertyName, LjsObject value)
    {
        throw new LjsRuntimeError($"fail to set property {propertyName}. {target} is not extensible");
    }

    public LjsObject GetProperty(LjsObject target, LjsObject propertyName)
    {
        return GetProperty(target, propertyName.ToString());
    }

    public void SetProperty(LjsObject target, LjsObject propertyName, LjsObject value)
    {
        throw new LjsRuntimeError($"fail to set property {propertyName}. {target} is not extensible");
    }
}