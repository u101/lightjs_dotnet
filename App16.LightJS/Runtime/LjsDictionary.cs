namespace App16.LightJS.Runtime;

public sealed class LjsDictionary : LjsObject, ILjsDictionary
{
    private readonly Dictionary<string, LjsObject> _map;

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

    public string[] Keys => _map.Keys.ToArray();

    public bool ContainsKey(string key) => _map.ContainsKey(key);
    
    public LjsObject this[string key]
    {
        get => _map[key];
        set => _map[key] = value;
    }

    public LjsObject Get(string key) => _map.TryGetValue(key, out var v) ? v : Undefined;

    public void Set(string key, LjsObject value)
    {
        _map[key] = value;
    }

    public int Count => _map.Count;

    public LjsObject Get(LjsObject elementId)
    {
        var key = elementId.ToString();
        return _map.TryGetValue(key, out var result) ? result : LjsObject.Undefined;
    }

    public void Set(LjsObject elementId, LjsObject value)
    {
        var key = elementId.ToString();
        _map[key] = value;
    }
}