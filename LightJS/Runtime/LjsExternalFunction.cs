namespace LightJS.Runtime;

public abstract class LjsExternalFunction : LjsObject
{
    public abstract int ArgumentsCount { get; }

    public abstract LjsObject Invoke(List<LjsObject> arguments);

}

public sealed class LjsExternalFunction0 : LjsExternalFunction
{
    private readonly Func<LjsObject> _func;
    
    public override int ArgumentsCount => 0;

    public LjsExternalFunction0(Func<LjsObject> func)
    {
        _func = func;
    }

    public override LjsObject Invoke(List<LjsObject> arguments)
    {
        return _func.Invoke();
    }
}