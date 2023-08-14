using LightJS.Ast;

namespace LightJS.Test;

public static class LsjAstNodesExt
{
    public static LjsAstLiteral<int> ToLit(this int x)
    {
        return new LjsAstLiteral<int>(x);
    }
    
    public static LjsAstLiteral<string> ToLit(this string x)
    {
        return new LjsAstLiteral<string>(x);
    }
    public static LjsAstGetVar ToVar(this string x)
    {
        return new LjsAstGetVar(x);
    }
    
    public static LjsAstUnaryOperation WithUnaryMinus(this ILjsAstNode node)
    {
        return new LjsAstUnaryOperation(node, LjsAstUnaryOperationType.Minus);
    }
    
    public static LjsAstUnaryOperation WithUnaryMinus(this string x)
    {
        return new LjsAstUnaryOperation(new LjsAstGetVar(x), LjsAstUnaryOperationType.Minus);
    }
    
    public static LjsAstUnaryOperation WithPrefixIncrement(this ILjsAstNode node)
    {
        return new LjsAstUnaryOperation(node, LjsAstUnaryOperationType.PrefixIncrement);
    }
    public static LjsAstUnaryOperation WithPostfixIncrement(this ILjsAstNode node)
    {
        return new LjsAstUnaryOperation(node, LjsAstUnaryOperationType.PostfixIncrement);
    }
    
    public static LjsAstUnaryOperation WithPrefixDecrement(this ILjsAstNode node)
    {
        return new LjsAstUnaryOperation(node, LjsAstUnaryOperationType.PrefixDecrement);
    }
    
    public static LjsAstUnaryOperation WithPostfixDecrement(this ILjsAstNode node)
    {
        return new LjsAstUnaryOperation(node, LjsAstUnaryOperationType.PostfixDecrement);
    }
    
    public static LjsAstUnaryOperation WithUnaryPlus(this ILjsAstNode node)
    {
        return new LjsAstUnaryOperation(node, LjsAstUnaryOperationType.Plus);
    }
    
    public static LjsAstUnaryOperation WithUnaryPlus(this string x)
    {
        return new LjsAstUnaryOperation(new LjsAstGetVar(x), LjsAstUnaryOperationType.Plus);
    }

    public static LjsAstBinaryOperation Plus(this ILjsAstNode node, ILjsAstNode other)
    {
        return new LjsAstBinaryOperation(node, other, LjsAstBinaryOperationType.Plus);
    }
    
    public static LjsAstBinaryOperation Plus(this string x, ILjsAstNode other)
    {
        return new LjsAstBinaryOperation(new LjsAstGetVar(x), other, LjsAstBinaryOperationType.Plus);
    }
    
    public static LjsAstBinaryOperation Plus(this ILjsAstNode x, string y)
    {
        return new LjsAstBinaryOperation(x, new LjsAstGetVar(y), LjsAstBinaryOperationType.Plus);
    }
    
    public static LjsAstBinaryOperation Plus(this string x, string y)
    {
        return new LjsAstBinaryOperation(new LjsAstGetVar(x), new LjsAstGetVar(y), LjsAstBinaryOperationType.Plus);
    }
    public static LjsAstBinaryOperation Plus(this string x, int y)
    {
        return new LjsAstBinaryOperation(new LjsAstGetVar(x), new LjsAstLiteral<int>(y), LjsAstBinaryOperationType.Plus);
    }
    
    public static LjsAstBinaryOperation Minus(this ILjsAstNode x, ILjsAstNode y)
    {
        return new LjsAstBinaryOperation(x, y, LjsAstBinaryOperationType.Minus);
    }
    
    public static LjsAstBinaryOperation Minus(this string x, ILjsAstNode y)
    {
        return new LjsAstBinaryOperation(new LjsAstGetVar(x), y, LjsAstBinaryOperationType.Minus);
    }
    
    public static LjsAstBinaryOperation Minus(this ILjsAstNode x, string y)
    {
        return new LjsAstBinaryOperation(x, new LjsAstGetVar(y), LjsAstBinaryOperationType.Minus);
    }
    
    public static LjsAstBinaryOperation Minus(this string x, string y)
    {
        return new LjsAstBinaryOperation(new LjsAstGetVar(x), new LjsAstGetVar(y), LjsAstBinaryOperationType.Minus);
    }
    
    public static LjsAstSetVar Assign(this string varName, ILjsAstNode other)
    {
        return new LjsAstSetVar(varName, other, LjsAstAssignMode.Normal);
    }

    public static ILjsAstNode GetProp(this string varName, string prop)
    {
        return new LjsAstGetNamedProperty(prop, new LjsAstGetVar(varName));
    }

    public static ILjsAstNode GetProp(this ILjsAstNode node, string prop)
    {
        return new LjsAstGetNamedProperty(prop, node);
    }
    
    public static ILjsAstNode GetProp(this string varName, ILjsAstNode prop)
    {
        return new LjsAstGetProperty(prop, new LjsAstGetVar(varName));
    }

    public static ILjsAstNode GetProp(this ILjsAstNode node, ILjsAstNode prop)
    {
        return new LjsAstGetProperty(prop, node);
    }

    public static ILjsAstNode SetProp(this ILjsAstNode node, string propName, ILjsAstNode v)
    {
        return new LjsAstSetNamedProperty(propName, node, v, LjsAstAssignMode.Normal);
    }
    
    public static ILjsAstNode SetProp(this string varName, string propName, ILjsAstNode v)
    {
        return new LjsAstSetNamedProperty(propName, new LjsAstGetVar(varName), v, LjsAstAssignMode.Normal);
    }
    
    public static ILjsAstNode SetProp(this string varName, ILjsAstNode propName, ILjsAstNode v)
    {
        return new LjsAstSetProperty(propName, new LjsAstGetVar(varName), v, LjsAstAssignMode.Normal);
    }

    public static ILjsAstNode FuncCall(this string funcName, params ILjsAstNode[] args)
    {
        var f = new LjsAstFunctionCall(new LjsAstGetVar(funcName));
        f.Arguments.AddRange(args);
        return f;
    }
    
    public static ILjsAstNode FuncCall(this ILjsAstNode func, params ILjsAstNode[] args)
    {
        var f = new LjsAstFunctionCall(func);
        f.Arguments.AddRange(args);
        return f;
    }
    
}