namespace App16.ALang.Ast.Builders;

public interface IDecoratorForwardLookup
{
    bool LookForward(IAstNode decoratee, AstTokensIterator tokensIterator);
}