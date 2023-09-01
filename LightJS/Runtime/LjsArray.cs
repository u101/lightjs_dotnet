namespace LightJS.Runtime;

public sealed class LjsArray : LjsObject
{
    private readonly List<LjsObject> _list = new();

    public int Count => _list.Count;

    public void Add(LjsObject o)
    {
        _list.Add(o);
    }
}