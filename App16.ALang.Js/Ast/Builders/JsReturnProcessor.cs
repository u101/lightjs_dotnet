using App16.ALang.Ast;
using App16.ALang.Ast.Builders;
using App16.ALang.Js.Tokenizers;
using App16.ALang.Tokenizers;

namespace App16.ALang.Js.Ast.Builders;

public sealed class JsReturnProcessor : IAstNodeProcessor
{
    private readonly IAstNodeProcessor _expressionProcessor;

    public JsReturnProcessor(IAstNodeProcessor expressionProcessor)
    {
        _expressionProcessor = expressionProcessor;
    }
    
    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;
        
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.Return);
                
        var returnNodeToken = tokensIterator.CurrentToken;

        var returnExpression = AstEmptyNode.Instance;
                
        if (tokensIterator.HasNextToken &&
            ShouldProcessReturnStatementExpression(
                returnNodeToken, tokensIterator.NextToken))
        {
            returnExpression = _expressionProcessor.ProcessNext(context);
        }

        return new AstReturn(returnExpression, returnNodeToken);
    }
    
    private static bool ShouldProcessReturnStatementExpression(Token currentToken, Token nextToken)
    {
        if (nextToken.TokenType == JsTokenTypes.OpSemicolon) return false;
    
        if (nextToken.Position.Line == currentToken.Position.Line) return true;
    
        var nextType = nextToken.TokenType;

        if (nextType == JsTokenTypes.Function) return true;
    
        if (JsAstBuilderUtils.IsKeyword(nextType)) return false;

        return nextType == JsTokenTypes.Identifier ||
               nextType == JsTokenTypes.OpParenthesesOpen ||
               nextType == JsTokenTypes.OpSquareBracketsOpen ||
               nextType == JsTokenTypes.OpBracketOpen ||
               JsAstBuilderUtils.IsLiteral(nextType) ||
               JsAstBuilderUtils.IsPossiblyPrefixUnaryOperator(nextType);
    }
}

public sealed class JsReturnLookup : IForwardLookup
{
    public bool LookForward(AstTokensIterator tokensIterator)
    {
        return tokensIterator.NextToken.TokenType == JsTokenTypes.Return;
    }
}