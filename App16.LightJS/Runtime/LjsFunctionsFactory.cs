namespace App16.LightJS.Runtime;

public static class LjsFunctionsFactory
{
    public static LjsFunction CreateStatic(Func<LjsObject> f) => new LjsExternalFunction0(f);
    public static LjsFunction CreateStatic(Action f) => new LjsExternalAction0(f);

    public static LjsFunction CreateStatic(Func<LjsObject, LjsObject> f) => new LjsExternalFunction1(f);
    public static LjsFunction CreateStatic(Action<LjsObject> f) => new LjsExternalAction1(f);

    public static LjsFunction CreateStatic(Func<LjsObject, LjsObject, LjsObject> f) => new LjsExternalFunction2(f);
    public static LjsFunction CreateStatic(Action<LjsObject, LjsObject> f) => new LjsExternalAction2(f);

    public static LjsProperty CreateStaticProp(LjsObject value) => new StaticGetProp(value);

    private abstract class LjsStaticFunction : LjsFunction
    {
        public override LjsMemberType MemberType => LjsMemberType.StaticMember;
    }
    
    private sealed class LjsExternalAction0 : LjsStaticFunction
    {
        private readonly Action _func;

        public override int ArgumentsCount => 0;

        public LjsExternalAction0(Action func)
        {
            _func = func;
        }

        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            _func.Invoke();
            return Undefined;
        }
    }

    private sealed class LjsExternalFunction0 : LjsStaticFunction
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

    private sealed class LjsExternalAction1 : LjsStaticFunction
    {
        private readonly Action<LjsObject> _func;

        public override int ArgumentsCount => 1;

        public LjsExternalAction1(Action<LjsObject> func)
        {
            _func = func;
        }

        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            _func.Invoke(arguments[0]);
            return Undefined;
        }
    }

    private sealed class LjsExternalFunction1 : LjsStaticFunction
    {
        private readonly Func<LjsObject, LjsObject> _func;

        public override int ArgumentsCount => 1;

        public LjsExternalFunction1(Func<LjsObject, LjsObject> func)
        {
            _func = func;
        }

        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            return _func.Invoke(arguments[0]);
        }
    }

    private sealed class LjsExternalAction2 : LjsStaticFunction
    {
        private readonly Action<LjsObject, LjsObject> _func;

        public override int ArgumentsCount => 2;

        public LjsExternalAction2(Action<LjsObject, LjsObject> func)
        {
            _func = func;
        }

        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            _func.Invoke(arguments[0], arguments[1]);
            return Undefined;
        }
    }

    private sealed class LjsExternalFunction2 : LjsStaticFunction
    {
        private readonly Func<LjsObject, LjsObject, LjsObject> _func;

        public override int ArgumentsCount => 2;

        public LjsExternalFunction2(Func<LjsObject, LjsObject, LjsObject> func)
        {
            _func = func;
        }

        public override LjsObject Invoke(List<LjsObject> arguments)
        {
            return _func.Invoke(arguments[0], arguments[1]);
        }
    }

    private sealed class StaticGetProp : LjsProperty
    {
        private readonly LjsObject _value;
        public override LjsMemberType MemberType => LjsMemberType.StaticMember;
        public override LjsPropertyAccessType AccessType => LjsPropertyAccessType.Read;

        public StaticGetProp(LjsObject value)
        {
            _value = value;
        }
        
        public override LjsObject Get(LjsObject instance)
        {
            return _value;
        }

        public override void Set(LjsObject instance, LjsObject v)
        {
        }
    }
    
}