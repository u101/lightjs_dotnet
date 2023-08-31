namespace LightJS.Runtime.PropertyProviders;

public abstract class LjsObjectFunction : LjsFunction
{
    
    public static LjsObjectFunction Create(LjsObject target, Func<LjsObject, LjsObject> f) => new Function0(target, f);
    public static LjsObjectFunction Create(LjsObject target, Action<LjsObject> f) => new Action0(target, f);

    private sealed class Action0 : LjsObjectFunction
    {
        private readonly LjsObject _target;
        private readonly Action<LjsObject> _func;

        public override int ArgumentsCount => 0;

        public Action0(LjsObject target, Action<LjsObject> func)
        {
            _target = target;
            _func = func;
        }

        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            _func.Invoke(_target);
            return Undefined;
        }
    }

    private sealed class Function0 : LjsObjectFunction
    {
        private readonly LjsObject _target;
        private readonly Func<LjsObject, LjsObject> _func;

        public override int ArgumentsCount => 0;

        public Function0(LjsObject target, Func<LjsObject, LjsObject> func)
        {
            _target = target;
            _func = func;
        }

        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            return _func.Invoke(_target);
        }
    }

    
    
}