using App16.ALang.Ast.Errors;
using App16.ALang.Tokenizers;

namespace App16.ALang.Ast.Builders;

public sealed class AstTokensIterator
{
    private readonly List<Token> _tokens;

    private int _currentIndex = -1;
    
    public AstTokensIterator(List<Token> tokens)
    {
        _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
    }

    public int CurrentIndex => _currentIndex;
    
    public Token CurrentToken => 
        _currentIndex >= 0 && _currentIndex < _tokens.Count ? 
            _tokens[_currentIndex] : default;

    public Token NextToken => 
        _currentIndex + 1 < _tokens.Count ? 
            _tokens[_currentIndex + 1] : default;

    public Token PrevToken => 
        _currentIndex > 0 ?
            _tokens[_currentIndex - 1] : default;

    public bool HasNextToken => _currentIndex + 1 < _tokens.Count;

    public Token LookForward(int offset)
    {
        if (offset < 1)
            throw new ArgumentException($"invalid offset = {offset}. should be >= 1");
        
        var i = _currentIndex + offset;

        return i < _tokens.Count ? _tokens[i] : default;
    }
    
    public void MoveForward()
    {
        if (!HasNextToken)
        {
            throw new IndexOutOfRangeException();
        }
        
        ++_currentIndex;
    }
        
    public void MoveBackward(int stepsCount = 1)
    {
        if (stepsCount < 1)
            throw new ArgumentException($"invalid staeps count {stepsCount}");
            
        if (_currentIndex - stepsCount < 0)
        {
            throw new IndexOutOfRangeException();
        }
        
        _currentIndex -= stepsCount;
    }
    
    // ----------
    
    public void CheckEarlyEof()
    {
        if (!HasNextToken)
        {
            throw new AstUnexpectedEofError(CurrentToken);
        }
    }
    
    public void SkipTokens(int tokenType)
    {
        while (HasNextToken &&
               NextToken.TokenType == tokenType)
        {
            CheckExpectedNextAndMoveForward(tokenType);
        }
    }
    
    public void CheckExpectedNextAndMoveForward(int tokenType)
    {
        CheckExpectedNext(tokenType);
        
        MoveForward();
    }
    
    public void CheckExpectedNext(int tokenType)
    {
        CheckEarlyEof();
        
        if (NextToken.TokenType != tokenType)
        {
            throw new AstUnexpectedTokenError(tokenType, NextToken);
        }
    }

    public bool IfNextMoveForward(int nextTokenType)
    {
        if (HasNextToken && NextToken.TokenType == nextTokenType)
        {
            MoveForward();
            return true;
        }

        return false;
    }
}