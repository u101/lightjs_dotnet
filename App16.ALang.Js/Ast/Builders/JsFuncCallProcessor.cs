using App16.ALang.Ast;
using App16.ALang.Ast.Builders;
using App16.ALang.Js.Tokenizers;

namespace App16.ALang.Js.Ast.Builders;

public sealed class JsFuncCallProcessor :  IAstDecoratorProcessor
{
    private readonly IAstNodeProcessor _expressionProcessor;

    public JsFuncCallProcessor(
        IAstNodeProcessor expressionProcessor)
    {
        _expressionProcessor = expressionProcessor;
    }
    
    public IAstNode ProcessNext(IAstNode decoratee, AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;
        
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpParenthesesOpen);

        var funcCallToken = tokensIterator.CurrentToken;

        var argumentsList = new JsFunctionCallArguments();
                    
        if (tokensIterator.IfNextMoveForward(JsTokenTypes.OpParenthesesClose))
        {
            // func call without arguments
            return new JsFunctionCall(decoratee, argumentsList, funcCallToken);
        }

        context.PushStopPoint(JsStopPoints.ParenthesesClose);
        context.PushStopPoint(JsStopPoints.OptionalComma);
        
        var funcArg = _expressionProcessor.ProcessNext(context);
                        
        argumentsList.AddNode(funcArg);
                        
        while (tokensIterator.IfNextMoveForward(JsTokenTypes.OpComma))
        {
            funcArg = _expressionProcessor.ProcessNext(context);
            argumentsList.AddNode(funcArg);
        }
        
        context.PopStopPoint();
        context.PopStopPoint();
        
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpParenthesesClose);

        return new JsFunctionCall(decoratee, argumentsList, funcCallToken);
    }
}

public sealed class JsFuncCallLookup : IDecoratorForwardLookup
{
    public bool LookForward(IAstNode decoratee, AstTokensIterator tokensIterator)
    {
        if (decoratee is not IAstValueNode) return false;
        var current = tokensIterator.CurrentToken.TokenType;
        var next = tokensIterator.NextToken.TokenType;
        
        return next == JsTokenTypes.OpParenthesesOpen &&
               (current == JsTokenTypes.Identifier ||
                current ==JsTokenTypes.OpParenthesesClose ||
                current ==JsTokenTypes.OpSquareBracketsClose);
    }
}