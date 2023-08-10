using LightJS.Tokenizer;

namespace LightJS.Ast;

public class LjsAstModel
{
    private readonly Dictionary<ILjsAstNode, LjsTokenPosition> _tokenPositionsMap;
    public ILjsAstNode RootNode { get; }

    public LjsAstModel(ILjsAstNode rootNode, Dictionary<ILjsAstNode, LjsTokenPosition> tokenPositionsMap)
    {
        _tokenPositionsMap = tokenPositionsMap;
        RootNode = rootNode;
    }

    public LjsTokenPosition GetTokenPositionForNode(ILjsAstNode node) =>
        _tokenPositionsMap.TryGetValue(node, out var p) ? p : default;

    public bool HasTokenPositionForNode(ILjsAstNode node) => 
        _tokenPositionsMap.ContainsKey(node);

}