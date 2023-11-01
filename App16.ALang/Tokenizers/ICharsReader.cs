namespace App16.ALang.Tokenizers;

public interface ICharsReader
{
    char CurrentChar { get; }
    
    int CurrentIndex { get; }
    
    char NextChar { get; }
    
    bool HasNextChar { get; }
    char PrevChar { get; }

    bool CanLookForward(int offset);

    char LookForward(int offset);
    
    void MoveForward();
    
    TokenPosition CurrentTokenPosition { get; }
}