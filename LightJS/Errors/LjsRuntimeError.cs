namespace LightJS.Errors;

public class LjsRuntimeError : Exception
{
    public LjsRuntimeError() {}
    
    public LjsRuntimeError(string errorMessage):base(errorMessage) {}
}