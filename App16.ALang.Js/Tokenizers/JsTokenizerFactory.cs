using App16.ALang.Tokenizers;

namespace App16.ALang.Js.Tokenizers;

public static class JsTokenizerFactory
{
    private static readonly TokenizerFactory TokenizerFactory = CreateTokenizerFactory();

    public static JsTokenizer CreateTokenizer(string sourceCodeString) =>
        new JsTokenizer(TokenizerFactory.CreateTokenizer(sourceCodeString));
    
    private static TokenizerFactory CreateTokenizerFactory()
    {
        var processors = new List<ITokensProcessor>();

        processors.Add(new DefaultCommentsProcessor());

        processors.Add(new DefaultStringLiteralsProcessor(
            JsTokenTypes.StringLiteral));

        var keywordsMap = new Dictionary<string, int>()
        {
            { "null", JsTokenTypes.Null },
            { "undefined", JsTokenTypes.Undefined },
            { "this", JsTokenTypes.This },

            { "true", JsTokenTypes.True },
            { "false", JsTokenTypes.False },

            { "NaN", JsTokenTypes.FloatNaN },

            { "var", JsTokenTypes.Var },
            { "let", JsTokenTypes.Let },
            { "const", JsTokenTypes.Const },
            { "function", JsTokenTypes.Function },

            { "return", JsTokenTypes.Return },
            { "break", JsTokenTypes.Break },
            { "continue", JsTokenTypes.Continue },

            { "if", JsTokenTypes.If },
            { "else", JsTokenTypes.Else },
            { "switch", JsTokenTypes.Switch },
            { "case", JsTokenTypes.Case },
            { "default", JsTokenTypes.Default },
            { "while", JsTokenTypes.While },
            { "do", JsTokenTypes.Do },
            { "for", JsTokenTypes.For },
            { "in", JsTokenTypes.In },
        };


        var opCompositionMap = CreateOperatorsCompositionMap();

        var identifierCompositionMap = CreateKeywordsCompositionMap();

        var identifierProcessor = new DefaultIdentifierProcessor(JsTokenTypes.Identifier, keywordsMap);
        var identifierCompositionDecorator = new TokensCompositionDecorator(
            identifierProcessor, identifierCompositionMap, false);

        processors.Add(identifierCompositionDecorator);

        var operatorsMap = CreateOperatorsMap();

        var operatorSymbolsSet = new HashSet<char>(operatorsMap.Keys);

        processors.Add(new DefaultHexLiteralProcessor(JsTokenTypes.IntHex, operatorSymbolsSet));
        processors.Add(new DefaultBinaryIntLiteralProcessor(JsTokenTypes.IntBinary, operatorSymbolsSet));
        processors.Add(new DefaultNumberLiteralProcessor(
            JsTokenTypes.IntDecimal,
            JsTokenTypes.Float,
            JsTokenTypes.FloatE,
            operatorSymbolsSet));

        var operatorsProcessor = new DefaultOperatorsProcessor(operatorsMap);
        var operatorsCompositionDecorator =
            new TokensCompositionDecorator(operatorsProcessor, opCompositionMap, true);

        processors.Add(operatorsCompositionDecorator);
        return new TokenizerFactory(processors);
    }
    
    private static TokensCompositionMap CreateKeywordsCompositionMap() => new(
        (JsTokenTypes.Else, JsTokenTypes.If, JsTokenTypes.ElseIf)
    );
    
    private static Dictionary<char, int> CreateOperatorsMap() => new()
    {
        { '>', JsTokenTypes.OpGreater },
        { '<', JsTokenTypes.OpLess },
        { '=', JsTokenTypes.OpAssign },
        { '+', JsTokenTypes.OpPlus },
        { '-', JsTokenTypes.OpMinus },
        { '*', JsTokenTypes.OpMultiply },
        { '/', JsTokenTypes.OpDiv },
        { '%', JsTokenTypes.OpModulo },
        { '&', JsTokenTypes.OpBitAnd },
        { '|', JsTokenTypes.OpBitOr },
        { '^', JsTokenTypes.OpBitXor },
        { '!', JsTokenTypes.OpLogicalNot },
        { '~', JsTokenTypes.OpBitNot },
        { '?', JsTokenTypes.OpQuestionMark },
        { ',', JsTokenTypes.OpComma },
        { '.', JsTokenTypes.OpDot },
        { ':', JsTokenTypes.OpColon },
        { ';', JsTokenTypes.OpSemicolon },
        { '{', JsTokenTypes.OpBracketOpen },
        { '}', JsTokenTypes.OpBracketClose },
        { '(', JsTokenTypes.OpParenthesesOpen },
        { ')', JsTokenTypes.OpParenthesesClose },
        { '[', JsTokenTypes.OpSquareBracketsOpen },
        { ']', JsTokenTypes.OpSquareBracketsClose },
    };
    
    private static TokensCompositionMap CreateOperatorsCompositionMap() => new(
        (JsTokenTypes.OpPlus, JsTokenTypes.OpPlus, JsTokenTypes.OpIncrement),
        (JsTokenTypes.OpMinus, JsTokenTypes.OpMinus, JsTokenTypes.OpDecrement),
        (JsTokenTypes.OpPlus, JsTokenTypes.OpAssign, JsTokenTypes.OpPlusAssign),
        (JsTokenTypes.OpMinus, JsTokenTypes.OpAssign, JsTokenTypes.OpMinusAssign),
        (JsTokenTypes.OpMultiply, JsTokenTypes.OpAssign, JsTokenTypes.OpMultAssign),
        (JsTokenTypes.OpMultiply, JsTokenTypes.OpMultiply, JsTokenTypes.OpExponent),
        (JsTokenTypes.OpDiv, JsTokenTypes.OpAssign, JsTokenTypes.OpDivAssign),
        (JsTokenTypes.OpBitOr, JsTokenTypes.OpAssign, JsTokenTypes.OpBitOrAssign),
        (JsTokenTypes.OpBitAnd, JsTokenTypes.OpAssign, JsTokenTypes.OpBitAndAssign),
        (JsTokenTypes.OpLogicalOr, JsTokenTypes.OpAssign, JsTokenTypes.OpLogicalOrAssign),
        (JsTokenTypes.OpLogicalAnd, JsTokenTypes.OpAssign, JsTokenTypes.OpLogicalAndAssign),
        (JsTokenTypes.OpAssign, JsTokenTypes.OpAssign, JsTokenTypes.OpEquals),
        (JsTokenTypes.OpEquals, JsTokenTypes.OpAssign, JsTokenTypes.OpEqualsStrict),
        (JsTokenTypes.OpGreater, JsTokenTypes.OpAssign, JsTokenTypes.OpGreaterOrEqual),
        (JsTokenTypes.OpGreater, JsTokenTypes.OpGreater, JsTokenTypes.OpBitRightShift),
        (JsTokenTypes.OpBitRightShift, JsTokenTypes.OpGreater, JsTokenTypes.OpBitUnsignedRightShift),
        (JsTokenTypes.OpLess, JsTokenTypes.OpLess, JsTokenTypes.OpBitLeftShift),
        (JsTokenTypes.OpLess, JsTokenTypes.OpAssign, JsTokenTypes.OpLessOrEqual),
        (JsTokenTypes.OpLogicalNot, JsTokenTypes.OpAssign, JsTokenTypes.OpNotEqual),
        (JsTokenTypes.OpNotEqual, JsTokenTypes.OpAssign, JsTokenTypes.OpNotEqualStrict),
        (JsTokenTypes.OpBitAnd, JsTokenTypes.OpBitAnd, JsTokenTypes.OpLogicalAnd),
        (JsTokenTypes.OpBitOr, JsTokenTypes.OpBitOr, JsTokenTypes.OpLogicalOr),
        (JsTokenTypes.OpAssign, JsTokenTypes.OpGreater, JsTokenTypes.OpArrow)
    );
}