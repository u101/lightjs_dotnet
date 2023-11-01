using App16.ALang.Ast.Builders;
using App16.ALang.Js.Tokenizers;

namespace App16.ALang.Js.Ast.Builders;

public sealed class JsStopPointAutoSemicolon : IAstProcessorStopPoint
{
    public static readonly IAstProcessorStopPoint WithPreviousCheck = new JsStopPointAutoSemicolon(true);
    public static readonly IAstProcessorStopPoint WithoutPreviousCheck = new JsStopPointAutoSemicolon(false);
    
    public bool CheckPreviousPoint { get; }

    private JsStopPointAutoSemicolon(bool checkPreviousPoint)
    {
        CheckPreviousPoint = checkPreviousPoint;
    }

    public bool ShouldStop(AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;

        var currentToken = tokensIterator.CurrentToken;
        var nextToken = tokensIterator.NextToken;

        if (nextToken.TokenType == JsTokenTypes.OpSemicolon) return true;

        if (nextToken.Position.Line == currentToken.Position.Line) return false;

        var currentType = currentToken.TokenType;
        var nextType = nextToken.TokenType;
        
        if (JsAstBuilderUtils.IsKeyword(nextType)) return true;

        return (currentType == JsTokenTypes.Identifier ||
                currentType == JsTokenTypes.OpParenthesesClose ||
                currentType == JsTokenTypes.OpSquareBracketsClose ||
                currentType == JsTokenTypes.OpBracketClose ||
                JsAstBuilderUtils.IsLiteral(currentType)) &&
               (nextType == JsTokenTypes.Identifier ||
                JsAstBuilderUtils.IsLiteral(nextType) ||
                JsAstBuilderUtils.IsDefinitelyPrefixUnaryOperator(nextType));
    }

    
}