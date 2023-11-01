namespace App16.ALang.Ast.Builders;

public interface IAstNodeProcessor
{
    
    IAstNode ProcessNext(AstModelBuilderContext context);
}