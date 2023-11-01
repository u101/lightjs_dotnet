namespace App16.ALang.Tokenizers;

public interface ITokensProcessor
{

    /// <summary>
    /// returns true if processed, false otherwise
    /// </summary>
    public bool Process(TokenizerContext ctx);

}