namespace App16.LightJS.Runtime;

public class LjsPointersStack
{
    private const int InitialCapacity = 32;
        
    private int[] _indices = new int[InitialCapacity];
    private LjsObject[] _pointers = new LjsObject[InitialCapacity];

    private int _size = 0;

    private void EnsureCapacity(int required)
    {
        if (_indices.Length >= required) return;

        var newCapacity = Math.Max(required, _indices.Length * 2);

        var newIndices = new int[newCapacity];
        var newPointers = new LjsObject[newCapacity];
        
        Array.Copy(_indices, newIndices, _indices.Length);
        Array.Copy(_pointers, newPointers, _pointers.Length);

        _indices = newIndices;
        _pointers = newPointers;
    }
    
    public void PushPointer(int stackIndex, LjsObject pointer)
    {
        EnsureCapacity(_size + 1);

        var lastIndex = _size > 0 ? _indices[_size - 1] : -1;

        if (lastIndex >= stackIndex)
            throw new Exception($"stackIndex {stackIndex} == lastIndex {lastIndex}");

        _indices[_size] = stackIndex;
        _pointers[_size] = pointer;
        ++_size;
    }

    public bool TryGetPointer(int stackIndex, out LjsObject pointer)
    {
        var i = _size - 1;
        while (i >= 0)
        {
            var j = _indices[i];
            if (j == stackIndex)
            {
                pointer = _pointers[i];
                return true;
            }

            --i;
        }
        
        pointer = LjsObject.Undefined;
        return false;
    }

    public void Clear(int currentStackIndex)
    {
        while (_size - 1 >= 0 && _indices[_size - 1] >= currentStackIndex)
        {
            _pointers[_size - 1] = LjsObject.Undefined;
            --_size;
        }
    }
    
}