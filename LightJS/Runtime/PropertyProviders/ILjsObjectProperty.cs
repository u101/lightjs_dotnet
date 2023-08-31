namespace LightJS.Runtime.PropertyProviders;

public interface ILjsObjectProperty
{
    LjsObject GetProperty(LjsObject target);
}