using App16.ALang.Ast;
using App16.ALang.Ast.Builders;
using App16.ALang.Js.Tokenizers;
using App16.ALang.Tokenizers;

namespace App16.ALang.Js.Ast.Builders;

public sealed class JsVarDeclarationProcessor : IAstNodeProcessor
{
    private readonly IAstNodeProcessor _expressionsProcessor;

    public JsVarDeclarationProcessor(IAstNodeProcessor expressionsProcessor)
    {
        _expressionsProcessor = expressionsProcessor;
    }
    
    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;

        tokensIterator.MoveForward();
        
        var variableKind = GetVariableDeclarationKind(tokensIterator.CurrentToken.TokenType);
        
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.Identifier);
        
        var firstVarNameToken = tokensIterator.CurrentToken;
        var firstVarValue = AstEmptyNode.Instance;
        
        context.PushStopPoint(JsStopPoints.OptionalComma);
        
        if (tokensIterator.IfNextMoveForward(JsTokenTypes.OpAssign))
        {
            firstVarValue = _expressionsProcessor.ProcessNext(context);
        }
        
        var firstVar = new JsVariableDeclaration(
            TokenizerUtils.GetTokenStringValue(context.SourceCodeString, firstVarNameToken),
            firstVarValue, variableKind, firstVarNameToken);
        
        if (tokensIterator.NextToken.TokenType != JsTokenTypes.OpComma)
        {
            context.PopStopPoint();
            return firstVar;
        }

        var seq = new AstSequence();
        seq.AddNode(firstVar);
        
        while (tokensIterator.NextToken.TokenType == JsTokenTypes.OpComma)
        {
            tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpComma);
            
            tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.Identifier);
            
            var nextVarToken = tokensIterator.CurrentToken;
            var nextVarValue = AstEmptyNode.Instance;
            
            if (tokensIterator.NextToken.TokenType == JsTokenTypes.OpAssign)
            {
                tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpAssign);
                
                nextVarValue = _expressionsProcessor.ProcessNext(context);
            }
            
            var nextVar = new JsVariableDeclaration(
                TokenizerUtils.GetTokenStringValue(context.SourceCodeString, nextVarToken),
                nextVarValue, variableKind, nextVarToken);
            
            seq.AddNode(nextVar);
        }
        
        context.PopStopPoint();

        return seq;
    }
    
    private static JsVariableKind GetVariableDeclarationKind(int tokenType) => tokenType switch
    {
        JsTokenTypes.Var => JsVariableKind.Var,
        JsTokenTypes.Let => JsVariableKind.Let,
        JsTokenTypes.Const => JsVariableKind.Const,
        _ => throw new Exception($"invalid variable declaration token type {tokenType}")
    };
}

public sealed class JsVarDeclarationLookup : IForwardLookup
{
    public bool LookForward(AstTokensIterator tokensIterator)
    {
        var nextTokenType = tokensIterator.NextToken.TokenType;

        return nextTokenType == JsTokenTypes.Var ||
               nextTokenType == JsTokenTypes.Let ||
               nextTokenType == JsTokenTypes.Const;
    }
}