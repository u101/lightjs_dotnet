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

    public static LjsAstReturn Return(ILjsAstNode returnValue) => new LjsAstReturn(returnValue);
    public static LjsAstReturn Return() => new LjsAstReturn();
    
}