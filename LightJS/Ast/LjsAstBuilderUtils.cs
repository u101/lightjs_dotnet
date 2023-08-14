using LightJS.Tokenizer;

namespace LightJS.Ast;

public static class LjsAstBuilderUtils
{
    private const int GroupingOperatorsPriority = 18000;
    public const int FuncCallOperatorPriority = 17000;
    private const int PropertyAccessOperatorsPriority = 17500;
    private const int UnaryOperatorPriority = 14000;
    private const int HighCalculationPriority = 12000;
    private const int MiddleCalculationPriority = 11000;
    private const int BitwiseCalculationPriority = 10000;
    private const int CompareOperatorsPriority = 9000;
    private const int EqualityOperatorsPriority = 8000;
    
    private const int AssignmentOperatorsPriority = 2000;
    
    private static readonly Dictionary<LjsTokenType, int> OperatorsPriorityMap = new()
    {
        { LjsTokenType.OpParenthesesOpen, GroupingOperatorsPriority},
        { LjsTokenType.OpParenthesesClose, GroupingOperatorsPriority},
        
        { LjsTokenType.OpDot, PropertyAccessOperatorsPriority},
        { LjsTokenType.OpSquareBracketsOpen, PropertyAccessOperatorsPriority},
        
        { LjsTokenType.OpLogicalNot, UnaryOperatorPriority},
        { LjsTokenType.OpBitNot, UnaryOperatorPriority},
        { LjsTokenType.OpIncrement, UnaryOperatorPriority},
        { LjsTokenType.OpDecrement, UnaryOperatorPriority},
        
        { LjsTokenType.OpMultiply, HighCalculationPriority},
        { LjsTokenType.OpDiv, HighCalculationPriority},
        { LjsTokenType.OpModulo, HighCalculationPriority},
        
        { LjsTokenType.OpPlus, MiddleCalculationPriority},
        { LjsTokenType.OpMinus, MiddleCalculationPriority},
        
        { LjsTokenType.OpBitLeftShift, BitwiseCalculationPriority},
        { LjsTokenType.OpBitRightShift, BitwiseCalculationPriority},
        { LjsTokenType.OpBitUnsignedRightShift, BitwiseCalculationPriority},
        
        { LjsTokenType.OpLess, CompareOperatorsPriority},
        { LjsTokenType.OpLessOrEqual, CompareOperatorsPriority},
        { LjsTokenType.OpGreater, CompareOperatorsPriority},
        { LjsTokenType.OpGreaterOrEqual, CompareOperatorsPriority},
        
        { LjsTokenType.OpEquals, EqualityOperatorsPriority},
        { LjsTokenType.OpNotEqual, EqualityOperatorsPriority},
        { LjsTokenType.OpEqualsStrict, EqualityOperatorsPriority},
        { LjsTokenType.OpNotEqualStrict, EqualityOperatorsPriority},
        
        { LjsTokenType.OpBitAnd, 7000},
        // { LjsTokenType.OpBitXor, 6000}, // TODO
        { LjsTokenType.OpBitOr, 5000},
        
        { LjsTokenType.OpLogicalAnd, 4000},
        { LjsTokenType.OpLogicalOr, 3000},
        
    };
    
    public static int GetOperatorPriority(LjsTokenType tokenType, bool isUnary)
    {
        if (isUnary) 
            return UnaryOperatorPriority;
        
        if (IsAssignOperator(tokenType)) 
            return AssignmentOperatorsPriority;

        return OperatorsPriorityMap.TryGetValue(tokenType, out var priority) ? priority : 0;
    }

    private static readonly HashSet<LjsTokenType> LiteralTypesSet = new()
    {
        LjsTokenType.True,
        LjsTokenType.False,
        LjsTokenType.IntBinary,
        LjsTokenType.IntDecimal, 
        LjsTokenType.IntHex,
        LjsTokenType.Float, 
        LjsTokenType.FloatE, 
        LjsTokenType.Null,
        LjsTokenType.Undefined, 
        LjsTokenType.StringLiteral
    };

    private static readonly Dictionary<LjsTokenType, LjsAstAssignMode> AssignModes = new()
    {
        {LjsTokenType.OpAssign, LjsAstAssignMode.Normal},
        {LjsTokenType.OpPlusAssign, LjsAstAssignMode.PlusAssign},
        {LjsTokenType.OpMinusAssign, LjsAstAssignMode.MinusAssign},
        
        {LjsTokenType.OpMultAssign, LjsAstAssignMode.MulAssign},
        {LjsTokenType.OpDivAssign, LjsAstAssignMode.DivAssign},
        {LjsTokenType.OpBitOrAssign, LjsAstAssignMode.BitOrAssign},
        
        {LjsTokenType.OpBitAndAssign, LjsAstAssignMode.BitAndAssign},
        {LjsTokenType.OpLogicalOrAssign, LjsAstAssignMode.LogicalOrAssign},
        {LjsTokenType.OpLogicalAndAssign, LjsAstAssignMode.LogicalAndAssign},
    };

    private static readonly Dictionary<LjsTokenType, LjsAstBinaryOperationType> BinaryOperationsMap = new()
    {
        {LjsTokenType.OpPlus, LjsAstBinaryOperationType.Plus},
        {LjsTokenType.OpMinus, LjsAstBinaryOperationType.Minus},
        
        {LjsTokenType.OpGreater, LjsAstBinaryOperationType.Greater},
        {LjsTokenType.OpLess, LjsAstBinaryOperationType.Less},
        {LjsTokenType.OpMultiply, LjsAstBinaryOperationType.Multiply},
        {LjsTokenType.OpDiv, LjsAstBinaryOperationType.Div},
        {LjsTokenType.OpModulo, LjsAstBinaryOperationType.Modulo},
        {LjsTokenType.OpBitAnd, LjsAstBinaryOperationType.BitAnd},
        {LjsTokenType.OpBitOr, LjsAstBinaryOperationType.BitOr},
        {LjsTokenType.OpBitLeftShift, LjsAstBinaryOperationType.BitLeftShift},
        {LjsTokenType.OpBitRightShift, LjsAstBinaryOperationType.BitRightShift},
        {LjsTokenType.OpBitUnsignedRightShift, LjsAstBinaryOperationType.BitUnsignedRightShift},
        
        {LjsTokenType.OpEquals, LjsAstBinaryOperationType.Equals},
        {LjsTokenType.OpEqualsStrict, LjsAstBinaryOperationType.EqualsStrict},
        {LjsTokenType.OpGreaterOrEqual, LjsAstBinaryOperationType.GreaterOrEqual},
        {LjsTokenType.OpLessOrEqual, LjsAstBinaryOperationType.LessOrEqual},
        {LjsTokenType.OpNotEqual, LjsAstBinaryOperationType.NotEqual},
        {LjsTokenType.OpNotEqualStrict, LjsAstBinaryOperationType.NotEqualStrict},
        {LjsTokenType.OpLogicalAnd, LjsAstBinaryOperationType.LogicalAnd},
        {LjsTokenType.OpLogicalOr, LjsAstBinaryOperationType.LogicalOr},
    };

    private static readonly HashSet<LjsTokenType> BinaryOperatorsSet =
        BinaryOperationsMap.Keys.Concat(AssignModes.Keys).Concat(new [] { LjsTokenType.OpDot}).ToHashSet();

    private static readonly Dictionary<LjsTokenType, LjsAstUnaryOperationType> PrefixUnaryOperationsMap = new()
    {
        {LjsTokenType.OpPlus, LjsAstUnaryOperationType.Plus},
        {LjsTokenType.OpMinus, LjsAstUnaryOperationType.Minus},
            
        {LjsTokenType.OpLogicalNot, LjsAstUnaryOperationType.LogicalNot},
        {LjsTokenType.OpBitNot, LjsAstUnaryOperationType.BitNot},
        {LjsTokenType.OpIncrement, LjsAstUnaryOperationType.PrefixIncrement},
        {LjsTokenType.OpDecrement, LjsAstUnaryOperationType.PrefixDecrement},
    };

    public static bool CanBeUnaryPrefixOp(LjsTokenType tokenType) => 
        PrefixUnaryOperationsMap.ContainsKey(tokenType);
    
    public static bool CanBeUnaryPostfixOp(LjsTokenType tokenType)
    {
        return tokenType is 
            LjsTokenType.OpIncrement or 
            LjsTokenType.OpDecrement;
    }

    public static bool IsBinaryOp(LjsTokenType tokenType) => 
        BinaryOperatorsSet.Contains(tokenType);

    public static bool IsOrdinaryOperator(LjsTokenType tokenType) => 
        IsBinaryOp(tokenType) || CanBeUnaryPrefixOp(tokenType);

    

    public static LjsAstAssignMode GetAssignMode(LjsTokenType tokenType) =>
        AssignModes.TryGetValue(tokenType, out var mode)
            ? mode
            : throw new ArgumentException($"unsupported token type {tokenType}");

    public static bool IsAssignOperator(LjsTokenType tokenType) => 
        AssignModes.ContainsKey(tokenType);

    public static bool IsLiteral(LjsTokenType tokenType) => LiteralTypesSet.Contains(tokenType);

    public static LjsAstBinaryOperationType GetBinaryOperationType(LjsTokenType tokenType) =>
        BinaryOperationsMap.TryGetValue(tokenType, out var op)
            ? op
            : throw new ArgumentException($"unsupported binary operator token {tokenType}");

    public static LjsAstUnaryOperationType GetUnaryPrefixOperationType(LjsTokenType tokenType) =>
        PrefixUnaryOperationsMap.TryGetValue(tokenType, out var op) ? op
            : throw new ArgumentException($"unsupported unary operation token type {tokenType}");
    
    public static LjsAstUnaryOperationType GetUnaryPostfixOperationType(LjsTokenType tokenType) => tokenType switch
    {
        LjsTokenType.OpIncrement => LjsAstUnaryOperationType.PostfixIncrement,
        LjsTokenType.OpDecrement => LjsAstUnaryOperationType.PostfixDecrement,
        _ => throw new Exception($"unsupported unary operation token type {tokenType}")
    };

    public static LjsAstUnaryOperationType GetUnaryOperationType(LjsTokenType tokenType, bool isPrefix) =>
        isPrefix ? GetUnaryPrefixOperationType(tokenType) : GetUnaryPostfixOperationType(tokenType);

    public static ILjsAstNode CreateLiteralNode(LjsToken token, string sourceCodeString)
    {
        switch (token.TokenType)
        {
            case LjsTokenType.IntDecimal:
            case LjsTokenType.IntHex:
            case LjsTokenType.IntBinary:

                return new LjsAstLiteral<int>(
                    LjsTokenizerUtils.GetTokenIntValue(sourceCodeString, token));
                    
            case LjsTokenType.Float:
            case LjsTokenType.FloatE:

                return new LjsAstLiteral<double>(
                    LjsTokenizerUtils.GetTokenFloatValue(sourceCodeString, token));
                    
            case LjsTokenType.StringLiteral:
                return new LjsAstLiteral<string>(
                    sourceCodeString.Substring(token.Position.CharIndex, token.StringLength));
                    
            case LjsTokenType.True:
                        
                return new LjsAstLiteral<bool>(true);
                    
            case LjsTokenType.False:
                        
                return new LjsAstLiteral<bool>(false);
                    
            case LjsTokenType.Null:
                return new LjsAstNull();
                    
            case LjsTokenType.Undefined:
                return new LjsAstUndefined();
                    
                    
            default:
                throw new Exception($"unsupported literal token type {token.TokenType}");
        }
    }
    
}