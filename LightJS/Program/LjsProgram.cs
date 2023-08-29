namespace LightJS.Program;

public sealed class LjsProgram
{

    public LjsProgramConstants Constants { get; }
    
    private readonly Dictionary<string, int> _namedFunctionsMap;
    private readonly List<LjsFunction> _functionsList;

    public LjsProgram(
        LjsProgramConstants constants,
        List<LjsFunction> functionsList,
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

    public LjsFunction MainFunction => _functionsList[0];

    public IEnumerable<string> FunctionsNames => _namedFunctionsMap.Keys;

    public bool ContainsFunction(string name) => _namedFunctionsMap.ContainsKey(name);
    
    public LjsFunction GetFunction(string name)
    {
        if (_namedFunctionsMap.TryGetValue(name, out var index))
        {
            return _functionsList[index];
        }

        throw new Exception($"function '{name}' not found");
    }

    public LjsFunction GetFunction(int index)
    {
        if (index < 0 || index >= _functionsList.Count)
            throw new IndexOutOfRangeException($"index {index} out of range [0..{_functionsList.Count}]");
        return _functionsList[index];
    }
    

}