using FluentAssertions;
using LightJS.Ast;
using LightJS.Test.Ast;

namespace LightJS.Test.Utils;

public static class AstTestUtils
{
    public static ILjsAstNode BuildAstNode(string sourceCode)
    {
        var builder = new LjsAstBuilder(sourceCode);
        var model = builder.Build();
        return model.RootNode;
    }
    
    public static LjsAstSequence Sequence(params ILjsAstNode[] nodes)
    {
        return new LjsAstSequence(nodes);
    }

    public static LjsAstWhileLoop While(ILjsAstNode condition, ILjsAstNode expression)
    {
        return new LjsAstWhileLoop(condition, expression);
    }

    public static LjsAstForLoop For(ILjsAstNode init, ILjsAstNode cond, ILjsAstNode iter, ILjsAstNode body)
    {
        return new LjsAstForLoop(init, cond, iter, body);
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
        return new LjsAstAnonymousFunctionDeclaration(
            Array.Empty<LjsAstFunctionDeclarationParameter>(),funcBody);
    }
    
    public static LjsAstFunctionDeclaration Func(string argName0, ILjsAstNode funcBody)
    {
        return new LjsAstAnonymousFunctionDeclaration(
            new []
            {
                new LjsAstFunctionDeclarationParameter(argName0)
            },funcBody);
    }
    public static LjsAstFunctionDeclaration Func(string argName0, string argName1, ILjsAstNode funcBody)
    {
        return new LjsAstAnonymousFunctionDeclaration(
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
        return new LjsAstAnonymousFunctionDeclaration(
            new []
            {
                new LjsAstFunctionDeclarationParameter(argName0),
                new LjsAstFunctionDeclarationParameter(argName1),
                new LjsAstFunctionDeclarationParameter(argName2),
            },funcBody);
    }
    
    public static LjsAstFunctionDeclaration Func(string[] args, ILjsAstNode funcBody)
    {
        return new LjsAstAnonymousFunctionDeclaration(
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

    public static LjsAstVariableDeclaration Var(string name) => new(name, LjsAstVariableKind.Var);
    public static LjsAstVariableDeclaration Var(string name, ILjsAstNode value) => new(name, value, LjsAstVariableKind.Var);
    
    public static LjsAstVariableDeclaration Let(string name) => new(name, LjsAstVariableKind.Let);
    public static LjsAstVariableDeclaration Let(string name, ILjsAstNode value) => new(name, value, LjsAstVariableKind.Let);
    
    public static LjsAstVariableDeclaration Const(string name) => new(name, LjsAstVariableKind.Const);
    public static LjsAstVariableDeclaration Const(string name, ILjsAstNode value) => new(name, value, LjsAstVariableKind.Const);

    public static LjsAstArrayLiteral ArrayLit() => new();
    public static LjsAstArrayLiteral ArrayLit(params ILjsAstNode[] nodes) => new(nodes);
    
    public static LjsAstArrayLiteral ArrayLit(params int[] nodes) => new(nodes.Select(i => i.ToLit()));
    
    public static LjsAstObjectLiteral ObjectLit() => new();
    public static LjsAstObjectLiteral ObjectLit(params LjsAstObjectLiteralProperty[] props) => new(props);
    
    public static LjsAstObjectLiteral ObjectLit(string firstPropName, ILjsAstNode val) => 
        new(new []{new LjsAstObjectLiteralProperty(firstPropName, val)});

    public static LjsAstLiteral<bool> True => new (true);
    public static LjsAstLiteral<bool> False => new (false);

    public static LjsAstGetThis This => new();
    
    public static ILjsAstNode Nothing => LjsAstEmptyNode.Instance;

    public static ILjsAstNode Break => new LjsAstBreak();
    public static ILjsAstNode Continue => new LjsAstContinue();
}