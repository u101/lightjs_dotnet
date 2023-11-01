using App16.ALang.Ast;
using App16.ALang.Tests.DefaultLang;

namespace App16.ALang.Tests.Ast;

public static class AstNodesExtensions
{
    public static IAstNode Tif(
        this IAstNode condition, IAstNode trueExpression,
        IAstNode falseExpression) => new AstTernaryIfOperation(condition, trueExpression, falseExpression);
    
    public static IAstNode Tif(
        this string condition, IAstNode trueExpression,
        IAstNode falseExpression) => new AstTernaryIfOperation(condition.ToVar(), trueExpression, falseExpression);
    
    public static IAstNode Tif(
        this string condition, string trueExpression,
        string falseExpression) => new AstTernaryIfOperation(condition.ToVar(), trueExpression.ToVar(), falseExpression.ToVar());
    
    public static AstValueLiteral<int> ToLit(this int x)
    {
        return new AstValueLiteral<int>(x);
    }
    
    public static AstValueLiteral<string> ToLit(this string x)
    {
        return new AstValueLiteral<string>(x);
    }
    public static AstGetId ToVar(this string x)
    {
        return new AstGetId(x);
    }
    
    public static IAstNode Dot(this string x, string y) => new AstGetDotProperty(x.ToVar(), y);
    public static IAstNode Dot(this IAstNode x, string y) => new AstGetDotProperty(x, y);
    
    public static IAstNode Sqb(this string x, string y) => new AstGetSquareBracketsProp(x.ToVar(), y.ToVar());
    public static IAstNode Sqb(this IAstNode x, string y) => new AstGetSquareBracketsProp(x, y.ToVar());
    
    public static AstUnaryOperation WithUnaryMinus(this string x) => PrefixUnaryOp(x,DefUnaryOperationTypes.UnaryMinus);
    public static AstUnaryOperation WithUnaryMinus(this IAstNode x) => PrefixUnaryOp(x,DefUnaryOperationTypes.UnaryMinus);
    public static AstUnaryOperation WithUnaryPlus(this string x) => PrefixUnaryOp(x,DefUnaryOperationTypes.UnaryPlus);
    public static AstUnaryOperation WithUnaryPlus(this IAstNode x) => PrefixUnaryOp(x,DefUnaryOperationTypes.UnaryPlus);
    
    public static AstUnaryOperation WithUnaryNot(this string x) => PrefixUnaryOp(x,DefUnaryOperationTypes.LogicalNot);
    public static AstUnaryOperation WithUnaryNot(this IAstNode x) => PrefixUnaryOp(x,DefUnaryOperationTypes.LogicalNot);

    public static AstUnaryOperation WithPostfixIncrement(this string x) =>
        PostfixUnaryOp(x, DefUnaryOperationTypes.Increment);
    
    public static AstUnaryOperation WithPostfixIncrement(this IAstNode x) =>
        PostfixUnaryOp(x, DefUnaryOperationTypes.Increment);
    
    public static AstUnaryOperation WithPrefixIncrement(this string x) =>
        PrefixUnaryOp(x, DefUnaryOperationTypes.Increment);
    
    public static AstUnaryOperation WithPrefixIncrement(this IAstNode x) =>
        PrefixUnaryOp(x, DefUnaryOperationTypes.Increment);
    
    public static AstBinaryOperation Assign(this string x, IAstNode y) => BinOp(x,y,DefBinaryOperationTypes.Assign);
    public static AstBinaryOperation Assign(this IAstNode x, IAstNode y) => BinOp(x,y,DefBinaryOperationTypes.Assign);
    public static AstBinaryOperation PlusAssign(this string x, IAstNode y) => BinOp(x,y,DefBinaryOperationTypes.PlusAssign);
    public static AstBinaryOperation PlusAssign(this IAstNode x, IAstNode y) => BinOp(x,y,DefBinaryOperationTypes.PlusAssign);
    
    public static AstBinaryOperation Mul(this string x, IAstNode y) => BinOp(x,y,DefBinaryOperationTypes.Multiply);
    public static AstBinaryOperation Mul(this string x, string y) => BinOp(x,y,DefBinaryOperationTypes.Multiply);
    public static AstBinaryOperation Mul(this IAstNode x, string y) => BinOp(x,y,DefBinaryOperationTypes.Multiply);
    
    public static AstBinaryOperation Plus(this IAstNode x, IAstNode y) => BinOp(x,y,DefBinaryOperationTypes.Plus);
    
    public static AstBinaryOperation Plus(this string x, IAstNode y) => BinOp(x,y,DefBinaryOperationTypes.Plus);
    
    public static AstBinaryOperation Plus(this IAstNode x, string y) => BinOp(x,y,DefBinaryOperationTypes.Plus);
    
    public static AstBinaryOperation Plus(this string x, string y) => BinOp(x,y,DefBinaryOperationTypes.Plus);
    public static AstBinaryOperation Plus(this string x, int y) => BinOp(x,y,DefBinaryOperationTypes.Plus);
    
    public static AstBinaryOperation Minus(this IAstNode x, IAstNode y) => BinOp(x,y,DefBinaryOperationTypes.Minus);
    
    public static AstBinaryOperation Minus(this string x, IAstNode y) => BinOp(x,y,DefBinaryOperationTypes.Minus);
    
    public static AstBinaryOperation Minus(this IAstNode x, string y) => BinOp(x,y,DefBinaryOperationTypes.Minus);
    
    public static AstBinaryOperation Minus(this string x, string y) => BinOp(x,y,DefBinaryOperationTypes.Minus);
    public static AstBinaryOperation Minus(this string x, int y) => BinOp(x,y,DefBinaryOperationTypes.Minus);

    private static AstBinaryOperationInfo GetBinaryOperationInfo(int type) =>
        DefOperationInfos.BinaryOperationInfos.First(i => i.OperatorId == type);
    
    private static AstUnaryOperationInfo GetUnaryOperationInfo(int type) =>
        DefOperationInfos.UnaryOperationInfos.First(i => i.OperatorId == type);
    
    private static AstBinaryOperation BinOp(IAstNode x, IAstNode y, int operationType)
    {
        return new AstBinaryOperation(x, y, GetBinaryOperationInfo(operationType));
    }
    
    private static AstBinaryOperation BinOp(string x, IAstNode y, int operationType)
    {
        return new AstBinaryOperation(new AstGetId(x), y, GetBinaryOperationInfo(operationType));
    }
    
    private static AstBinaryOperation BinOp(IAstNode x, string y, int operationType)
    {
        return new AstBinaryOperation(x, new AstGetId(y), GetBinaryOperationInfo(operationType));
    }
    
    private static AstBinaryOperation BinOp(string x, string y, int operationType)
    {
        return new AstBinaryOperation(new AstGetId(x), new AstGetId(y), GetBinaryOperationInfo(operationType));
    }
    private static AstBinaryOperation BinOp(string x, int y, int operationType)
    {
        return new AstBinaryOperation(new AstGetId(x), new AstValueLiteral<int>(y), GetBinaryOperationInfo(operationType));
    }
    
    private static AstUnaryOperation PrefixUnaryOp(string x, int operationType)
    {
        return new AstUnaryOperation(new AstGetId(x), GetUnaryOperationInfo(operationType), true);
    }
    
    private static AstUnaryOperation PrefixUnaryOp(IAstNode x, int operationType)
    {
        return new AstUnaryOperation(x, GetUnaryOperationInfo(operationType), true);
    }
    
    private static AstUnaryOperation PostfixUnaryOp(string x, int operationType)
    {
        return new AstUnaryOperation(new AstGetId(x), GetUnaryOperationInfo(operationType), false);
    }
    
    private static AstUnaryOperation PostfixUnaryOp(IAstNode x, int operationType)
    {
        return new AstUnaryOperation(x, GetUnaryOperationInfo(operationType), false);
    }
}