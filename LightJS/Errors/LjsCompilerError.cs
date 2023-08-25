namespace LightJS.Errors;

public class LjsCompilerError : Exception
{
    
    public LjsCompilerError() {}
    
    public LjsCompilerError(string errorMessage):base(errorMessage) {}
}