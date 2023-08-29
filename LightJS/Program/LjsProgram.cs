namespace LightJS.Program;

public sealed class LjsProgram
{

    public LjsProgramConstants Constants { get; }
    
    private readonly Dictionary<string, int> _namedFunctionsMap;
    private readonly List<LjsFunctionData> _functionsList;

    public LjsProgram(
        LjsProgramConstants constants,
        List<LjsFunctionData> functionsList,
        Dictionary<string, int> namedFunctionsMap)
    {
        if (functionsList == null)
            throw new ArgumentNullException(nameof(functionsList));
        if (functionsList.Count == 0)
            throw new ArgumentException(
                "functionsList must contain at leas one function", nameof(functionsList));
        
        Constants = constants;
        _namedFunctionsMap = namedFunctionsMap;
        _functionsList = functionsList;
    }

    public LjsFunctionData MainFunctionData => _functionsList[0];

    public IEnumerable<string> FunctionsNames => _namedFunctionsMap.Keys;

    public bool ContainsFunction(string name) => _namedFunctionsMap.ContainsKey(name);
    
    public LjsFunctionData GetFunction(string name)
    {
        if (_namedFunctionsMap.TryGetValue(name, out var index))
        {
            return _functionsList[index];
        }

        throw new Exception($"function '{name}' not found");
    }

    public LjsFunctionData GetFunction(int index)
    {
        if (index < 0 || index >= _functionsList.Count)
            throw new IndexOutOfRangeException($"index {index} out of range [0..{_functionsList.Count}]");
        return _functionsList[index];
    }
    

}