namespace App16.ALang.Tests.DefaultLang;

public static class DefTokenTypes
{
    public const int None = 0;
    
    public const int Identifier = 50;

    #region ValueLiterals

    public const int True = 100;
    public const int False = 101;

    public const int IntDecimal = 200;
    public const int IntHex = 201;
    public const int IntBinary = 202;

    public const int Float = 300;
    public const int FloatE = 301;
    public const int FloatNaN = 302;

    public const int StringLiteral = 400;
    
    public const int Null = 500;
    public const int Undefined = 600;

    #endregion

    #region BinaryOperators

    public const int OpExponent = 1000;
    public const int OpGreater = 1001;
    public const int OpLess = 1002;
    public const int OpPlus = 1003;
    public const int OpMinus = 1004;
    public const int OpMultiply = 1005;
    public const int OpDiv = 1006;
    public const int OpModulo = 1007;
    public const int OpBitAnd = 1008;
    public const int OpBitOr = 1009;
    public const int OpBitXor = 1013;
    public const int OpBitLeftShift = 1010;
    public const int OpBitRightShift = 1011;
    public const int OpBitUnsignedRightShift = 1012;

    public const int OpEquals = 1400;
    public const int OpEqualsStrict = 1401;
    public const int OpGreaterOrEqual = 1402;
    public const int OpLessOrEqual = 1403;
    public const int OpNotEqual = 1404;
    public const int OpNotEqualStrict = 1405;
    public const int OpLogicalAnd = 1406;
    public const int OpLogicalOr = 1407;

    public const int OpAssign = 1500;
    public const int OpPlusAssign = 1501;
    public const int OpMinusAssign = 1502;
    public const int OpMultAssign = 1503;
    public const int OpDivAssign = 1504;
    public const int OpBitOrAssign = 1505;
    public const int OpBitAndAssign = 1506;
    public const int OpLogicalOrAssign = 1507;
    public const int OpLogicalAndAssign = 1508;

    #endregion

    #region UnaryOperators

    public const int OpLogicalNot = 1600;
    public const int OpBitNot = 1601;
    public const int OpIncrement = 1602;
    public const int OpDecrement = 1603;

    #endregion

    #region OtherOperators

    public const int OpQuestionMark = 1700;

    public const int OpComma = 1800;
    public const int OpDot = 1801;
    public const int OpColon = 1802;
    public const int OpSemicolon = 1803;

    public const int OpBracketOpen = 1900;
    public const int OpBracketClose = 1901;
    public const int OpParenthesesOpen = 1902;
    public const int OpParenthesesClose = 1903;
    public const int OpSquareBracketsOpen = 1904;
    public const int OpSquareBracketsClose = 1905;

    public const int OpArrow = 2000;

    #endregion
    
    
    
    public const int This = 2100;

    public const int Var = 2200;
    public const int Const = 2201;
    public const int Let = 2202;
    
    public const int Function = 2300;

    public const int Return = 2400;
    public const int Break = 2401;
    public const int Continue = 2402;

    public const int If = 2500;
    public const int ElseIf = 2501;
    public const int Else = 2502;

    public const int Switch = 2503;
    public const int Case = 2504;
    public const int Default = 2505;

    public const int While = 2600;
    public const int Do = 2601;

    public const int For = 2602;
    public const int In = 2700;
}