namespace App16.ALang.Ast.Builders;

public interface IForwardLookup
{
    bool LookForward(AstTokensIterator tokensIterator);
}