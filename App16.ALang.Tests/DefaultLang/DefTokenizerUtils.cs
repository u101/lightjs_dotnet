using App16.ALang.Tokenizers;

namespace App16.ALang.Tests.DefaultLang;

public static class DefTokenizerUtils
{
    public static Dictionary<char, int> CreateOperatorsMap() => new()
    {
        { '>', DefTokenTypes.OpGreater },
        { '<', DefTokenTypes.OpLess },
        { '=', DefTokenTypes.OpAssign },
        { '+', DefTokenTypes.OpPlus },
        { '-', DefTokenTypes.OpMinus },
        { '*', DefTokenTypes.OpMultiply },
        { '/', DefTokenTypes.OpDiv },
        { '%', DefTokenTypes.OpModulo },
        { '&', DefTokenTypes.OpBitAnd },
        { '|', DefTokenTypes.OpBitOr },
        { '!', DefTokenTypes.OpLogicalNot },
        { '~', DefTokenTypes.OpBitNot },
        { '?', DefTokenTypes.OpQuestionMark },
        { ',', DefTokenTypes.OpComma },
        { '.', DefTokenTypes.OpDot },
        { ':', DefTokenTypes.OpColon },
        { ';', DefTokenTypes.OpSemicolon },
        { '{', DefTokenTypes.OpBracketOpen },
        { '}', DefTokenTypes.OpBracketClose },
        { '(', DefTokenTypes.OpParenthesesOpen },
        { ')', DefTokenTypes.OpParenthesesClose },
        { '[', DefTokenTypes.OpSquareBracketsOpen },
        { ']', DefTokenTypes.OpSquareBracketsClose },
    };
    
    public static TokensCompositionMap CreateOperatorsCompositionMap() => new(
        (DefTokenTypes.OpPlus, DefTokenTypes.OpPlus, DefTokenTypes.OpIncrement),
        (DefTokenTypes.OpMinus, DefTokenTypes.OpMinus, DefTokenTypes.OpDecrement),
        (DefTokenTypes.OpPlus, DefTokenTypes.OpAssign, DefTokenTypes.OpPlusAssign),
        (DefTokenTypes.OpMinus, DefTokenTypes.OpAssign, DefTokenTypes.OpMinusAssign),
        (DefTokenTypes.OpMultiply, DefTokenTypes.OpAssign, DefTokenTypes.OpMultAssign),
        (DefTokenTypes.OpMultiply, DefTokenTypes.OpMultiply, DefTokenTypes.OpExponent),
        (DefTokenTypes.OpDiv, DefTokenTypes.OpAssign, DefTokenTypes.OpDivAssign),
        (DefTokenTypes.OpBitOr, DefTokenTypes.OpAssign, DefTokenTypes.OpBitOrAssign),
        (DefTokenTypes.OpBitAnd, DefTokenTypes.OpAssign, DefTokenTypes.OpBitAndAssign),
        (DefTokenTypes.OpLogicalOr, DefTokenTypes.OpAssign, DefTokenTypes.OpLogicalOrAssign),
        (DefTokenTypes.OpLogicalAnd, DefTokenTypes.OpAssign, DefTokenTypes.OpLogicalAndAssign),
        (DefTokenTypes.OpAssign, DefTokenTypes.OpAssign, DefTokenTypes.OpEquals),
        (DefTokenTypes.OpEquals, DefTokenTypes.OpAssign, DefTokenTypes.OpEqualsStrict),
        (DefTokenTypes.OpGreater, DefTokenTypes.OpAssign, DefTokenTypes.OpGreaterOrEqual),
        (DefTokenTypes.OpGreater, DefTokenTypes.OpGreater, DefTokenTypes.OpBitRightShift),
        (DefTokenTypes.OpBitRightShift, DefTokenTypes.OpGreater, DefTokenTypes.OpBitUnsignedRightShift),
        (DefTokenTypes.OpLess, DefTokenTypes.OpLess, DefTokenTypes.OpBitLeftShift),
        (DefTokenTypes.OpLess, DefTokenTypes.OpAssign, DefTokenTypes.OpLessOrEqual),
        (DefTokenTypes.OpLogicalNot, DefTokenTypes.OpAssign, DefTokenTypes.OpNotEqual),
        (DefTokenTypes.OpNotEqual, DefTokenTypes.OpAssign, DefTokenTypes.OpNotEqualStrict),
        (DefTokenTypes.OpBitAnd, DefTokenTypes.OpBitAnd, DefTokenTypes.OpLogicalAnd),
        (DefTokenTypes.OpBitOr, DefTokenTypes.OpBitOr, DefTokenTypes.OpLogicalOr),
        (DefTokenTypes.OpAssign, DefTokenTypes.OpGreater, DefTokenTypes.OpArrow)
    );
    
    public static TokensCompositionMap CreateKeywordsCompositionMap() => new(
        (DefTokenTypes.Else, DefTokenTypes.If, DefTokenTypes.ElseIf)
    );
    
    public static TokenizerFactory CreateTokenizerFactory()
    {
        var processors = new List<ITokensProcessor>();

        processors.Add(new DefaultCommentsProcessor());

        processors.Add(new DefaultStringLiteralsProcessor(
            DefTokenTypes.StringLiteral));

        var keywordsMap = new Dictionary<string, int>()
        {
            { "true", DefTokenTypes.True },
            { "false", DefTokenTypes.False },
            { "NaN", DefTokenTypes.FloatNaN }
        };

        var keywordsCompositionMap = CreateKeywordsCompositionMap();
        
        var identifierProcessor = new DefaultIdentifierProcessor(DefTokenTypes.Identifier, keywordsMap);
        
        var keywordsComposition = new TokensCompositionDecorator(
            identifierProcessor, keywordsCompositionMap, false);

        processors.Add(keywordsComposition);

        //var operationInfos = DefOperationInfos.OperationInfos;
        
        var operatorsMap = CreateOperatorsMap();

        var operatorSymbolsSet = new HashSet<char>(operatorsMap.Keys);

        processors.Add(new DefaultHexLiteralProcessor(DefTokenTypes.IntHex, operatorSymbolsSet));
        processors.Add(new DefaultBinaryIntLiteralProcessor(DefTokenTypes.IntBinary, operatorSymbolsSet));
        processors.Add(new DefaultNumberLiteralProcessor(
            DefTokenTypes.IntDecimal,
            DefTokenTypes.Float,
            DefTokenTypes.FloatE,
            operatorSymbolsSet));

        var operatorsProcessor = new DefaultOperatorsProcessor(operatorsMap);
        var operatorsComposition = new TokensCompositionDecorator(
            operatorsProcessor, CreateOperatorsCompositionMap(), true);
        

        processors.Add(operatorsComposition);

        return new TokenizerFactory(processors);
    }
}