using LightJS.Ast;

namespace LightJS.Test.Utils;

public static class AstNodesExtensions
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
    
    public static LjsAstUnaryOperation WithUnaryMinus(this ILjsAstNode x)
        => UnaryOp(x, LjsAstUnaryOperationType.Minus);
    
    public static LjsAstUnaryOperation WithUnaryMinus(this string x)
        => UnaryOp(x, LjsAstUnaryOperationType.Minus);
    
    public static LjsAstIncrementVar WithPrefixIncrement(this string x)
        => new(x, LjsAstIncrementSign.Plus, LjsAstIncrementOrder.Prefix);
    

    public static LjsAstIncrementVar WithPostfixIncrement(this string x)
        => new(x, LjsAstIncrementSign.Plus, LjsAstIncrementOrder.Postfix);
    
    public static LjsAstIncrementVar WithPrefixDecrement(this string x) =>
        new(x, LjsAstIncrementSign.Minus, LjsAstIncrementOrder.Prefix);
    
    public static LjsAstIncrementVar WithPostfixDecrement(this string x) =>
        new(x, LjsAstIncrementSign.Minus, LjsAstIncrementOrder.Postfix);

    public static LjsAstUnaryOperation WithUnaryPlus(this ILjsAstNode x) =>
        UnaryOp(x, LjsAstUnaryOperationType.Plus);
    
    public static LjsAstUnaryOperation WithUnaryPlus(this string x) =>
        UnaryOp(x, LjsAstUnaryOperationType.Plus);
    
    public static LjsAstBinaryOperation NotEq(this ILjsAstNode x, ILjsAstNode y) => BinOp(x,y,LjsAstBinaryOperationType.NotEqual);
    public static LjsAstBinaryOperation Eq(this ILjsAstNode x, ILjsAstNode y) => BinOp(x,y,LjsAstBinaryOperationType.Equals);

    public static LjsAstBinaryOperation Plus(this ILjsAstNode x, ILjsAstNode y) => BinOp(x,y,LjsAstBinaryOperationType.Plus);
    
    public static LjsAstBinaryOperation Plus(this string x, ILjsAstNode y) => BinOp(x,y,LjsAstBinaryOperationType.Plus);
    
    public static LjsAstBinaryOperation Plus(this ILjsAstNode x, string y) => BinOp(x,y,LjsAstBinaryOperationType.Plus);
    
    public static LjsAstBinaryOperation Plus(this string x, string y) => BinOp(x,y,LjsAstBinaryOperationType.Plus);
    public static LjsAstBinaryOperation Plus(this string x, int y) => BinOp(x,y,LjsAstBinaryOperationType.Plus);
    
    public static LjsAstBinaryOperation Minus(this ILjsAstNode x, ILjsAstNode y) => BinOp(x,y,LjsAstBinaryOperationType.Minus);
    
    public static LjsAstBinaryOperation Minus(this string x, ILjsAstNode y) => BinOp(x,y,LjsAstBinaryOperationType.Minus);
    
    public static LjsAstBinaryOperation Minus(this ILjsAstNode x, string y) => BinOp(x,y,LjsAstBinaryOperationType.Minus);
    
    public static LjsAstBinaryOperation Minus(this string x, string y) => BinOp(x,y,LjsAstBinaryOperationType.Minus);
    public static LjsAstBinaryOperation Minus(this string x, int y) => BinOp(x,y,LjsAstBinaryOperationType.Minus);
    
    public static LjsAstBinaryOperation Exponent(this string x, int y) => BinOp(x,y,LjsAstBinaryOperationType.Pow);
    public static LjsAstBinaryOperation Exponent(this string x, string y) => BinOp(x,y,LjsAstBinaryOperationType.Pow);
    
    public static LjsAstBinaryOperation LessThen(this ILjsAstNode x, ILjsAstNode y) => BinOp(x,y,LjsAstBinaryOperationType.Less);
    
    public static LjsAstBinaryOperation LessThen(this string x, ILjsAstNode y) => BinOp(x,y,LjsAstBinaryOperationType.Less);
    
    public static LjsAstBinaryOperation LessThen(this ILjsAstNode x, string y) => BinOp(x,y,LjsAstBinaryOperationType.Less);
    
    public static LjsAstBinaryOperation LessThen(this string x, string y) => BinOp(x,y,LjsAstBinaryOperationType.Less);
    public static LjsAstBinaryOperation LessThen(this string x, int y) => BinOp(x,y,LjsAstBinaryOperationType.Less);
    
    
    
    public static LjsAstBinaryOperation MultiplyBy(this ILjsAstNode x, ILjsAstNode y)
    {
        return new LjsAstBinaryOperation(x, y, LjsAstBinaryOperationType.Multiply);
    }
    
    public static LjsAstBinaryOperation MultiplyBy(this ILjsAstNode x, string y)
    {
        return new LjsAstBinaryOperation(x, new LjsAstGetVar(y), LjsAstBinaryOperationType.Multiply);
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
        => new LjsAstFunctionCall(new LjsAstGetVar(funcName), new LjsAstFunctionCallArguments(args));
    
    public static ILjsAstNode FuncCall(this ILjsAstNode func, params ILjsAstNode[] args)
        => new LjsAstFunctionCall(func, new LjsAstFunctionCallArguments(args));

    public static ILjsAstNode TernaryIf(
        this ILjsAstNode condition, ILjsAstNode trueExpression,
        ILjsAstNode falseExpression)
    {
        return new LjsAstTernaryIfOperation(condition, trueExpression, falseExpression);
    }

    public static LjsAstIfBlock ElseIf(this LjsAstIfBlock ifBlock, ILjsAstNode condition, ILjsAstNode expression)
    {
        var e = new LjsAstConditionalExpression(condition, expression);
        ifBlock.ConditionalAlternatives.Add(e);
        return ifBlock;
    }

    public static LjsAstIfBlock Else(this LjsAstIfBlock ifBlock, ILjsAstNode expression)
    {
        ifBlock.ElseBlock = expression;
        return ifBlock;
    }

    public static LjsAstObjectLiteral AddProp(this LjsAstObjectLiteral obj, string name, ILjsAstNode value)
    {
        obj.AddNode(new LjsAstObjectLiteralProperty(name, value));
        return obj;
    }
    
    public static LjsAstObjectLiteral AddProp(this LjsAstObjectLiteral obj, string name, int value)
    {
        obj.AddNode(new LjsAstObjectLiteralProperty(name, value.ToLit()));
        return obj;
    }
    
    public static LjsAstObjectLiteral AddProp(this LjsAstObjectLiteral obj, string name, string value)
    {
        obj.AddNode(new LjsAstObjectLiteralProperty(name, value.ToLit()));
        return obj;
    }
    
    private static LjsAstBinaryOperation BinOp(ILjsAstNode x, ILjsAstNode y, LjsAstBinaryOperationType operationType)
    {
        return new LjsAstBinaryOperation(x, y, operationType);
    }
    
    private static LjsAstBinaryOperation BinOp(string x, ILjsAstNode y, LjsAstBinaryOperationType operationType)
    {
        return new LjsAstBinaryOperation(new LjsAstGetVar(x), y, operationType);
    }
    
    private static LjsAstBinaryOperation BinOp(ILjsAstNode x, string y, LjsAstBinaryOperationType operationType)
    {
        return new LjsAstBinaryOperation(x, new LjsAstGetVar(y), operationType);
    }
    
    private static LjsAstBinaryOperation BinOp(string x, string y, LjsAstBinaryOperationType operationType)
    {
        return new LjsAstBinaryOperation(new LjsAstGetVar(x), new LjsAstGetVar(y), operationType);
    }
    private static LjsAstBinaryOperation BinOp(string x, int y, LjsAstBinaryOperationType operationType)
    {
        return new LjsAstBinaryOperation(new LjsAstGetVar(x), new LjsAstLiteral<int>(y), operationType);
    }

    private static LjsAstUnaryOperation UnaryOp(ILjsAstNode x, LjsAstUnaryOperationType operationType)
    {
        return new LjsAstUnaryOperation(x, operationType);
    }
    
    private static LjsAstUnaryOperation UnaryOp(string x, LjsAstUnaryOperationType operationType)
    {
        return new LjsAstUnaryOperation(new LjsAstGetVar(x), operationType);
    }

}