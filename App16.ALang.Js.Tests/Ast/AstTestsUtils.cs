using App16.ALang.Ast;
using App16.ALang.Js.Ast;
using App16.ALang.Js.Ast.Builders;
using FluentAssertions;

namespace App16.ALang.Js.Tests.Ast;

public static class AstTestsUtils
{
    public static void Match(IAstNode result, IAstNode expected)
    {
        result.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes().WithStrictOrdering());
    }
    
    public static void MatchNot(IAstNode result, IAstNode expected)
    {
        result.Should().NotBeEquivalentTo(expected, options => options.RespectingRuntimeTypes().WithStrictOrdering());
    }

    public static IAstNode BuildAstNode(string sourceCode)
    {
        var builder = JsAstBuilderFactory.CreateBuilder(sourceCode);
        return builder.Build();
    }

    #region Ast Nodes generator methods
    
    public static AstSequence Sequence(params IAstNode[] nodes)
    {
        return new AstSequence(nodes);
    }
    
    public static AstReturn Return(IAstNode returnValue) => new(returnValue);
    public static AstReturn Return() => new();
    
    public static AstWhileLoop While(IAstNode condition, IAstNode expression) => new(condition, expression);

    public static AstForLoop For(IAstNode init, IAstNode cond, IAstNode iter, IAstNode body) =>
        new(init, cond, iter, body);
    
    public static JsSwitchBlock Switch(IAstNode expression, AstSequence body) => new(expression, body);
    public static JsSwitchCase Case(IAstNode value) => new(value);
    public static JsSwitchCase Case(int value) => new(value.ToLit());
    public static JsSwitchCase Case(string value) => new(value.ToLit());
    
    public static AstIfBlock IfBlock(IAstNode condition, IAstNode expression)
    {
        var e = new AstConditionalExpression(condition, expression);
        return new AstIfBlock(e);
    }
    
    public static JsVariableDeclaration Var(string name) => new(name, JsVariableKind.Var);
    public static JsVariableDeclaration Var(string name, IAstNode value) => new(name, value, JsVariableKind.Var);
    
    public static JsVariableDeclaration Let(string name) => new(name, JsVariableKind.Let);
    public static JsVariableDeclaration Let(string name, IAstNode value) => new(name, value, JsVariableKind.Let);
    
    public static JsVariableDeclaration Const(string name) => new(name, JsVariableKind.Const);
    public static JsVariableDeclaration Const(string name, IAstNode value) => new(name, value, JsVariableKind.Const);
    
    public static JsArrayLiteral ArrayLit() => new();
    public static JsArrayLiteral ArrayLit(params IAstNode[] nodes) => new(nodes);
    
    public static JsArrayLiteral ArrayLit(params int[] nodes) => new(nodes.Select(i => i.ToLit()));
    
    public static JsObjectLiteral ObjectLit() => new();
    public static JsObjectLiteral ObjectLit(params JsObjectLiteralProperty[] props) => new(props);
    
    public static AstValueLiteral<bool> True => new (true);
    public static AstValueLiteral<bool> False => new (false);

    public static AstGetThis This => new();
    
    public static IAstNode Nothing => AstEmptyNode.Instance;
    
    public static JsSwitchDefault Default => new();
    public static IAstNode NaN => new AstValueLiteral<double>(double.NaN);

    public static IAstNode Break => new AstBreak();
    public static IAstNode Continue => new AstContinue();
    
    public static JsFunctionDeclaration Func(IAstNode funcBody)
    {
        return new JsAnonymousFunctionDeclaration(
            Array.Empty<JsFunctionDeclarationParameter>(),funcBody);
    }
    
    public static JsFunctionDeclaration Func(string argName0, IAstNode funcBody)
    {
        return new JsAnonymousFunctionDeclaration(
            new []
            {
                new JsFunctionDeclarationParameter(argName0)
            },funcBody);
    }
    public static JsFunctionDeclaration Func(string argName0, string argName1, IAstNode funcBody)
    {
        return new JsAnonymousFunctionDeclaration(
            new []
            {
                new JsFunctionDeclarationParameter(argName0),
                new JsFunctionDeclarationParameter(argName1),
            },funcBody);
    }
    
    public static JsFunctionDeclaration Func(
        string argName0, string argName1, string argName2,
        IAstNode funcBody)
    {
        return new JsAnonymousFunctionDeclaration(
            new []
            {
                new JsFunctionDeclarationParameter(argName0),
                new JsFunctionDeclarationParameter(argName1),
                new JsFunctionDeclarationParameter(argName2),
            },funcBody);
    }
    
    public static JsFunctionDeclaration Func(string[] args, IAstNode funcBody)
    {
        return new JsAnonymousFunctionDeclaration(
            args.Select(s => new JsFunctionDeclarationParameter(s)).ToArray(),
            funcBody);
    }
    
    public static JsNamedFunctionDeclaration NamedFunc(string name, IAstNode funcBody)
    {
        return new JsNamedFunctionDeclaration(
            name,
            Array.Empty<JsFunctionDeclarationParameter>(),funcBody);
    }
    
    public static JsNamedFunctionDeclaration NamedFunc(string name, string argName0, IAstNode funcBody)
    {
        return new JsNamedFunctionDeclaration(
            name,
            new []
            {
                new JsFunctionDeclarationParameter(argName0)
            },funcBody);
    }
    public static JsNamedFunctionDeclaration NamedFunc(string name, string argName0, string argName1, IAstNode funcBody)
    {
        return new JsNamedFunctionDeclaration(
            name,
            new []
            {
                new JsFunctionDeclarationParameter(argName0),
                new JsFunctionDeclarationParameter(argName1),
            },funcBody);
    }
    
    public static JsNamedFunctionDeclaration NamedFunc(
        string name, string argName0, string argName1, string argName2,
        IAstNode funcBody)
    {
        return new JsNamedFunctionDeclaration(
            name,
            new []
            {
                new JsFunctionDeclarationParameter(argName0),
                new JsFunctionDeclarationParameter(argName1),
                new JsFunctionDeclarationParameter(argName2),
            },funcBody);
    }
    
    public static JsNamedFunctionDeclaration NamedFunc(string name, string[] args, IAstNode funcBody)
    {
        return new JsNamedFunctionDeclaration(
            name,
            args.Select(s => new JsFunctionDeclarationParameter(s)).ToArray(),
            funcBody);
    }
    
    #endregion
}