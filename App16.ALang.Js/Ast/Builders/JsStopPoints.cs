using App16.ALang.Ast.Builders;
using App16.ALang.Js.Tokenizers;

namespace App16.ALang.Js.Ast.Builders;

public static class JsStopPoints
{
    public static readonly IAstProcessorStopPoint ParenthesesClose =
        new AstStopPointBeforeToken(JsTokenTypes.OpParenthesesClose, false);
    
    public static readonly IAstProcessorStopPoint Colon =
        new AstStopPointBeforeToken(JsTokenTypes.OpColon, false);
    
    public static readonly IAstProcessorStopPoint BracketClose =
        new AstStopPointBeforeToken(JsTokenTypes.OpBracketClose, false);
    
    public static readonly IAstProcessorStopPoint SquareBracketClose =
        new AstStopPointBeforeToken(JsTokenTypes.OpSquareBracketsClose, false);
    
    public static readonly IAstProcessorStopPoint OptionalComma = 
        new AstStopPointBeforeToken(JsTokenTypes.OpComma, true);
    
    public static readonly IAstProcessorStopPoint Semicolon = 
        new AstStopPointBeforeToken(JsTokenTypes.OpSemicolon, false);
}