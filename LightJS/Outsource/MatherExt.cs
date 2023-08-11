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
    
}