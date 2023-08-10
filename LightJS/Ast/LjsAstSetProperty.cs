namespace LightJS.Ast;

public class LjsAstSetProperty : ILjsAstNode
{
    public ILjsAstNode PropertyName { get; }
    public ILjsAstNode PropertySource { get; }
    public ILjsAstNode AssignmentExpression { get; }
    public LjsAstAssignMode AssignMode { get; }

    public LjsAstSetProperty(
        ILjsAstNode propertyName, 
        ILjsAstNode propertySource, 
        ILjsAstNode assignmentExpression, 
        LjsAstAssignMode assignMode)
    {
        PropertyName = propertyName;
        PropertySource = propertySource;
        AssignmentExpression = assignmentExpression;
        AssignMode = assignMode;
    }

    public IEnumerable<ILjsAstNode> ChildNodes => new[] { PropertyName, PropertySource, AssignmentExpression };
    public bool HasChildNodes => true;
}