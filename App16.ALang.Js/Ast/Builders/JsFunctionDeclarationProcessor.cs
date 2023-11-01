using App16.ALang.Ast;
using App16.ALang.Ast.Builders;
using App16.ALang.Ast.Errors;
using App16.ALang.Js.Tokenizers;
using App16.ALang.Tokenizers;

namespace App16.ALang.Js.Ast.Builders;

public sealed class JsArrowFunctionDeclarationProcessor : IAstNodeProcessor
{
    private readonly JsFunctionDeclarationParametersProcessor _parametersProcessor;
    private readonly IAstNodeProcessor _blockInBracketsProcessor;
    private readonly IAstNodeProcessor _expressionProcessor;

    public JsArrowFunctionDeclarationProcessor(
        JsFunctionDeclarationParametersProcessor parametersProcessor,
        IAstNodeProcessor blockInBracketsProcessor,
        IAstNodeProcessor expressionProcessor)
    {
        _parametersProcessor = parametersProcessor;
        _blockInBracketsProcessor = blockInBracketsProcessor;
        _expressionProcessor = expressionProcessor;
    }

    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;
                
        var functionToken = tokensIterator.NextToken;

        var parameters = _parametersProcessor.Process(context);
        
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpArrow);
        
        IAstNode functionBody;

        if (tokensIterator.NextToken.TokenType == JsTokenTypes.OpBracketOpen)
        {
            functionBody = _blockInBracketsProcessor.ProcessNext(context);
        }
        else
        {
            var returnToken = tokensIterator.NextToken;
            var returnExpression = _expressionProcessor.ProcessNext(context);

            var returnNode = new AstReturn(returnExpression, returnToken);

            functionBody = returnNode;
        }
        
        return new JsAnonymousFunctionDeclaration(parameters, functionBody, functionToken);
    }
}

public sealed class JsArrowFunctionDeclarationLookup : IForwardLookup
{
    public bool LookForward(AstTokensIterator tokensIterator)
    {
        if (tokensIterator.NextToken.TokenType != JsTokenTypes.OpParenthesesOpen) return false;
        
        var stepsCount = 1;
        var prevTokenType = JsTokenTypes.OpParenthesesOpen;
        var stop = false;
        var result = false;
        
        while (!stop)
        {
            stepsCount++;
            
            var t = tokensIterator.LookForward(stepsCount);

            if (t.TokenType == JsTokenTypes.None) return false;
            
            var tokenType = t.TokenType;
            
            switch (prevTokenType)
            {
                case JsTokenTypes.OpParenthesesOpen:
                    // expecting ) or argument
                    stop = tokenType != JsTokenTypes.Identifier && tokenType != JsTokenTypes.OpParenthesesClose;
                    break;
                
                case JsTokenTypes.Identifier:
                    // expecting ) or ,
                    stop = tokenType != JsTokenTypes.OpComma && tokenType != JsTokenTypes.OpParenthesesClose;
                    break;
                case JsTokenTypes.OpComma:
                    // expecting argument
                    stop = tokenType != JsTokenTypes.Identifier;
                    break;
                case JsTokenTypes.OpParenthesesClose:
                    result = tokenType == JsTokenTypes.OpArrow;
                    stop = true;
                    break;
            }

            prevTokenType = tokenType;
        }

        return result;
    }
}

public sealed class JsAnonymousFunctionDeclarationProcessor : IAstNodeProcessor
{
    private readonly JsFunctionDeclarationParametersProcessor _parametersProcessor;
    private readonly IAstNodeProcessor _blockInBracketsProcessor;

    public JsAnonymousFunctionDeclarationProcessor(
        JsFunctionDeclarationParametersProcessor parametersProcessor,
        IAstNodeProcessor blockInBracketsProcessor)
    {
        _parametersProcessor = parametersProcessor;
        _blockInBracketsProcessor = blockInBracketsProcessor;
    }

    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;
        
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.Function);
                
        var functionToken = tokensIterator.CurrentToken;

        var parameters = _parametersProcessor.Process(context);
        
        var functionBody = _blockInBracketsProcessor.ProcessNext(context);
        
        return new JsAnonymousFunctionDeclaration(parameters, functionBody, functionToken);
    }
}

public sealed class JsAnonymousFunctionDeclarationLookup : IForwardLookup
{
    public bool LookForward(AstTokensIterator tokensIterator)
    {
        return tokensIterator.LookForward(1).TokenType == JsTokenTypes.Function;
    }
} 

public sealed class JsNamedFunctionDeclarationProcessor : IAstNodeProcessor
{
    private readonly JsFunctionDeclarationParametersProcessor _parametersProcessor;
    private readonly IAstNodeProcessor _blockInBracketsProcessor;

    public JsNamedFunctionDeclarationProcessor(
        JsFunctionDeclarationParametersProcessor parametersProcessor,
        IAstNodeProcessor blockInBracketsProcessor)
    {
        _parametersProcessor = parametersProcessor;
        _blockInBracketsProcessor = blockInBracketsProcessor;
    }

    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;
        
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.Function);
                
        var functionToken = tokensIterator.CurrentToken;
                
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.Identifier);

        var funcName = TokenizerUtils.GetTokenStringValue(
            context.SourceCodeString, tokensIterator.CurrentToken);

        var parameters = _parametersProcessor.Process(context);
        
        var functionBody = _blockInBracketsProcessor.ProcessNext(context);
        
        return new JsNamedFunctionDeclaration(funcName, parameters, functionBody, functionToken);
    }
}

public sealed class JsNamedFunctionLookup : IForwardLookup
{
    public bool LookForward(AstTokensIterator tokensIterator)
    {
        return tokensIterator.LookForward(1).TokenType == JsTokenTypes.Function &&
               tokensIterator.LookForward(2).TokenType == JsTokenTypes.Identifier;
    }
}

public sealed class JsFunctionDeclarationParametersProcessor
{
    private readonly IAstNodeProcessor _literalsProcessor;
    private readonly List<JsFunctionDeclarationParameter> _functionDeclarationParameters = new();

    public JsFunctionDeclarationParametersProcessor(IAstNodeProcessor literalsProcessor)
    {
        _literalsProcessor = literalsProcessor;
    }
    
    public JsFunctionDeclarationParameter[] Process(AstModelBuilderContext context)
    {
        _functionDeclarationParameters.Clear();
        
        var tokensIterator = context.TokensIterator;
        
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpParenthesesOpen);

        ProcessFunctionDeclarationArguments(context, _functionDeclarationParameters);
        
        tokensIterator.CheckEarlyEof();
        
        tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpParenthesesClose);

        var parametersCount = _functionDeclarationParameters.Count;

        if (parametersCount == 0) 
            return Array.Empty<JsFunctionDeclarationParameter>();

        var parameters = new JsFunctionDeclarationParameter[parametersCount];

        for (var i = 0; i < parameters.Length; i++)
        {
            parameters[i] = _functionDeclarationParameters[i];
        }

        return parameters;
    }
    
    private void ProcessFunctionDeclarationArguments(
        AstModelBuilderContext context,
        ICollection<JsFunctionDeclarationParameter> functionDeclarationParameters)
    {
        var tokensIterator = context.TokensIterator;
        
        while (tokensIterator.HasNextToken && 
               tokensIterator.NextToken.TokenType != JsTokenTypes.OpParenthesesClose)
        {
            tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.Identifier);

            var argNameToken = tokensIterator.CurrentToken;
            var defaultValue = AstEmptyNode.Instance;

            if (argNameToken.TokenType != JsTokenTypes.Identifier)
                throw new AstUnexpectedTokenError(JsTokenTypes.Identifier, argNameToken);

            if (tokensIterator.NextToken.TokenType == JsTokenTypes.OpAssign)
            {
                tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpAssign);
                
                if (!JsAstBuilderUtils.IsLiteral(tokensIterator.NextToken.TokenType))
                {
                    throw new AstUnexpectedTokenError(tokensIterator.NextToken);
                }

                defaultValue = _literalsProcessor.ProcessNext(context);
            }
            
            functionDeclarationParameters.Add(new JsFunctionDeclarationParameter(
                TokenizerUtils.GetTokenStringValue(context.SourceCodeString, argNameToken),
                defaultValue
            ));

            if (tokensIterator.NextToken.TokenType == JsTokenTypes.OpComma)
            {
                tokensIterator.CheckExpectedNextAndMoveForward(JsTokenTypes.OpComma);
            }
            else if (tokensIterator.NextToken.TokenType == JsTokenTypes.OpParenthesesClose)
            {
                break; // arguments section finish
            }
            else
            {
                throw new AstUnexpectedTokenError(JsTokenTypes.OpParenthesesClose, tokensIterator.NextToken);
            }
        }
    }
}

