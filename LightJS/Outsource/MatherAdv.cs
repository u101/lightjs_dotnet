using LightJS.Ast;
using LightJS.Tokenizer;

namespace LightJS.Outsource;

public static class MatherAdv
{
    
    [Flags]
    private enum OpType
    {
        None = 0,
        Binary = 1 << 0,
        UnaryPrefix = 1 << 1,
        UnaryPostfix = 1 << 2,
        Assign = 1 << 3,
        Parentheses = 1 << 4
    }

    private class Op : ILjsAstNode
    {
        public LjsTokenType TokenType { get; }
        public OpType OpType { get; }
        public bool IsUnary => 
            ((this.OpType & OpType.UnaryPrefix) | (this.OpType & OpType.UnaryPostfix)) != 0;
        
        public bool IsUnaryPrefix => (this.OpType & OpType.UnaryPrefix) != 0;
        public bool IsUnaryPostfix => (this.OpType & OpType.UnaryPostfix) != 0;
        public bool IsBinary => (this.OpType & OpType.Binary) != 0;
        public bool IsAssign => (this.OpType & OpType.Assign) != 0;
        public int Priority { get; }

        public Op(LjsTokenType tokenType, OpType opType, int priority)
        {
            TokenType = tokenType;
            OpType = opType;
            Priority = priority;
        }
    }

    public static ILjsAstNode Convert(string sourceCodeString)
    {
        var ljsTokenizer = new LjsTokenizer(sourceCodeString);
        var tokens = ljsTokenizer.ReadTokens();
        return Convert(tokens, sourceCodeString);
    }
    

    public static ILjsAstNode Convert(List<LjsToken> tokens, string sourceCodeString)
    {
        //	Выходная строка, содержащая постфиксную запись
        var postfixExpr = new List<ILjsAstNode>();
        //	Инициализация стека, содержащий операторы в виде символов
        var stack = new Stack<Op>();

        var prevToken = default(LjsToken);
        
        //	Перебираем строку
        for (var i = 0; i < tokens.Count; i++)
        {
            //	Текущий символ
            var token = tokens[i];

            //	Если симовол - цифра
            if (token.TokenType == LjsTokenType.Identifier)
            {
                postfixExpr.Add(new LjsAstGetVar(
                    sourceCodeString.Substring(token.Position.CharIndex, token.StringLength)));
            }
            else if (LjsAstBuilderUtils.IsLiteral(token.TokenType))
            {
                postfixExpr.Add( LjsAstBuilderUtils.CreateLiteralNode(token, sourceCodeString));
            }
            
            //	Если открывающаяся скобка 
            else if (token.TokenType == LjsTokenType.OpParenthesesOpen)
            {
                //	Заносим её в стек
                
                stack.Push(new Op(token.TokenType, OpType.Parentheses, 0));
            }
            //	Если закрывающая скобка
            else if (token.TokenType == LjsTokenType.OpParenthesesClose)
            {
                //	Заносим в выходную строку из стека всё вплоть до открывающей скобки
                while (stack.Count > 0 && stack.Peek().TokenType != LjsTokenType.OpParenthesesOpen)
                    postfixExpr.Add(stack.Pop()); 
                //	Удаляем открывающуюся скобку из стека
                stack.Pop();
            }
            //	Проверяем, содержится ли символ в списке операторов
            else if (LjsAstBuilderUtils.IsCalculationOperator(token.TokenType))
            {
                var isUnaryPrefix = LjsAstBuilderUtils.CanBeUnaryPrefixOp(token.TokenType) &&
                                    (prevToken.TokenType == LjsTokenType.None ||
                                     LjsAstBuilderUtils.IsBinaryOp(prevToken.TokenType) ||
                                     prevToken.TokenType == LjsTokenType.OpParenthesesOpen);
                var isUnaryPostfix = !isUnaryPrefix &&
                                     LjsAstBuilderUtils.CanBeUnaryPostfixOp(token.TokenType) && 
                              (prevToken.TokenType == LjsTokenType.OpParenthesesClose ||
                               prevToken.TokenType == LjsTokenType.OpSquareBracketsClose ||
                               prevToken.TokenType == LjsTokenType.Identifier);

                var isUnary = isUnaryPrefix || isUnaryPostfix;

                var opType = OpType.None;
                if (isUnaryPrefix) opType |= OpType.UnaryPrefix;
                if (isUnaryPostfix) opType |= OpType.UnaryPostfix;
                if (!isUnary)
                {
                    opType |= OpType.Binary;
                    if (LjsAstBuilderUtils.IsAssignOperator(token.TokenType))
                    {
                        opType |= OpType.Assign;
                    }
                }
                
                // todo check isUnary equals isDefinitelyUnary() method and throw error if needed

                var opPriority = LjsAstBuilderUtils.GetOperatorPriority(token.TokenType, isUnary);

                //	Заносим в выходную строку все операторы из стека, имеющие более высокий приоритет
                while (stack.Count > 0 && (stack.Peek().Priority >= opPriority))
                    postfixExpr.Add(stack.Pop());
                
                //	Заносим в стек оператор
                stack.Push(new Op(token.TokenType, opType, opPriority));
            }

            prevToken = token;
        }

        //	Заносим все оставшиеся операторы из стека в выходную строку
        while (stack.Count > 0)
        {
            postfixExpr.Add(stack.Pop());
        }
        
        // --------- CREATE NODES
        
        var locals = new Stack<ILjsAstNode>();

        //	Проходим по строке
        for (var i = 0; i < postfixExpr.Count; i++)
        {
            var node = postfixExpr[i];
            
            if (node is Op op)
            {

                if (op.IsUnary)
                {
                    var operand = locals.Pop();
                    locals.Push(new LjsAstUnaryOperation(
                        operand, LjsAstBuilderUtils.GetUnaryOperationType(op.TokenType, op.IsUnaryPrefix)));
                }
                else
                {
                    var right = locals.Pop();
                    var left = locals.Pop();

                    if (op.IsAssign)
                    {
                        if (left is LjsAstGetVar getVar)
                        {
                            locals.Push(new LjsAstSetVar(getVar.VarName, right, LjsAstBuilderUtils.GetAssignMode(op.TokenType)));
                        }
                        else
                        {
                            throw new Exception("invalid assign");
                        }
                    }
                    else
                    {
                        locals.Push(new LjsAstBinaryOperation(
                            left, right, LjsAstBuilderUtils.GetBinaryOperationType(op.TokenType)));
                    }
                    
                    
                }
                
            }
            else
            {
                locals.Push(node);
            }
        }
        
        return locals.Pop();

        
    }
    
}