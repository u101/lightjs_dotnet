using LightJS.Errors;

namespace LightJS.Program;

public sealed class LjsProgram
{
    private readonly List<int> _integerConstants = new();
    private readonly List<double> _doubleConstants = new();
    private readonly List<string> _stringConstants = new();

    /// <summary>
    /// returns constant index
    /// </summary>
    public short AddIntegerConstant(int value) => AddConstant(value, _integerConstants);
    
    /// <summary>
    /// returns constant index
    /// </summary>
    public short AddDoubleConstant(double value) => AddConstant(value, _doubleConstants);
    
    /// <summary>
    /// returns constant index
    /// </summary>
    public short AddStringConstant(string value) => AddConstant(value, _stringConstants);

    private short AddConstant<TConstType>(TConstType value, List<TConstType> constantsList)
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

    public void AddInstruction(byte instruction)
    {
        
    }
    
    public void AddInstruction(byte instruction, short parameter)
    {
        
    }


    // to be done
}