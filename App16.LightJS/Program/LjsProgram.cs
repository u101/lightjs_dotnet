namespace App16.LightJS.Program;

public sealed class LjsProgram
{

    public LjsProgramConstants Constants { get; }
    
    private readonly Dictionary<string, int> _namedFunctionsMap;
    private readonly LjsFunctionData[] _functions;

    public LjsProgram(
        LjsProgramConstants constants,
        LjsFunctionData[] functions,
        Dictionary<string, int> namedFunctionsMap)
    {
        if (functions == null)
            throw new ArgumentNullException(nameof(functions));
        if (functions.Length == 0)
            throw new ArgumentException(
                "functions must contain at leas one function", nameof(functions));
        
        Constants = constants;
        _namedFunctionsMap = namedFunctionsMap;
        _functions = functions;
    }

    public LjsFunctionData MainFunctionData => _functions[0];

    public IEnumerable<string> FunctionsNames => _namedFunctionsMap.Keys;

    public bool ContainsFunction(string name) => _namedFunctionsMap.ContainsKey(name);
    
    public LjsFunctionData GetFunction(string name)
    {
        if (_namedFunctionsMap.TryGetValue(name, out var index))
        {
            return _functions[index];
        }

        throw new Exception($"function '{name}' not found");
    }

    public LjsFunctionData GetFunction(int index)
    {
        if (index < 0 || index >= _functions.Length)
            throw new IndexOutOfRangeException($"index {index} out of range [0..{_functions.Length}]");
        return _functions[index];
    }
    

}