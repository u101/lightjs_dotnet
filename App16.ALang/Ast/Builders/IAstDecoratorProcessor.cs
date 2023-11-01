namespace App16.ALang.Ast.Builders;

public interface IAstDecoratorProcessor
{
    IAstNode ProcessNext(IAstNode decoratee, AstModelBuilderContext context);
}