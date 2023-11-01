using App16.ALang.Ast;
using App16.ALang.Ast.Builders;
using App16.ALang.Js.Tokenizers;

namespace App16.ALang.Js.Ast.Builders;

public sealed class JsCommaSeparatedExpressionsProcessor : IAstNodeProcessor
{
    private readonly IAstNodeProcessor _expressionsProcessor;

    public JsCommaSeparatedExpressionsProcessor(IAstNodeProcessor expressionsProcessor)
    {
        _expressionsProcessor = expressionsProcessor;
    }
    
    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;
        
        context.PushStopPoint(JsStopPoints.OptionalComma);

        var exp = _expressionsProcessor.ProcessNext(context);

        if (tokensIterator.NextToken.TokenType != JsTokenTypes.OpComma)
        {
            context.PopStopPoint();
            return exp;
        }
        
        var seq = new AstSequence();
        seq.AddNode(exp);

        while (tokensIterator.IfNextMoveForward(JsTokenTypes.OpComma))
        {

            var nextExp = _expressionsProcessor.ProcessNext(context);
            
            seq.AddNode(nextExp);
        }

        context.PopStopPoint();
        
        return seq;
    }
}