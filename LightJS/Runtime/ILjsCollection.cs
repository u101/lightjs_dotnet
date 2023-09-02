namespace LightJS.Runtime;

public interface ILjsCollection
{
    LjsObject Get(LjsObject elementId);
    void Set(LjsObject elementId, LjsObject value);
}