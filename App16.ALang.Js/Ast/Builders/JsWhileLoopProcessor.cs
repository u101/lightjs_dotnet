using App16.ALang.Ast;
using App16.ALang.Ast.Builders;
using App16.ALang.Js.Tokenizers;

namespace App16.ALang.Js.Ast.Builders;

public sealed class JsWhileLoopProcessor : IAstNodeProcessor
{
    private readonly IAstNodeProcessor _expressionProcessor;
    private readonly IAstNodeProcessor _blockInBracketsProcessor;
    private readonly IAstNodeProcessor _codeLineProcessor;

    public JsWhileLoopProcessor(
        IAstNodeProcessor expressionProcessor,
        IAstNodeProcessor blockInBracketsProcessor,
        IAstNodeProcessor codeLineProcessor)
    {
        _expressionProcessor = expressionProcessor;
        _blockInBracketsProcessor = blockInBracketsProcessor;
        _codeLineProcessor = codeLineProcessor;
    }
    
    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;

        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.While);

        var loopStartToken = tokensIterator.CurrentToken;

        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpParenthesesOpen);
        
        context.PushStopPoint(JsStopPoints.ParenthesesClose);

        var condition = _expressionProcessor.ProcessNext(context);
        
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

        return new AstWhileLoop(condition, mainBody, loopStartToken);
    }
}

public sealed class JsAstWhileLoopLookup : IForwardLookup
{
    public bool LookForward(AstTokensIterator tokensIterator)
    {
        return tokensIterator.NextToken.TokenType == JsTokenTypes.While;
    }
}