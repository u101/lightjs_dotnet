namespace App16.ALang.Tokenizers;

public sealed class TokenizerFactory
{
    private readonly IReadOnlyList<ITokensProcessor> _processors;

    public TokenizerFactory(IReadOnlyList<ITokensProcessor> processors)
    {
        _processors = processors;
    }

    public Tokenizer CreateTokenizer(string sourceCodeString) => new(sourceCodeString, _processors);

}