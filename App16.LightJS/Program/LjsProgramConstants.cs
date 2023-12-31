using App16.LightJS.Errors;

namespace App16.LightJS.Program;

public class LjsProgramConstants
{
    private readonly List<double> _doubleConstants = new();
    private readonly List<string> _stringConstants = new();

    /// <summary>
    /// returns constant index
    /// </summary>
    public short AddDoubleConstant(double value) => AddConstant(value, _doubleConstants);
    
    /// <summary>
    /// returns constant index
    /// </summary>
    public short AddStringConstant(string value) => AddConstant(value, _stringConstants);
    public double GetDoubleConstant(int index) => GetConstant(index, _doubleConstants);
    public string GetStringConstant(int index) => GetConstant(index, _stringConstants);

    private static TConstType GetConstant<TConstType>(int index, List<TConstType> constantsList)
    {
        if (index < 0 || index >= constantsList.Count)
            throw new LjsInternalError(
                $"internal error: failed to get constant at index {index} of type {typeof(TConstType).Name}");

        return constantsList[index];
    }

    private static short AddConstant<TConstType>(TConstType value, List<TConstType> constantsList)
    {
        if (constantsList.Count == 0)
        {
            constantsList.Add(value);
            return 0;
        }

        var i = constantsList.IndexOf(value);
        if (i >= 0) return (short) i;

        if (constantsList.Count == short.MaxValue)
        {
            throw new LjsCompilerError("constants pool overflow");
        }
        
        i = constantsList.Count;
        
        constantsList.Add(value);
        
        return (short) i;
    }
}