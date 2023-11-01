namespace App16.LightJS.Runtime;

public sealed class LjsMath : LjsObject
{

    public static readonly LjsMath Instance = new();
    
    private static readonly LjsTypeInfo _TypeInfo = new(
        LjsObject.TypeInfo,
        new Dictionary<string, LjsObject>()
        {
            { "PI", LjsFunctionsFactory.CreateStaticProp(new LjsDouble(Math.PI)) },
            { "E", LjsFunctionsFactory.CreateStaticProp(new LjsDouble(Math.E)) },
            
            { "abs", LjsFunctionsFactory.CreateStatic((x) =>
            {
                if (x is LjsInteger i)
                    return i.Value >= 0 ? i : new LjsInteger(-i.Value);
                if (x is LjsDouble d)
                    return d.Value >= 0 ? d : new LjsDouble(-d.Value);
                return LjsDouble.Zero;
            }) },
            { "sqrt", LjsFunctionsFactory.CreateStatic((x) =>
            {
                if (x is LjsNumber i && i.NumericValue >= 0)
                    return Math.Sqrt(i.NumericValue);
                return LjsDouble.NaN;
            }) },
            {"sin", LjsFunctionsFactory.CreateStatic((x) =>
            {
                if (x is LjsNumber i)
                    return Math.Sin(i.NumericValue);
                return LjsDouble.NaN;
            })},
            {"cos", LjsFunctionsFactory.CreateStatic((x) =>
            {
                if (x is LjsNumber i)
                    return Math.Cos(i.NumericValue);
                return LjsDouble.NaN;
            })},
            {"floor", LjsFunctionsFactory.CreateStatic((x) =>
            {
                if (x is LjsNumber i)
                    return (int) Math.Floor(i.NumericValue);
                return LjsDouble.NaN;
            })},
            {"ceil", LjsFunctionsFactory.CreateStatic((x) =>
            {
                if (x is LjsNumber i)
                    return (int) Math.Ceiling(i.NumericValue);
                return LjsDouble.NaN;
            })}
        });

    public override LjsTypeInfo GetTypeInfo() => _TypeInfo;
    
    private LjsMath() {}
    
}