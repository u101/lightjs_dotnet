namespace App16.ALang.Tokenizers;

public class TokensCompositionMap
{

    private readonly Dictionary<OpCompositionKey, int> _map = new();
    
    public TokensCompositionMap() {}

    public TokensCompositionMap(params (int, int, int)[] valueTuples)
    {
        foreach (var (firstTokenType, nextTokenType, resultType) in valueTuples)
        {
            _map[new OpCompositionKey(firstTokenType, nextTokenType)] = resultType;
        }
        
    }

    public void Add(int firstTokenType, int nextTokenType, int resultType)
    {
        _map[new OpCompositionKey(firstTokenType, nextTokenType)] = resultType;
    }

    public bool TryGetComposition(int firstTokenType, int nextTokenType, out int resultType)
    {
        var key = new OpCompositionKey(firstTokenType, nextTokenType);
        return _map.TryGetValue(key, out resultType);
    }
    
    private readonly struct OpCompositionKey : IEquatable<OpCompositionKey>
    {
        public int FirstTokenType { get; }
        public int NextTokenType { get; }

        public OpCompositionKey(int firstTokenType, int nextTokenType)
        {
            FirstTokenType = firstTokenType;
            NextTokenType = nextTokenType;
        }

        public bool Equals(OpCompositionKey other)
        {
            return FirstTokenType.Equals(other.FirstTokenType) && NextTokenType.Equals(other.NextTokenType);
        }

        public override bool Equals(object? obj)
        {
            return obj is OpCompositionKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FirstTokenType, NextTokenType);
        }
    }
}