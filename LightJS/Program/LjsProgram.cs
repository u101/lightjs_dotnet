using System.Text;
using LightJS.Errors;

namespace LightJS.Program;

public sealed class LjsProgram
{
    private readonly List<int> _integerConstants = new();
    private readonly List<double> _doubleConstants = new();
    private readonly List<string> _stringConstants = new();
    private readonly List<LjsInstruction> _instructions = new();

    public IReadOnlyList<LjsInstruction> Instructions => _instructions;
    

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

    public int GetIntegerConstant(short index) => GetConstant(index, _integerConstants);
    public double GetDoubleConstant(short index) => GetConstant(index, _doubleConstants);
    public string GetStringConstant(short index) => GetConstant(index, _stringConstants);

    private static TConstType GetConstant<TConstType>(short index, List<TConstType> constantsList)
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

    public int InstructionsCount => _instructions.Count;

    public void AddInstruction(LjsInstruction instruction)
    {
        _instructions.Add(instruction);
    }

    public void SetInstructionAt(LjsInstruction instruction, int index)
    {
        _instructions[index] = instruction;
    }

    public string GetProgramString()
    {
        var sb = new StringBuilder();

        sb.Append("int constants:\n");
        
        foreach (var i in _integerConstants)
        {
            sb.Append($"{i}\n");
        }
        
        sb.Append("double constants:\n");
        
        foreach (var i in _doubleConstants)
        {
            sb.Append($"{i}\n");
        }
        
        sb.Append("string constants:\n");
        
        foreach (var i in _stringConstants)
        {
            sb.Append($"{i}\n");
        }

        sb.Append("instructions:\n");

        foreach (var i in _instructions)
        {
            sb.Append($"{i.Code} {i.Index}");
        }

        return sb.ToString();
    }


    // to be done
}