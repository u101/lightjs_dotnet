namespace App16.ALang.Js.Ast;

public static class JsOperationPriorityGroups
{
    /// <summary>
    /// Mul,Div,Modulo 
    /// </summary>
    public const int Multiplication = 150; 
    
    /// <summary>
    /// Plus, Minus
    /// </summary>
    public const int Addition = 140;

    /// <summary>
    /// BitLeftShift, BitRightShift, BitUnsignedRightShift
    /// </summary>
    public const int BitShift = 130;
    
    /// <summary>
    /// Greater, GreaterOrEqual, Less, LessOrEqual
    /// </summary>
    public const int Compare = 120;
    
    /// <summary>
    /// Equals, NotEqual
    /// </summary>
    public const int EqualityCheck = 110;
    
    public const int BitAnd = 100;
    public const int BitXor = 90;
    public const int BitOr = 80;
    public const int LogicalAnd = 70;
    public const int LogicalOr = 60;

    public const int TernaryIf = 30;
    
    public const int Assign = 20;

}