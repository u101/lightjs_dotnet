namespace LightJS.Runtime;

public sealed class LjsDictionary : LjsObject
{
    private readonly Dictionary<string, LjsObject> _map;

    public IReadOnlyDictionary<string, LjsObject> Map => _map;

    public LjsDictionary()
    {
        _map = new Dictionary<string, LjsObject>();
    }

    public LjsDictionary(Dictionary<string, LjsObject> map)
    {
        _map = map;
    }

    public LjsDictionary(IEnumerable<KeyValuePair<string, LjsObject>> elements)
    {
        _map = new Dictionary<string, LjsObject>(elements);
    }

    public bool ContainsKey(string key) => _map.ContainsKey(key);

    public LjsObject Get(string key) => _map.TryGetValue(key, out var v) ? v : Undefined;

    public void Set(string key, LjsObject value)
    {
        _map[key] = value;
    }

    public int Count => _map.Count;

}