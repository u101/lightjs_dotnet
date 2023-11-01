using App16.ALang.Ast;
using App16.ALang.Js.Ast;

namespace App16.ALang.Js.Tests.Ast;

public static class AstNodesExtensions
{
    public static IAstNode FuncCall(this string funcName, params IAstNode[] args)
        => new JsFunctionCall(new AstGetId(funcName), new JsFunctionCallArguments(args));
    
    public static IAstNode FuncCall(this IAstNode func, params IAstNode[] args)
        => new JsFunctionCall(func, new JsFunctionCallArguments(args));
    
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
    
    public static IAstNode Sqb(this string x, IAstNode y) => new AstGetSquareBracketsProp(x.ToVar(), y);
    public static IAstNode Sqb(this IAstNode x, IAstNode y) => new AstGetSquareBracketsProp(x, y);
    
    public static AstUnaryOperation WithUnaryMinus(this string x) => PrefixUnaryOp(x,JsUnaryOperationTypes.UnaryMinus);
    public static AstUnaryOperation WithUnaryMinus(this IAstNode x) => PrefixUnaryOp(x,JsUnaryOperationTypes.UnaryMinus);
    public static AstUnaryOperation WithUnaryPlus(this string x) => PrefixUnaryOp(x,JsUnaryOperationTypes.UnaryPlus);
    public static AstUnaryOperation WithUnaryPlus(this IAstNode x) => PrefixUnaryOp(x,JsUnaryOperationTypes.UnaryPlus);
    
    public static AstUnaryOperation WithUnaryNot(this string x) => PrefixUnaryOp(x,JsUnaryOperationTypes.LogicalNot);
    public static AstUnaryOperation WithUnaryNot(this IAstNode x) => PrefixUnaryOp(x,JsUnaryOperationTypes.LogicalNot);

    public static AstUnaryOperation WithPostfixIncrement(this string x) =>
        PostfixUnaryOp(x, JsUnaryOperationTypes.Increment);
    
    public static AstUnaryOperation WithPostfixDecrement(this string x) =>
        PostfixUnaryOp(x, JsUnaryOperationTypes.Decrement);
    
    public static AstUnaryOperation WithPostfixIncrement(this IAstNode x) =>
        PostfixUnaryOp(x, JsUnaryOperationTypes.Increment);
    
    public static AstUnaryOperation WithPrefixIncrement(this string x) =>
        PrefixUnaryOp(x, JsUnaryOperationTypes.Increment);
    
    public static AstUnaryOperation WithPrefixDecrement(this string x) =>
        PrefixUnaryOp(x, JsUnaryOperationTypes.Decrement);
    
    public static AstUnaryOperation WithPrefixIncrement(this IAstNode x) =>
        PrefixUnaryOp(x, JsUnaryOperationTypes.Increment);
    
    public static AstBinaryOperation NotEq(this IAstNode x, IAstNode y) => BinOp(x,y,JsBinaryOperationTypes.NotEqual);
    public static AstBinaryOperation Eq(this IAstNode x, IAstNode y) => BinOp(x,y,JsBinaryOperationTypes.Equals);
    
    public static AstBinaryOperation Exp(this string x, string y) => BinOp(x,y,JsBinaryOperationTypes.Pow);
    
    public static AstBinaryOperation Assign(this string x, IAstNode y) => BinOp(x,y,JsBinaryOperationTypes.Assign);
    public static AstBinaryOperation Assign(this IAstNode x, IAstNode y) => BinOp(x,y,JsBinaryOperationTypes.Assign);
    public static AstBinaryOperation PlusAssign(this string x, IAstNode y) => BinOp(x,y,JsBinaryOperationTypes.PlusAssign);
    public static AstBinaryOperation PlusAssign(this IAstNode x, IAstNode y) => BinOp(x,y,JsBinaryOperationTypes.PlusAssign);
    
    public static AstBinaryOperation Mul(this string x, IAstNode y) => BinOp(x,y,JsBinaryOperationTypes.Multiply);
    public static AstBinaryOperation Mul(this string x, string y) => BinOp(x,y,JsBinaryOperationTypes.Multiply);
    public static AstBinaryOperation Mul(this IAstNode x, string y) => BinOp(x,y,JsBinaryOperationTypes.Multiply);
    
    public static AstBinaryOperation Plus(this IAstNode x, IAstNode y) => BinOp(x,y,JsBinaryOperationTypes.Plus);
    
    public static AstBinaryOperation Plus(this string x, IAstNode y) => BinOp(x,y,JsBinaryOperationTypes.Plus);
    
    public static AstBinaryOperation Plus(this IAstNode x, string y) => BinOp(x,y,JsBinaryOperationTypes.Plus);
    
    public static AstBinaryOperation Plus(this string x, string y) => BinOp(x,y,JsBinaryOperationTypes.Plus);
    public static AstBinaryOperation Plus(this string x, int y) => BinOp(x,y,JsBinaryOperationTypes.Plus);
    
    public static AstBinaryOperation Minus(this IAstNode x, IAstNode y) => BinOp(x,y,JsBinaryOperationTypes.Minus);
    
    public static AstBinaryOperation Minus(this string x, IAstNode y) => BinOp(x,y,JsBinaryOperationTypes.Minus);
    
    public static AstBinaryOperation Minus(this IAstNode x, string y) => BinOp(x,y,JsBinaryOperationTypes.Minus);
    
    public static AstBinaryOperation Minus(this string x, string y) => BinOp(x,y,JsBinaryOperationTypes.Minus);
    public static AstBinaryOperation Minus(this string x, int y) => BinOp(x,y,JsBinaryOperationTypes.Minus);
    
    public static AstBinaryOperation LessThen(this string x, string y) => BinOp(x,y,JsBinaryOperationTypes.Less);

    private static AstBinaryOperationInfo GetBinaryOperationInfo(int type) =>
        JsOperationInfos.BinaryOperationInfos.First(i => i.OperatorId == type);
    
    private static AstUnaryOperationInfo GetUnaryOperationInfo(int type) =>
        JsOperationInfos.UnaryOperationInfos.First(i => i.OperatorId == type);
    
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
    
    public static AstIfBlock ElseIf(this AstIfBlock ifBlock, IAstNode condition, IAstNode expression)
    {
        var e = new AstConditionalExpression(condition, expression);
        ifBlock.ElseIfs.Add(e);
        return ifBlock;
    }

    public static AstIfBlock Else(this AstIfBlock ifBlock, IAstNode expression)
    {
        ifBlock.Else = expression;
        return ifBlock;
    }
    
    public static JsObjectLiteral AddProp(this JsObjectLiteral obj, string name, IAstNode value)
    {
        obj.AddNode(new JsObjectLiteralProperty(name, value));
        return obj;
    }
    
    public static JsObjectLiteral AddProp(this JsObjectLiteral obj, string name, int value)
    {
        obj.AddNode(new JsObjectLiteralProperty(name, value.ToLit()));
        return obj;
    }
    
    public static JsObjectLiteral AddProp(this JsObjectLiteral obj, string name, string value)
    {
        obj.AddNode(new JsObjectLiteralProperty(name, value.ToLit()));
        return obj;
    }
}