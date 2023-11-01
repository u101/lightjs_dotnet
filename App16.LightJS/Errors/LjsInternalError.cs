namespace App16.LightJS.Errors;

public class LjsInternalError : Exception
{
    public LjsInternalError() {}
    
    public LjsInternalError(string errorMessage):base(errorMessage) {}
}