using App16.ALang.Ast;
using App16.ALang.Ast.Builders;
using App16.ALang.Ast.Errors;
using App16.ALang.Js.Tokenizers;
using App16.ALang.Tokenizers;

namespace App16.ALang.Js.Ast.Builders;

public sealed class JsObjectLiteralProcessor : IAstNodeProcessor
{
    private readonly IAstNodeProcessor _expressionProcessor;

    public JsObjectLiteralProcessor(IAstNodeProcessor expressionProcessor)
    {
        _expressionProcessor = expressionProcessor;
    }
    
    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;
        
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpBracketOpen);

        var firstToken = tokensIterator.CurrentToken;
        
        // current token = {
        if (tokensIterator.NextToken.TokenType == JsTokenTypes.OpBracketClose)
        {
            tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpBracketClose);
            return new JsObjectLiteral(firstToken);
        }
        
        context.PushStopPoint(JsStopPoints.BracketClose);
        
        var obj = new JsObjectLiteral(firstToken);

        while (tokensIterator.NextToken.TokenType != JsTokenTypes.OpBracketClose)
        {
            tokensIterator.CheckEarlyEof();

            string propName;
            
            if (tokensIterator.IfNextMoveForward(JsTokenTypes.Identifier))
            {
                propName = TokenizerUtils.GetTokenStringValue(context.SourceCodeString, tokensIterator.CurrentToken);
            }
            else if (tokensIterator.IfNextMoveForward(JsTokenTypes.StringLiteral))
            {
                propName = TokenizerUtils.GetTokenStringValue(context.SourceCodeString, tokensIterator.CurrentToken);
            }
            else if (tokensIterator.IfNextMoveForward(JsTokenTypes.IntDecimal))
            {
                propName = TokenizerUtils.GetTokenStringValue(context.SourceCodeString, tokensIterator.CurrentToken);
            }
            else
            {
                throw new AstUnexpectedTokenError(tokensIterator.NextToken);
            }
            
            tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpColon);
            
            context.PushStopPoint(JsStopPoints.OptionalComma);

            var propertyValue = _expressionProcessor.ProcessNext(context);
            
            context.PopStopPoint();
            
            obj.AddNode(new JsObjectLiteralProperty(propName, propertyValue));
            
            tokensIterator.CheckEarlyEof();

            switch (tokensIterator.NextToken.TokenType)
            {
                case JsTokenTypes.OpComma:
                    tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpComma);
                    break;
                case JsTokenTypes.OpBracketClose:
                    // we'll stop here
                    break;
                default:
                    throw new AstUnexpectedTokenError(tokensIterator.NextToken);
            }
        }

        context.PopStopPoint();
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpBracketClose);

        return obj;
    }
}

public sealed class JsObjectLiteralLookup : IForwardLookup
{
    public bool LookForward(AstTokensIterator tokensIterator)
    {
        return tokensIterator.NextToken.TokenType == JsTokenTypes.OpBracketOpen;
    }
}