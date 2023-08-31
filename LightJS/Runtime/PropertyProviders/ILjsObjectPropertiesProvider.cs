namespace LightJS.Runtime.PropertyProviders;

public interface ILjsObjectPropertiesProvider
{
    bool HasProperty(string propertyName);

    LjsPropertyAccessType GetPropertyAccessType(string propertyName);
    
    LjsObject GetProperty(LjsObject target, string propertyName);
    
    void SetProperty(LjsObject target, string propertyName, LjsObject value);
    
    LjsObject GetProperty(LjsObject target, LjsObject propertyName);
    
    void SetProperty(LjsObject target, LjsObject propertyName, LjsObject value);
}