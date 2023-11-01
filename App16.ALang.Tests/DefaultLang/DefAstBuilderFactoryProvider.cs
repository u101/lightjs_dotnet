using App16.ALang.Ast;
using App16.ALang.Ast.Builders;

namespace App16.ALang.Tests.DefaultLang;

public static class DefAstBuilderFactoryProvider
{
    public static AstModelBuilderFactory CreateAstBuilderFactory()
    {
        var idProcessorRec = new AstProcessorRecord(
            new AstIdentifierLookup(DefTokenTypes.Identifier), 
            new AstIdentifierProcessor(DefTokenTypes.Identifier));

        var expressionProcessorRef = new AstNodeProcessorRef();

        var expressionInParenthesesProcessor = new AstExpressionInParenthesesProcessor(
            DefTokenTypes.OpParenthesesOpen, DefTokenTypes.OpParenthesesClose, expressionProcessorRef);
        
        var expInParRec = new AstProcessorRecord(
            new AstExpressionInParenthesesLookup(DefTokenTypes.OpParenthesesOpen),
            expressionInParenthesesProcessor);

        var ternaryIfOperationInfo = new AstTernaryOperationInfo(
            DefTokenTypes.OpQuestionMark,
            DefTokenTypes.OpColon,
            DefOperationPriorityGroups.TernaryIf);
        
        var binaryOperatorsMap = 
            DefOperationInfos.BinaryOperationInfos.ToDictionary(i => i.OperatorTokenType);
        var unaryOperatorsMap =
            DefOperationInfos.UnaryOperationInfos.ToDictionary(i => i.OperatorTokenType);

        var dotPropDecorator = new AstDecoratorRecord(
            new AstDotPropertyLookup(DefTokenTypes.OpDot, DefTokenTypes.Identifier),
            new AstDotPropertyProcessor(DefTokenTypes.OpDot, DefTokenTypes.Identifier));
        
        var sqbPropDecorator = new AstDecoratorRecord(
            new AstSqbPropertyLookup(DefTokenTypes.OpSquareBracketsOpen),
            new AstSqbPropertyProcessor(DefTokenTypes.OpSquareBracketsOpen, DefTokenTypes.OpSquareBracketsClose, expressionProcessorRef));

        var expressionProcessor = new AstExpressionProcessor(
            new[] { idProcessorRec, expInParRec },
            new[] { dotPropDecorator, sqbPropDecorator },
            binaryOperatorsMap,
            unaryOperatorsMap,
            ternaryIfOperationInfo);

        expressionProcessorRef.Processor = expressionProcessor;

        return new AstModelBuilderFactory(expressionProcessor);
    }
}