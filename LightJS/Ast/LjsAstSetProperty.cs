namespace LightJS.Ast;

public class LjsAstSetProperty : ILjsAstNode, ILjsAstSetterNode
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
}