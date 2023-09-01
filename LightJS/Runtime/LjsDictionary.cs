namespace LightJS.Runtime;

public sealed class LjsDictionary : LjsObject
{
    private readonly Dictionary<string, LjsObject> _map = new();

    public bool ContainsKey(string key) => _map.ContainsKey(key);

    public LjsObject Get(string key) => _map.TryGetValue(key, out var v) ? v : Undefined;

    public void Set(string key, LjsObject value)
    {
        _map[key] = value;
    }

    public int Count => _map.Count;

}