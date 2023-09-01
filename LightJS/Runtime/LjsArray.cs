namespace LightJS.Runtime;

public sealed class LjsArray : LjsObject
{
    private readonly List<LjsObject> _list;

    public IReadOnlyList<LjsObject> List => _list;

    public int Count => _list.Count;

    public LjsArray()
    {
        _list = new List<LjsObject>();
    }
    
    public LjsArray(List<LjsObject> list)
    {
        _list = list ?? throw new ArgumentNullException(nameof(list));
    }

    public void Add(LjsObject o)
    {
        _list.Add(o);
    }
}