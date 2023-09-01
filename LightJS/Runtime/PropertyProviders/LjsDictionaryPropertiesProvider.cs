namespace LightJS.Runtime.PropertyProviders;

public sealed class LjsDictionaryPropertiesProvider : ILjsObjectPropertiesProvider
{
    public bool HasProperty(LjsObject target, string propertyName)
    {
        if (LjsObjectPropertiesProviderDefault.Instance.HasProperty(target, propertyName))
            return true;
        
        return target is LjsDictionary dict && dict.ContainsKey(propertyName);
    }

    public LjsPropertyAccessType GetPropertyAccessType(LjsObject target, string propertyName)
    {
        if (LjsObjectPropertiesProviderDefault.Instance.HasProperty(target, propertyName))
            return LjsObjectPropertiesProviderDefault.Instance.GetPropertyAccessType(target, propertyName);
        
        return LjsPropertyAccessType.All;
    }

    public LjsObject GetProperty(LjsObject target, string propertyName)
    {
        if (target is not LjsDictionary dict ||
            LjsObjectPropertiesProviderDefault.Instance.HasProperty(target, propertyName))
            return LjsObjectPropertiesProviderDefault.Instance.GetProperty(target, propertyName);
        
        return dict.Get(propertyName);
    }

    public void SetProperty(LjsObject target, string propertyName, LjsObject value)
    {
        if (target is LjsDictionary dict)
        {
            dict.Set(propertyName, value);
        }
    }

    public LjsObject GetProperty(LjsObject target, LjsObject propertyName)
    {
        return GetProperty(target, propertyName.ToString());
    }

    public void SetProperty(LjsObject target, LjsObject propertyName, LjsObject value)
    {
        SetProperty(target, propertyName.ToString(), value);
    }
}