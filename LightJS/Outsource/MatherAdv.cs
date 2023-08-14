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
        Parentheses = 1 << 4,
        FunctionCall = 1 << 5
    }

    private class Op : ILjsAstNode
    {
        public LjsTokenType TokenType => Token.TokenType;
        public LjsToken Token { get; }
        public OpType OpType { get; }
        public bool IsUnary => 
            ((this.OpType & OpType.UnaryPrefix) | (this.OpType & OpType.UnaryPostfix)) != 0;
        
        public bool IsUnaryPrefix => (this.OpType & OpType.UnaryPrefix) != 0;
        public bool IsUnaryPostfix => (this.OpType & OpType.UnaryPostfix) != 0;
        public bool IsBinary => (this.OpType & OpType.Binary) != 0;
        public bool IsAssign => (this.OpType & OpType.Assign) != 0;
        public int Priority { get; }

        public Op(LjsToken token, OpType opType, int priority)
        {
            Token = token;
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
        // TODO function call
        // TODO ternary opertaor .. ? .. : ..
        // TODO dot prop access
        
        var postfixExpr = new List<ILjsAstNode>();
        var stack = new Stack<Op>();

        var prevToken = default(LjsToken);

        var parenthesesCount = 0;
        
        
        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            
            if (token.TokenType == LjsTokenType.Identifier)
            {
                postfixExpr.Add(new LjsAstGetVar(
                    sourceCodeString.Substring(token.Position.CharIndex, token.StringLength)));
            }
            else if (LjsAstBuilderUtils.IsLiteral(token.TokenType))
            {
                postfixExpr.Add( LjsAstBuilderUtils.CreateLiteralNode(token, sourceCodeString));
            }
            
            else if (token.TokenType == LjsTokenType.OpParenthesesOpen)
            {
                var opType = OpType.Parentheses;
                
                if (IsFunctionCall(prevToken.TokenType))
                {
                    opType |= OpType.FunctionCall;
                }
                
                stack.Push(new Op(token, opType, 0));
                ++parenthesesCount;
            }
            
            else if (token.TokenType == LjsTokenType.OpParenthesesClose)
            {
                while (stack.Count > 0 && stack.Peek().TokenType != LjsTokenType.OpParenthesesOpen)
                {
                    postfixExpr.Add(stack.Pop()); 
                }
                
                stack.Pop(); // remove opening parentheses from stack
                --parenthesesCount;
            }
            else if (LjsAstBuilderUtils.IsOrdinaryOperator(token.TokenType))
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
                
                while (stack.Count > 0 && (stack.Peek().Priority >= opPriority))
                    postfixExpr.Add(stack.Pop());
                
                stack.Push(new Op(token, opType, opPriority));
            }

            prevToken = token;
        }

        // check unclosed groups
        
        if (parenthesesCount > 0)
        {
            foreach (var op in stack)
            {
                if (op.TokenType == LjsTokenType.OpParenthesesOpen)
                {
                    throw new LjsSyntaxError("unclosed parentheses", op.Token.Position);
                }
            }

            throw new LjsSyntaxError("unclosed parentheses");
        }
        
        while (stack.Count > 0)
        {
            postfixExpr.Add(stack.Pop());
        }
        
        // --------- CREATE NODES
        
        var locals = new Stack<ILjsAstNode>();

        
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

                    if (op.TokenType == LjsTokenType.OpDot)
                    {
                        // convert get var node into get named property
                        if (right is LjsAstGetVar getVar)
                        {
                            locals.Push(new LjsAstGetNamedProperty(getVar.VarName, left));
                        }
                        else
                        {
                            throw new LjsSyntaxError("invalid property access", op.Token.Position);
                        }
                    }

                    else if (op.IsAssign)
                    {
                        switch (left)
                        {
                            case LjsAstGetVar getVar:
                                locals.Push(new LjsAstSetVar(getVar.VarName, right, LjsAstBuilderUtils.GetAssignMode(op.TokenType)));
                                break;
                            case LjsAstGetNamedProperty getProp:
                                locals.Push(new LjsAstSetNamedProperty(
                                    getProp.PropertyName, getProp.PropertySource, 
                                    right, LjsAstBuilderUtils.GetAssignMode(op.TokenType)));
                                break;
                            default:
                                throw new LjsSyntaxError("invalid assign", op.Token.Position);
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

    private static bool IsFunctionCall(LjsTokenType prevTokenType) => prevTokenType is
        LjsTokenType.Identifier or
        LjsTokenType.OpParenthesesClose or
        LjsTokenType.OpSquareBracketsClose;

}