using App16.ALang.Ast;
using App16.ALang.Ast.Builders;
using App16.ALang.Js.Tokenizers;

namespace App16.ALang.Js.Ast.Builders;

public sealed class JsSwitchBlockProcessor : IAstNodeProcessor
{
    private readonly IAstNodeProcessor _expressionProcessor;
    private readonly IAstNodeProcessor _codeLineProcessor;

    public JsSwitchBlockProcessor(
        IAstNodeProcessor expressionProcessor,
        IAstNodeProcessor codeLineProcessor)
    {
        _expressionProcessor = expressionProcessor;
        _codeLineProcessor = codeLineProcessor;
    }
    
    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;

        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.Switch);

        var switchBlockToken = tokensIterator.CurrentToken;

        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpParenthesesOpen);
        
        context.PushStopPoint(JsStopPoints.ParenthesesClose);
        
        var condition =
            _expressionProcessor.ProcessNext(context);
        
        context.PopStopPoint();
        
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpParenthesesClose);
        
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpBracketOpen);
        
        context.PushStopPoint(JsStopPoints.BracketClose);
        
        var seq = new AstSequence();

        while (tokensIterator.NextToken.TokenType != JsTokenTypes.OpBracketClose)
        {
            tokensIterator.CheckEarlyEof();
            
            var nextTokenType = tokensIterator.NextToken.TokenType;

            switch (nextTokenType)
            {
                case JsTokenTypes.Case:
                    
                    tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.Case);

                    var caseToken = tokensIterator.CurrentToken;
                    
                    context.PushStopPoint(JsStopPoints.Colon);
                    
                    var e = _expressionProcessor.ProcessNext(context);
                    
                    context.PopStopPoint();
                    
                    tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpColon);

                    var switchCase = new JsSwitchCase(e, caseToken);
                    
                    seq.AddNode(switchCase);
                    
                    break;
                
                case JsTokenTypes.Default:
                    tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.Default);

                    var defaultToken = tokensIterator.CurrentToken;
                    
                    tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpColon);
                    
                    var astSwitchDefault = new JsSwitchDefault(defaultToken);
                    
                    seq.AddNode(astSwitchDefault);
                    break;
                
                default:
                    
                    context.PushStopPoint(JsStopPointAutoSemicolon.WithPreviousCheck);
                    
                    var c = _codeLineProcessor.ProcessNext(context);
                    tokensIterator.SkipTokens(JsTokenTypes.OpSemicolon);
                    
                    context.PopStopPoint();
                    
                    seq.AddNode(c);
                    break;
            }
        }
        
        context.PopStopPoint();
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpBracketClose);

        return new JsSwitchBlock(condition, seq, switchBlockToken);
    }
}

public sealed class JsSwitchBlockLookup : IForwardLookup
{
    public bool LookForward(AstTokensIterator tokensIterator)
    {
        return tokensIterator.NextToken.TokenType == JsTokenTypes.Switch;
    }
}