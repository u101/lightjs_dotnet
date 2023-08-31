namespace LightJS.Runtime.PropertyProviders;

public static class LjsObjectFunctionFactory
{

    public static ILjsObjectProperty Create(Action<LjsObject> func) => new ActionFactory0(func); 
    public static ILjsObjectProperty Create(Func<LjsObject, LjsObject> func) => new FuncFactory0(func); 
    
    
    private sealed class ActionFactory0 : ILjsObjectProperty
    {
        private readonly Action<LjsObject> _func;

        public ActionFactory0(Action<LjsObject> func)
        {
            _func = func;
        }

        public LjsObject GetProperty(LjsObject target)
        {
            return LjsObjectFunction.Create(target, _func);
        }
    }
    
    private sealed class FuncFactory0 : ILjsObjectProperty
    {
        private readonly Func<LjsObject, LjsObject> _func;

        public FuncFactory0(Func<LjsObject, LjsObject> func)
        {
            _func = func;
        }

        public LjsObject GetProperty(LjsObject target)
        {
            return LjsObjectFunction.Create(target, _func);
        }
    }
}