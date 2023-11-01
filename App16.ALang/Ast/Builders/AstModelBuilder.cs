using App16.ALang.Ast.Errors;
using App16.ALang.Tokenizers;

namespace App16.ALang.Ast.Builders;

public class AstModelBuilder
{
    private readonly IAstNodeProcessor _nodeProcessor;
    private readonly string _sourceCodeString;
    private readonly AstTokensIterator _tokensIterator;
    
    public AstModelBuilder(
        IAstNodeProcessor nodeProcessor,
        string sourceCodeString, 
        List<Token> tokens)
    {
        if (string.IsNullOrEmpty(sourceCodeString))
        {
            throw new ArgumentException("input string is null or empty");
        }
        
        if (tokens == null)
            throw new ArgumentNullException(nameof(tokens));

        if (tokens.Count == 0)
            throw new ArgumentException("empty tokens list");

        _nodeProcessor = nodeProcessor;
        _sourceCodeString = sourceCodeString;
        _tokensIterator = new AstTokensIterator(tokens);
    }

    /// <summary>
    /// Build Ast Node from source code
    /// </summary>
    /// <exception cref="AstUnexpectedTokenError"></exception>
    /// <exception cref="AstUnexpectedEofError"></exception>
    /// <exception cref="AstInvalidBinaryOperation"></exception>
    /// <exception cref="AstInvalidTernaryOperation"></exception>
    /// <returns></returns>
    public IAstNode Build()
    {
        var context = new AstModelBuilderContext(_sourceCodeString, _tokensIterator);
        
        return _nodeProcessor.ProcessNext(context);
    }
}