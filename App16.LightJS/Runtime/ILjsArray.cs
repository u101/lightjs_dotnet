namespace App16.LightJS.Runtime;

public interface ILjsArray
{
    LjsObject Get(int index);
    void Set(int index, LjsObject value);
}