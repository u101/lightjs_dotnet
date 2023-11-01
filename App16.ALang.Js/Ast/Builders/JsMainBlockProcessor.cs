using App16.ALang.Ast;
using App16.ALang.Ast.Builders;
using App16.ALang.Js.Tokenizers;

namespace App16.ALang.Js.Ast.Builders;

public sealed class JsMainBlockProcessor : IAstNodeProcessor
{
    private readonly IAstNodeProcessor _codeLineProcessor;

    public JsMainBlockProcessor(IAstNodeProcessor codeLineProcessor)
    {
        _codeLineProcessor = codeLineProcessor;
    }
    
    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;

        tokensIterator.SkipTokens(JsTokenTypes.OpSemicolon);
        
        tokensIterator.CheckEarlyEof();
        
        context.PushStopPoint(JsStopPointAutoSemicolon.WithPreviousCheck);
        
        var firstExpression = _codeLineProcessor.ProcessNext(context);
        
        tokensIterator.SkipTokens(JsTokenTypes.OpSemicolon);

        if (!tokensIterator.HasNextToken)
        {
            context.PopStopPoint();
            return firstExpression;
        }
        
        var sq = new AstSequence();
        sq.AddNode(firstExpression);

        while (tokensIterator.HasNextToken)
        {
            var node = _codeLineProcessor.ProcessNext(context);
            sq.AddNode(node);
            
            tokensIterator.SkipTokens(JsTokenTypes.OpSemicolon);
        }
        
        context.PopStopPoint();

        return sq;
    }
}