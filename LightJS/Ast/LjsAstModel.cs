using LightJS.Tokenizer;

namespace LightJS.Ast;

public sealed class LjsAstModel
{
    private readonly Dictionary<ILjsAstNode, LjsToken> _tokensMap;
    public ILjsAstNode RootNode { get; }

    public LjsAstModel(ILjsAstNode rootNode, Dictionary<ILjsAstNode, LjsToken> tokensMap)
    {
        _tokensMap = tokensMap;
        RootNode = rootNode;
    }

    public LjsToken GetTokenForNode(ILjsAstNode node) =>
        _tokensMap.TryGetValue(node, out var token) ? token : default;

    public bool HasTokenForNode(ILjsAstNode node) => 
        _tokensMap.ContainsKey(node);

}