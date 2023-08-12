using LightJS.Tokenizer;

namespace LightJS.Outsource;

public static class MatherExt
{

    public static MatherLiteralNode ToLit(this int x)
    {
        return new MatherLiteralNode(x.ToString());
    }
    
    public static MatherLiteralNode ToLit(this string x)
    {
        return new MatherLiteralNode(x);
    }
    public static MatherGetVarNode ToVar(this string x)
    {
        return new MatherGetVarNode(x);
    }
    
    public static MatherUnaryOpNode WithUnaryMinus(this IMatherNode node)
    {
        return new MatherUnaryOpNode(node, LjsTokenType.OpMinus);
    }
    
    public static MatherUnaryOpNode WithIncrement(this IMatherNode node)
    {
        return new MatherUnaryOpNode(node, LjsTokenType.OpIncrement);
    }
    
    public static MatherUnaryOpNode WithUnaryPlus(this IMatherNode node)
    {
        return new MatherUnaryOpNode(node, LjsTokenType.OpPlus);
    }

    public static MatherBinaryOpNode Plus(this IMatherNode node, IMatherNode other)
    {
        return new MatherBinaryOpNode(node, other, LjsTokenType.OpPlus);
    }
    
    public static MatherBinaryOpNode Minus(this IMatherNode node, IMatherNode other)
    {
        return new MatherBinaryOpNode(node, other, LjsTokenType.OpMinus);
    }
    
    public static MatherBinaryOpNode Assign(this IMatherNode node, IMatherNode other)
    {
        return new MatherBinaryOpNode(node, other, LjsTokenType.OpAssign);
    }
    
}