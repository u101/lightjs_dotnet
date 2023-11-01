using App16.ALang.Ast;
using App16.ALang.Ast.Builders;
using App16.ALang.Js.Tokenizers;

namespace App16.ALang.Js.Ast.Builders;

public class JsIfBlockProcessor : IAstNodeProcessor
{
    private readonly IAstNodeProcessor _expressionProcessor;
    private readonly IAstNodeProcessor _blockInBracketsProcessor;
    private readonly IAstNodeProcessor _codeLineProcessor;

    public JsIfBlockProcessor(
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

        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.If);

        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpParenthesesOpen);

        context.PushStopPoint(JsStopPoints.ParenthesesClose);

        var mainCondition = _expressionProcessor.ProcessNext(context);
        
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
        

        var result = new AstIfBlock(
            new AstConditionalExpression(mainCondition, mainBody));

        while (tokensIterator.NextToken.TokenType == JsTokenTypes.ElseIf)
        {
            tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.ElseIf);
            
            tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpParenthesesOpen);

            context.PushStopPoint(JsStopPoints.ParenthesesClose);

            var altCondition = _expressionProcessor.ProcessNext(context);
            
            context.PopStopPoint();
            
            tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpParenthesesClose);

            hasBrackets =
                tokensIterator.NextToken.TokenType == JsTokenTypes.OpBracketOpen;

            IAstNode altBody;

            if (hasBrackets)
            {
                altBody = _blockInBracketsProcessor.ProcessNext(context);
            }
            else
            {
                context.PushStopPoint(JsStopPointAutoSemicolon.WithPreviousCheck);
            
                altBody = _codeLineProcessor.ProcessNext(context);
            
                tokensIterator.SkipTokens(JsTokenTypes.OpSemicolon);
            
                context.PopStopPoint();
            }

            var conditionalExpression = new AstConditionalExpression(altCondition, altBody);
            
            result.ElseIfs.Add(conditionalExpression);
        }

        if (tokensIterator.NextToken.TokenType == JsTokenTypes.Else)
        {
            tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.Else);
            
            tokensIterator.CheckEarlyEof();

            hasBrackets = tokensIterator.NextToken.TokenType == JsTokenTypes.OpBracketOpen;

            IAstNode elseBody;

            if (hasBrackets)
            {
                elseBody = _blockInBracketsProcessor.ProcessNext(context);
            }
            else
            {
                context.PushStopPoint(JsStopPointAutoSemicolon.WithPreviousCheck);
            
                elseBody = _codeLineProcessor.ProcessNext(context);
            
                tokensIterator.SkipTokens(JsTokenTypes.OpSemicolon);
            
                context.PopStopPoint();
            }

            result.Else = elseBody;
        }

        return result;
    }
}

public class JsIfBlockLookup : IForwardLookup
{
    public bool LookForward(AstTokensIterator tokensIterator)
    {
        return tokensIterator.NextToken.TokenType == JsTokenTypes.If;
    }
}