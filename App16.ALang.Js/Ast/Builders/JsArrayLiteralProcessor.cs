using App16.ALang.Ast;
using App16.ALang.Ast.Builders;
using App16.ALang.Ast.Errors;
using App16.ALang.Js.Tokenizers;

namespace App16.ALang.Js.Ast.Builders;

public sealed class JsArrayLiteralProcessor : IAstNodeProcessor
{
    private readonly IAstNodeProcessor _expressionProcessor;

    public JsArrayLiteralProcessor(IAstNodeProcessor expressionProcessor)
    {
        _expressionProcessor = expressionProcessor;
    }
    
    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;
        
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpSquareBracketsOpen);

        var firstToken = tokensIterator.CurrentToken;
        
        // current token = [
        if (tokensIterator.NextToken.TokenType == JsTokenTypes.OpSquareBracketsClose)
        {
            tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpSquareBracketsClose);
            return new JsArrayLiteral(firstToken);
        }

        var arr = new JsArrayLiteral(firstToken);
        
        context.PushStopPoint(JsStopPoints.SquareBracketClose);
        
        while (tokensIterator.NextToken.TokenType != JsTokenTypes.OpSquareBracketsClose)
        {
            tokensIterator.CheckEarlyEof();
            
            context.PushStopPoint(JsStopPoints.OptionalComma);

            var element = _expressionProcessor.ProcessNext(context);
            
            context.PopStopPoint();
            
            arr.AddNode(element);
            
            tokensIterator.CheckEarlyEof();

            switch (tokensIterator.NextToken.TokenType)
            {
                case JsTokenTypes.OpComma:
                    tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpComma);
                    break;
                case JsTokenTypes.OpSquareBracketsClose:
                    // we'll stop here
                    break;
                default:
                    throw new AstUnexpectedTokenError(tokensIterator.NextToken);
            }
        }
        
        context.PopStopPoint();
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpSquareBracketsClose);

        return arr;
    }
}

public sealed class JsArrayLiteralLookup : IForwardLookup
{
    public bool LookForward(AstTokensIterator tokensIterator)
    {
        return tokensIterator.NextToken.TokenType == JsTokenTypes.OpSquareBracketsOpen;
    }
}