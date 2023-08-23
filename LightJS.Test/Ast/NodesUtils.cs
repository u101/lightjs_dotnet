using FluentAssertions;
using LightJS.Ast;

namespace LightJS.Test.Ast;

public static class NodesUtils
{
    public static LjsAstSequence Sequence(params ILjsAstNode[] nodes)
    {
        return new LjsAstSequence(nodes);
    }

    public static LjsAstIfBlock IfBlock(ILjsAstNode condition, ILjsAstNode expression)
    {
        var e = new LjsAstConditionalExpression(condition, expression);
        return new LjsAstIfBlock(e);
    }

    public static void Match(ILjsAstNode node, ILjsAstNode expected)
    {
        node.Should().BeEquivalentTo(expected, options => options.RespectingRuntimeTypes());
    }

    public static LjsAstFunctionDeclaration Func(ILjsAstNode funcBody)
    {
        return new LjsAstFunctionDeclaration(
            Array.Empty<LjsAstFunctionDeclarationParameter>(),funcBody);
    }
    
    public static LjsAstFunctionDeclaration Func(string argName0, ILjsAstNode funcBody)
    {
        return new LjsAstFunctionDeclaration(
            new []
            {
                new LjsAstFunctionDeclarationParameter(argName0)
            },funcBody);
    }
    public static LjsAstFunctionDeclaration Func(string argName0, string argName1, ILjsAstNode funcBody)
    {
        return new LjsAstFunctionDeclaration(
            new []
            {
                new LjsAstFunctionDeclarationParameter(argName0),
                new LjsAstFunctionDeclarationParameter(argName1),
            },funcBody);
    }
    
    public static LjsAstFunctionDeclaration Func(
        string argName0, string argName1, string argName2,
        ILjsAstNode funcBody)
    {
        return new LjsAstFunctionDeclaration(
            new []
            {
                new LjsAstFunctionDeclarationParameter(argName0),
                new LjsAstFunctionDeclarationParameter(argName1),
                new LjsAstFunctionDeclarationParameter(argName2),
            },funcBody);
    }
    
    public static LjsAstFunctionDeclaration Func(string[] args, ILjsAstNode funcBody)
    {
        return new LjsAstFunctionDeclaration(
            args.Select(s => new LjsAstFunctionDeclarationParameter(s)).ToArray(),
            funcBody);
    }
    
    public static LjsAstNamedFunctionDeclaration NamedFunc(string name, ILjsAstNode funcBody)
    {
        return new LjsAstNamedFunctionDeclaration(
            name,
            Array.Empty<LjsAstFunctionDeclarationParameter>(),funcBody);
    }
    
    public static LjsAstNamedFunctionDeclaration NamedFunc(string name, string argName0, ILjsAstNode funcBody)
    {
        return new LjsAstNamedFunctionDeclaration(
            name,
            new []
            {
                new LjsAstFunctionDeclarationParameter(argName0)
            },funcBody);
    }
    public static LjsAstNamedFunctionDeclaration NamedFunc(string name, string argName0, string argName1, ILjsAstNode funcBody)
    {
        return new LjsAstNamedFunctionDeclaration(
            name,
            new []
            {
                new LjsAstFunctionDeclarationParameter(argName0),
                new LjsAstFunctionDeclarationParameter(argName1),
            },funcBody);
    }
    
    public static LjsAstNamedFunctionDeclaration NamedFunc(
        string name, string argName0, string argName1, string argName2,
        ILjsAstNode funcBody)
    {
        return new LjsAstNamedFunctionDeclaration(
            name,
            new []
            {
                new LjsAstFunctionDeclarationParameter(argName0),
                new LjsAstFunctionDeclarationParameter(argName1),
                new LjsAstFunctionDeclarationParameter(argName2),
            },funcBody);
    }
    
    public static LjsAstNamedFunctionDeclaration NamedFunc(string name, string[] args, ILjsAstNode funcBody)
    {
        return new LjsAstNamedFunctionDeclaration(
            name,
            args.Select(s => new LjsAstFunctionDeclarationParameter(s)).ToArray(),
            funcBody);
    }

    public static LjsAstReturn Return(ILjsAstNode returnValue) => new(returnValue);
    public static LjsAstReturn Return() => new();

    public static LjsAstVariableDeclaration Var(string name) => new(name, true);
    public static LjsAstVariableDeclaration Var(string name, ILjsAstNode value) => new(name, value, true);
    
    public static LjsAstVariableDeclaration Const(string name) => new(name, false);
    public static LjsAstVariableDeclaration Const(string name, ILjsAstNode value) => new(name, value, false);

    public static LjsAstArrayLiteral ArrayLit() => new();
    public static LjsAstArrayLiteral ArrayLit(params ILjsAstNode[] nodes) => new(nodes);
    
    public static LjsAstArrayLiteral ArrayLit(params int[] nodes) => new(nodes.Select(i => i.ToLit()));
    
    public static LjsAstObjectLiteral ObjectLit() => new();
    public static LjsAstObjectLiteral ObjectLit(params LjsAstObjectLiteralProperty[] props) => new(props);
    
    public static LjsAstObjectLiteral ObjectLit(string firstPropName, ILjsAstNode val) => 
        new(new []{new LjsAstObjectLiteralProperty(firstPropName, val)});

    public static LjsAstLiteral<bool> True => new (true);
    public static LjsAstLiteral<bool> False => new (false);

}