namespace LightJS.Ast;

public sealed class LjsAstSetNamedProperty : ILjsAstNode
{
    public string PropertyName { get; }
    public ILjsAstNode PropertySource { get; }
    public ILjsAstNode AssignmentExpression { get; }
    public LjsAstAssignMode AssignMode { get; }

    public LjsAstSetNamedProperty(
        string propertyName, 
        ILjsAstNode propertySource, 
        ILjsAstNode assignmentExpression, 
        LjsAstAssignMode assignMode)
    {
        PropertyName = propertyName;
        PropertySource = propertySource;
        AssignmentExpression = assignmentExpression;
        AssignMode = assignMode;
    }
    
}