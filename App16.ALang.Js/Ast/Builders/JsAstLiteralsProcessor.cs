using App16.ALang.Ast;
using App16.ALang.Ast.Builders;
using App16.ALang.Js.Tokenizers;
using App16.ALang.Tokenizers;

namespace App16.ALang.Js.Ast.Builders;

public sealed class JsAstLiteralsProcessor : IAstNodeProcessor
{

    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;

        tokensIterator.MoveForward();
        
        var token = tokensIterator.CurrentToken;
        
        switch (token.TokenType)
        {
            case JsTokenTypes.This: return new AstGetThis(token);
            
            case JsTokenTypes.True: return new AstValueLiteral<bool>(true, token);
            case JsTokenTypes.False: return new AstValueLiteral<bool>(false, token);
            
            case JsTokenTypes.IntDecimal:
                return new AstValueLiteral<int>(
                    JsTokenizerUtils.GetTokenDecimalIntValue(context.SourceCodeString, token), token);
            
            case JsTokenTypes.IntHex:
                return new AstValueLiteral<int>(
                    JsTokenizerUtils.GetTokenHexIntValue(context.SourceCodeString, token), token);
            
            case JsTokenTypes.IntBinary:
                return new AstValueLiteral<int>(
                    JsTokenizerUtils.GetTokenBinaryIntValue(context.SourceCodeString, token), token);
            
            case JsTokenTypes.Float:
            case JsTokenTypes.FloatE:
                return new AstValueLiteral<double>(
                    JsTokenizerUtils.GetTokenDoubleValue(context.SourceCodeString, token), token);
            
            case JsTokenTypes.FloatNaN:
                return new AstValueLiteral<double>(double.NaN);
            
            case JsTokenTypes.StringLiteral:
                return new AstValueLiteral<string>(
                    TokenizerUtils.GetTokenStringValue(context.SourceCodeString, token), token);
            
            case JsTokenTypes.Null:
                return new AstNull(token);
            
            case JsTokenTypes.Undefined:
                return new JsUndefined(token);
            
            default:
                throw new Exception($"invalid token type {token.TokenType}");
                
        }
    }
}

public sealed class JsAstLiteralsLookup : IForwardLookup
{
    public bool LookForward(AstTokensIterator tokensIterator)
    {
        var nextTokenType = tokensIterator.NextToken.TokenType;
        
        return nextTokenType == JsTokenTypes.IntDecimal ||
               nextTokenType == JsTokenTypes.IntHex ||
               nextTokenType == JsTokenTypes.IntBinary ||
               nextTokenType == JsTokenTypes.Float ||
               nextTokenType == JsTokenTypes.FloatE ||
               nextTokenType == JsTokenTypes.FloatNaN ||
               
               nextTokenType == JsTokenTypes.True ||
               nextTokenType == JsTokenTypes.False ||
               nextTokenType == JsTokenTypes.StringLiteral ||
               nextTokenType == JsTokenTypes.Null ||
               nextTokenType == JsTokenTypes.Undefined ||
               nextTokenType == JsTokenTypes.This;
    }
}