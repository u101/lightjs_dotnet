namespace App16.LightJS.Runtime;

public static class LjsRuntimeUtils
{

    public static int CombineTwoShorts(int v0, int v1)
    {
        if (v0 < 0 || v0 >= ushort.MaxValue)
            throw new ArgumentException($"v0 {v0} out of range");
        
        if (v1 < 0 || v1 >= ushort.MaxValue)
            throw new ArgumentException($"v1 {v0} out of range");

        return (v0 & 0x0000FFFF) | (v1 << 16);
    }

    public static int GetLocalIndex(int combinedValue) => combinedValue & 0x0000FFFF;
    public static int GetFunctionIndex(int combinedValue) => (combinedValue >>> 16) & 0x0000FFFF;


    private static readonly List<List<LjsObject>> ObjectsListsPool = new();

    public static List<LjsObject> GetTemporaryObjectsList()
    {
        if (ObjectsListsPool.Count > 0)
        {
            var list = ObjectsListsPool[^1];
            ObjectsListsPool.RemoveAt(ObjectsListsPool.Count - 1);
            list.Clear();
            return list;
        }

        return new List<LjsObject>(8);
    }

    public static void ReleaseTemporaryObjectsList(List<LjsObject> list)
    {
        list.Clear();
        ObjectsListsPool.Add(list);
    }

    public static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
    
}