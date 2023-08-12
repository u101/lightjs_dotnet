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
}