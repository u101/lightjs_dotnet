using LightJS.Runtime;

namespace LightJS.ExternalApi;

public sealed class ExternalObjectAdapter : LjsObject
{
    private readonly LjsTypeInfo _typeInfo;
    private readonly object _target;

    public ExternalObjectAdapter(LjsTypeInfo typeInfo, object target)
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
        return other is ExternalObjectAdapter a && a._target.Equals(_target);
    }

    public override int GetHashCode()
    {
        return _target.GetHashCode();
    }
}