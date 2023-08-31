namespace LightJS.Runtime;

public abstract class LjsExternalFunction : LjsObject
{
    public abstract int ArgumentsCount { get; }

    public abstract LjsObject Invoke(List<LjsObject> arguments);

    public static LjsExternalFunction Create(Func<LjsObject> f) => new LjsExternalFunction0(f);
    public static LjsExternalFunction Create(Action f) => new LjsExternalAction0(f);

    public static LjsExternalFunction Create(Func<LjsObject, LjsObject> f) => new LjsExternalFunction1(f);
    public static LjsExternalFunction Create(Action<LjsObject> f) => new LjsExternalAction1(f);

    public static LjsExternalFunction Create(Func<LjsObject, LjsObject, LjsObject> f) => new LjsExternalFunction2(f);
    public static LjsExternalFunction Create(Action<LjsObject, LjsObject> f) => new LjsExternalAction2(f);

    private sealed class LjsExternalAction0 : LjsExternalFunction
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

    private sealed class LjsExternalFunction0 : LjsExternalFunction
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

    private sealed class LjsExternalAction1 : LjsExternalFunction
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

    private sealed class LjsExternalFunction1 : LjsExternalFunction
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

    private sealed class LjsExternalAction2 : LjsExternalFunction
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

    private sealed class LjsExternalFunction2 : LjsExternalFunction
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
}