using LightJS.Runtime;

namespace LightJS.ExternalApi;

public class ExternalTypesConverter
{

    public LjsObject ToLjsObject(System.Type systemType, object obj)
    {
        if (systemType == typeof(bool))
            return (bool)obj ? LjsBoolean.True : LjsBoolean.False;
        
        if (systemType == typeof(int))
            return new LjsInteger((int)obj);
        
        if (systemType == typeof(double))
            return new LjsDouble((double)obj);

        if (systemType == typeof(string)) 
            return new LjsString((string)obj);
        
        
        throw new NotImplementedException();
    }

    public object ToSystemObject(System.Type systemType, LjsObject obj)
    {
        
        // todo use coercion util
        switch (obj)
        {
            case LjsInteger i:
                if (systemType != typeof(int)) throw new Exception();
                return i.Value;
            default:
                throw new Exception("type not supported");
        }
        
        throw new NotImplementedException();
    }
    
}