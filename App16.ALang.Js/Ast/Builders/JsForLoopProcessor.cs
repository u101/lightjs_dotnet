using App16.ALang.Ast;
using App16.ALang.Ast.Builders;
using App16.ALang.Js.Tokenizers;

namespace App16.ALang.Js.Ast.Builders;

public sealed class JsForLoopProcessor : IAstNodeProcessor
{
    private readonly AstProcessorRecord _varDeclarationProcessor;
    private readonly IAstNodeProcessor _expressionProcessor;
    private readonly IAstNodeProcessor _commaSeparatedExpressionsProcessor;
    private readonly IAstNodeProcessor _blockInBracketsProcessor;
    private readonly IAstNodeProcessor _codeLineProcessor;

    public JsForLoopProcessor(
        AstProcessorRecord varDeclarationProcessor,
        IAstNodeProcessor expressionProcessor,
        IAstNodeProcessor commaSeparatedExpressionsProcessor,
        IAstNodeProcessor blockInBracketsProcessor,
        IAstNodeProcessor codeLineProcessor)
    {
        _varDeclarationProcessor = varDeclarationProcessor;
        _expressionProcessor = expressionProcessor;
        _commaSeparatedExpressionsProcessor = commaSeparatedExpressionsProcessor;
        _blockInBracketsProcessor = blockInBracketsProcessor;
        _codeLineProcessor = codeLineProcessor;
    }
    
    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;

        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.For);

        var loopStartToken = tokensIterator.CurrentToken;
        
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpParenthesesOpen);

        var initExpr = AstEmptyNode.Instance;
        var condExpr = AstEmptyNode.Instance;
        var iterExpr = AstEmptyNode.Instance;

        if (tokensIterator.NextToken.TokenType != JsTokenTypes.OpSemicolon)
        {
            context.PushStopPoint(JsStopPoints.Semicolon);
            
            initExpr = _varDeclarationProcessor.Lookup.LookForward(tokensIterator) ? 
                _varDeclarationProcessor.Processor.ProcessNext(context) : 
                _expressionProcessor.ProcessNext(context);
            
            context.PopStopPoint();
        }
        
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpSemicolon);
        
        if (tokensIterator.NextToken.TokenType != JsTokenTypes.OpSemicolon)
        {
            context.PushStopPoint(JsStopPoints.Semicolon);
            
            condExpr = _expressionProcessor.ProcessNext(context);
            
            context.PopStopPoint();
        }
        
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpSemicolon);
        
        context.PushStopPoint(JsStopPoints.ParenthesesClose);
        
        if (tokensIterator.NextToken.TokenType != JsTokenTypes.OpParenthesesClose)
        {
            iterExpr = _commaSeparatedExpressionsProcessor.ProcessNext(context);
        }
        
        context.PopStopPoint();
        
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpParenthesesClose);
        
        var hasBrackets =
            tokensIterator.NextToken.TokenType == JsTokenTypes.OpBracketOpen;

        IAstNode mainBody;
        
        if (hasBrackets)
        {
            mainBody = _blockInBracketsProcessor.ProcessNext(context);
        }
        else
        {
            context.PushStopPoint(JsStopPointAutoSemicolon.WithPreviousCheck);
            
            mainBody = _codeLineProcessor.ProcessNext(context);
            
            tokensIterator.SkipTokens(JsTokenTypes.OpSemicolon);
            
            context.PopStopPoint();
        }
        
        return new AstForLoop(
            initExpr, condExpr, iterExpr, mainBody, loopStartToken);
    }
}

public sealed class JsForLoopLookup : IForwardLookup
{
    public bool LookForward(AstTokensIterator tokensIterator)
    {
        return tokensIterator.NextToken.TokenType == JsTokenTypes.For;
    }
}