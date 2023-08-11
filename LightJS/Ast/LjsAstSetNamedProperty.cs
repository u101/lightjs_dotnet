namespace LightJS.Ast;

public class LjsAstSetNamedProperty : ILjsAstNode, ILjsAstSetterNode
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

    public IEnumerable<ILjsAstNode> ChildNodes => new[] { PropertySource, AssignmentExpression };
    public bool HasChildNodes => true;
}