using App16.ALang.Ast;
using App16.ALang.Ast.Builders;
using App16.ALang.Js.Tokenizers;

namespace App16.ALang.Js.Ast.Builders;

public sealed class JsBlockInBracketsProcessor : IAstNodeProcessor 
{
    private readonly IAstNodeProcessor _codeLineProcessor;

    public JsBlockInBracketsProcessor(IAstNodeProcessor codeLineProcessor)
    {
        _codeLineProcessor = codeLineProcessor;
    }
    
    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;

        // starting token - just before brackets open
        
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpBracketOpen);
        
        tokensIterator.SkipTokens(JsTokenTypes.OpSemicolon);
        
        tokensIterator.CheckEarlyEof();

        if (tokensIterator.NextToken.TokenType == JsTokenTypes.OpBracketClose)
        {
            tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpBracketClose);
            return AstEmptyNode.Instance;
        }
        
        context.PushStopPoint(JsStopPoints.BracketClose);
        context.PushStopPoint(JsStopPointAutoSemicolon.WithPreviousCheck);
        
        var firstExpression = _codeLineProcessor.ProcessNext(context);
        
        tokensIterator.SkipTokens(JsTokenTypes.OpSemicolon);
        tokensIterator.CheckEarlyEof();

        if (tokensIterator.NextToken.TokenType == JsTokenTypes.OpBracketClose)
        {
            tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpBracketClose);
            
            context.PopStopPoint();
            context.PopStopPoint();
            
            return firstExpression;
        }
        
        var sq = new AstSequence();
        sq.AddNode(firstExpression);

        while (tokensIterator.NextToken.TokenType != JsTokenTypes.OpBracketClose)
        {
            var nextExpression = _codeLineProcessor.ProcessNext(context);
            
            tokensIterator.SkipTokens(JsTokenTypes.OpSemicolon);
            tokensIterator.CheckEarlyEof();
            
            sq.AddNode(nextExpression);
        }
        
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpBracketClose);
            
        context.PopStopPoint();
        context.PopStopPoint();

        return sq;
    }
}