namespace App16.LightJS.Runtime;

public interface ILjsDictionary
{
    LjsObject Get(LjsObject key);
    void Set(LjsObject key, LjsObject value);
}