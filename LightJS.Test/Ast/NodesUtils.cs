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
}