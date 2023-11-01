using App16.ALang.Ast;
using App16.ALang.Ast.Builders;
using App16.ALang.Js.Tokenizers;
using App16.ALang.Tokenizers;

namespace App16.ALang.Js.Ast.Builders;

public static class JsAstBuilderFactory
{
    
    private static readonly AstModelBuilderFactory BuilderFactory = CreateAstBuilderFactory();

    public static JsAstBuilder CreateBuilder(string sourceCodeString)
    {
        if (string.IsNullOrEmpty(sourceCodeString))
        {
            throw new ArgumentException("input string is null or empty");
        }
        
        var ljsTokenizer = JsTokenizerFactory.CreateTokenizer(sourceCodeString);
        var tokens = ljsTokenizer.ReadTokens();

        return CreateBuilder(sourceCodeString, tokens);
    }
    
    public static JsAstBuilder CreateBuilder(string sourceCodeString, List<Token> tokens)
    {
        
        if (string.IsNullOrEmpty(sourceCodeString))
        {
            throw new ArgumentException("input string is null or empty");
        }
        
        if (tokens == null)
            throw new ArgumentNullException(nameof(tokens));

        if (tokens.Count == 0)
            throw new ArgumentException("empty tokens list");

        var astModelBuilder = BuilderFactory.CreateBuilder(sourceCodeString, tokens);

        return new JsAstBuilder(sourceCodeString, astModelBuilder);
    }
    
    private static AstModelBuilderFactory CreateAstBuilderFactory()
    {
        var idProcessorRec = new AstProcessorRecord(
            new AstIdentifierLookup(JsTokenTypes.Identifier), 
            new AstIdentifierProcessor(JsTokenTypes.Identifier));

        var literalsProcessor = new AstProcessorRecord(
            new JsAstLiteralsLookup(),
            new JsAstLiteralsProcessor()
        );

        var funcParamsProc = new JsFunctionDeclarationParametersProcessor(
            literalsProcessor.Processor);

        var transControlOpsProcessor = new AstProcessorRecord(
            new JsControlTransferOperatorLookup(),
            new JsControlTransferOperatorProcessor());
        
        var expressionProcessorRef = new AstNodeProcessorRef();
        var codeLineProcessorRef = new AstNodeProcessorRef();
        
        var varDeclarationProc = new AstProcessorRecord(
            new JsVarDeclarationLookup(),
            new JsVarDeclarationProcessor(expressionProcessorRef));

        var returnProc = new AstProcessorRecord(
            new JsReturnLookup(),
            new JsReturnProcessor(expressionProcessorRef)
        );

        var objLitProc = new AstProcessorRecord(
            new JsObjectLiteralLookup(),
            new JsObjectLiteralProcessor(expressionProcessorRef)); 
        
        var arrayLitProc = new AstProcessorRecord(
            new JsArrayLiteralLookup(),
            new JsArrayLiteralProcessor(expressionProcessorRef)); 
        
        var blockInBracketsProcessor = new JsBlockInBracketsProcessor(codeLineProcessorRef);

        var nameFuncProc = new AstProcessorRecord(
            new JsNamedFunctionLookup(),
            new JsNamedFunctionDeclarationProcessor(funcParamsProc, blockInBracketsProcessor)
        );

        var anonFuncProc = new AstProcessorRecord(
            new JsAnonymousFunctionDeclarationLookup(),
            new JsAnonymousFunctionDeclarationProcessor(funcParamsProc, blockInBracketsProcessor));
        
        var arrowFuncProc = new AstProcessorRecord(
            new JsArrowFunctionDeclarationLookup(),
            new JsArrowFunctionDeclarationProcessor(funcParamsProc, blockInBracketsProcessor, expressionProcessorRef));

        var ifBlockProcessor = new AstProcessorRecord(
            new JsIfBlockLookup(),
            new JsIfBlockProcessor(expressionProcessorRef, blockInBracketsProcessor, codeLineProcessorRef));

        var whileLoopProcessor = new AstProcessorRecord(
            new JsAstWhileLoopLookup(),
            new JsWhileLoopProcessor(expressionProcessorRef, blockInBracketsProcessor, codeLineProcessorRef)
        );

        var commaSeparatedExpressionsProcessor = 
            new JsCommaSeparatedExpressionsProcessor(expressionProcessorRef);

        var forLoopProcessor = new AstProcessorRecord(
            new JsForLoopLookup(),
            new JsForLoopProcessor(
                varDeclarationProc, expressionProcessorRef, 
                commaSeparatedExpressionsProcessor, blockInBracketsProcessor, codeLineProcessorRef)
        );

        var switchProc = new AstProcessorRecord(
            new JsSwitchBlockLookup(),
            new JsSwitchBlockProcessor(expressionProcessorRef, codeLineProcessorRef));

        var expressionInParenthesesProcessor = new AstExpressionInParenthesesProcessor(
            JsTokenTypes.OpParenthesesOpen, JsTokenTypes.OpParenthesesClose, expressionProcessorRef);
        
        var expInParRec = new AstProcessorRecord(
            new AstExpressionInParenthesesLookup(JsTokenTypes.OpParenthesesOpen),
            expressionInParenthesesProcessor);

        var ternaryIfOperationInfo = new AstTernaryOperationInfo(
            JsTokenTypes.OpQuestionMark,
            JsTokenTypes.OpColon,
            JsOperationPriorityGroups.TernaryIf);
        
        var binaryOperatorsMap = 
            JsOperationInfos.BinaryOperationInfos.ToDictionary(i => i.OperatorTokenType);
        var unaryOperatorsMap =
            JsOperationInfos.UnaryOperationInfos.ToDictionary(i => i.OperatorTokenType);

        var funcCallDecorator = new AstDecoratorRecord(
            new JsFuncCallLookup(),
            new JsFuncCallProcessor(expressionProcessorRef)
        );
        
        var dotPropDecorator = new AstDecoratorRecord(
            new AstDotPropertyLookup(JsTokenTypes.OpDot, JsTokenTypes.Identifier),
            new AstDotPropertyProcessor(JsTokenTypes.OpDot, JsTokenTypes.Identifier));
        
        var sqbPropDecorator = new AstDecoratorRecord(
            new AstSqbPropertyLookup(JsTokenTypes.OpSquareBracketsOpen),
            new AstSqbPropertyProcessor(JsTokenTypes.OpSquareBracketsOpen, JsTokenTypes.OpSquareBracketsClose, expressionProcessorRef));

        var expressionProcessor = new AstExpressionProcessor(
            new[]
            {
                idProcessorRec, literalsProcessor, arrowFuncProc, 
                expInParRec, anonFuncProc, objLitProc, arrayLitProc
            },
            new[] { dotPropDecorator, sqbPropDecorator, funcCallDecorator },
            binaryOperatorsMap,
            unaryOperatorsMap,
            ternaryIfOperationInfo);

        
        var codeLineProcessor = new JsCodeLineProcessor(
            new[]
            {
                varDeclarationProc, 
                ifBlockProcessor, 
                whileLoopProcessor, 
                forLoopProcessor,
                switchProc,
                returnProc,
                nameFuncProc,
                transControlOpsProcessor
            },
            expressionProcessorRef);
        
        expressionProcessorRef.Processor = expressionProcessor;
        codeLineProcessorRef.Processor = codeLineProcessor;

        var mainBlockProcessor = new JsMainBlockProcessor(codeLineProcessor);

        return new AstModelBuilderFactory(mainBlockProcessor);
    }
    
}