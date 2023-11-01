using App16.ALang.Ast.Errors;
using App16.ALang.Tokenizers;

namespace App16.ALang.Ast.Builders;

public class AstExpressionProcessor : IAstNodeProcessor
{

    private readonly AstProcessorRecord[] _nodeProcessors;
    private readonly AstDecoratorRecord[] _nodeDecorators;
    private readonly Dictionary<int, AstBinaryOperationInfo> _binaryOperatorsMap;
    private readonly Dictionary<int, AstUnaryOperationInfo> _unaryOperatorsMap;
    private readonly AstTernaryOperationInfo _ternaryOperationInfo;


    private readonly List<IAstNode> _postfixExpression = new();
    private readonly Stack<OperationData> _operatorsStack = new();
    private readonly Stack<IAstNode> _locals = new();

    private readonly Stack<UnaryOperationData> _prefixUnaryOps = new();
    
    public AstExpressionProcessor(
        AstProcessorRecord[] nodeProcessors,
        AstDecoratorRecord[] nodeDecorators,
        Dictionary<int,AstBinaryOperationInfo> binaryOperatorsMap,
        Dictionary<int, AstUnaryOperationInfo> unaryOperatorsMap,
        AstTernaryOperationInfo ternaryOperationInfo)
    {
        _nodeProcessors = nodeProcessors;
        _nodeDecorators = nodeDecorators;
        _binaryOperatorsMap = binaryOperatorsMap;
        _unaryOperatorsMap = unaryOperatorsMap;
        _ternaryOperationInfo = ternaryOperationInfo;
    }
    
    public IAstNode ProcessNext(AstModelBuilderContext context)
    {
        var tokensIterator = context.TokensIterator;
        
        tokensIterator.CheckEarlyEof();
        
        var operatorsStackStartingLn = _operatorsStack.Count;
        var postfixExpressionStartingLn = _postfixExpression.Count;
        var prefixUnaryOpsStartingLn = _prefixUnaryOps.Count;
        
        var lastNodeKind = NodeKind.None;
        var isFirstExpressionElement = true;

        while (isFirstExpressionElement || !context.ShouldStopAtCurrentPoint())
        {
            isFirstExpressionElement = false;
            
            var shouldFinishIteration = false;

            // process operands (value nodes like identifiers, literals, etc..)
            for (var i = 0; lastNodeKind != NodeKind.Operand && !shouldFinishIteration && i < _nodeProcessors.Length; ++i)
            {
                var p = _nodeProcessors[i];

                if (!p.Lookup.LookForward(tokensIterator)) continue;
                
                var node = p.Processor.ProcessNext(context);
                
                lastNodeKind = NodeKind.Operand;
                
                _postfixExpression.Add(node);
                shouldFinishIteration = true;
            }
            
            if (shouldFinishIteration) continue;

            // process operand decorators (like dot properties, sqb properties, func calls, etc)
            if (lastNodeKind == NodeKind.Operand)
            {
                var lastNode = _postfixExpression[^1];
                
                for (var i = 0; !shouldFinishIteration && i < _nodeDecorators.Length; ++i)
                {
                    var p = _nodeDecorators[i];

                    if (!p.Lookup.LookForward(lastNode, tokensIterator)) continue;
                
                    var node = p.Processor.ProcessNext(lastNode, context);
                
                    lastNodeKind = NodeKind.Operand;

                    _postfixExpression[^1] = node;
                    shouldFinishIteration = true;
                }
            }
            
            if (shouldFinishIteration) continue;
            
            tokensIterator.CheckEarlyEof();

            // apply prefix unary operators to last operand
            if (lastNodeKind == NodeKind.Operand && 
                _prefixUnaryOps.Count > prefixUnaryOpsStartingLn)
            {
                ApplyPrefixUnaryOperators(_postfixExpression, _prefixUnaryOps, prefixUnaryOpsStartingLn);
            }

            var nextToken = tokensIterator.NextToken;

            // check ternary operator first
            if (nextToken.TokenType == _ternaryOperationInfo.TokenType)
            {
                if (_prefixUnaryOps.Count > prefixUnaryOpsStartingLn || 
                    lastNodeKind != NodeKind.Operand)
                    throw new AstUnexpectedTokenError(nextToken);
                
                tokensIterator.MoveForward();

                FlushLeftToRightOperators(operatorsStackStartingLn);
                
                context.PushStopPoint(_ternaryOperationInfo.DelimiterStopPoint);
                
                var trueExpressionNode = ProcessNext(context);
                
                context.PopStopPoint();
                
                tokensIterator.CheckExpectedNextAndMoveForward(_ternaryOperationInfo.DelimiterTokenType);
                
                var falseExpressionNode = ProcessNext(context);

                _postfixExpression.Add(trueExpressionNode);
                _postfixExpression.Add(falseExpressionNode);
                
                PushOperatorToStack(
                    new TernaryOperationData(nextToken, _ternaryOperationInfo.OperationPriority), 
                    operatorsStackStartingLn);
            }
            
            // check unary operators first
            else if (_unaryOperatorsMap.TryGetValue(nextToken.TokenType, out var unaryOperationInfo) &&
                IsUnaryOperatorInAppropriatePosition(unaryOperationInfo, lastNodeKind))
            {
                tokensIterator.MoveForward();
                
                var isInPrefixPosition = lastNodeKind == NodeKind.None || 
                                       lastNodeKind == NodeKind.Operator;


                if (isInPrefixPosition)
                {
                    _prefixUnaryOps.Push(new UnaryOperationData(unaryOperationInfo, nextToken));
                }
                else
                {
                    var unaryOpTarget = _postfixExpression[^1];
                    
                    if (unaryOpTarget is AstUnaryOperation)
                    {
                        throw new AstUnexpectedTokenError(nextToken);
                    }

                    _postfixExpression[^1] = 
                        new AstUnaryOperation(unaryOpTarget, unaryOperationInfo, false, nextToken);
                }
            }
            

            else if (_binaryOperatorsMap.TryGetValue(nextToken.TokenType, out var operationInfo))
            {
                if (_prefixUnaryOps.Count > prefixUnaryOpsStartingLn)
                    throw new AstUnexpectedTokenError(nextToken);
                
                tokensIterator.MoveForward();

                var op = new BinaryOperationData(operationInfo, nextToken);
                
                lastNodeKind = NodeKind.Operator;
                
                PushOperatorToStack(op, operatorsStackStartingLn);
            }
            else
            {
                throw new AstUnexpectedTokenError(nextToken);
            }
        }

        if (_prefixUnaryOps.Count > prefixUnaryOpsStartingLn)
        {
            if (lastNodeKind == NodeKind.Operand)
            {
                ApplyPrefixUnaryOperators(_postfixExpression, _prefixUnaryOps, prefixUnaryOpsStartingLn);
            }
            else
            {
                throw new AstUnexpectedTokenError(_prefixUnaryOps.Peek().Token);
            }
        }
            
        
        while (_operatorsStack.Count > operatorsStackStartingLn)
        {
            _postfixExpression.Add(_operatorsStack.Pop());
        }
        
        _locals.Clear();

        for (var i = postfixExpressionStartingLn; i < _postfixExpression.Count; i++)
        {
            var node = _postfixExpression[i];

            switch (node)
            {
                case TernaryOperationData terOp when _locals.Count < 3:
                    throw new AstInvalidTernaryOperation(_ternaryOperationInfo, terOp.GetToken());
                
                case TernaryOperationData ternaryIfOp:
                {
                    var falseExpression = _locals.Pop();
                    var trueExpression = _locals.Pop();
                    var condition = _locals.Pop();
                
                    var ternaryIfOperation = new AstTernaryIfOperation(
                        condition, trueExpression, falseExpression, ternaryIfOp.GetToken());
                
                    _locals.Push(ternaryIfOperation);
                    break;
                }
                
                case BinaryOperationData binOp when _locals.Count < 2:
                    throw new AstInvalidBinaryOperation(binOp.OperationInfo, binOp.GetToken());
                
                case BinaryOperationData operationNode:
                {
                    var right = _locals.Pop();
                    var left = _locals.Pop();
                
                
                    var binaryOperation = new AstBinaryOperation(
                        left, right, operationNode.OperationInfo, operationNode.GetToken());
                
                    _locals.Push(binaryOperation);
                    break;
                }
                default:
                    _locals.Push(node);
                    break;
            }
        }

        _postfixExpression.RemoveRange(
            postfixExpressionStartingLn, _postfixExpression.Count - postfixExpressionStartingLn);
        
        return _locals.Pop();
    }

    private static void ApplyPrefixUnaryOperators(
        List<IAstNode> postfixExpression,
        Stack<UnaryOperationData> prefixUnaryOps,
        int prefixUnaryOpsStartingLn)
    {
        var lastNode = postfixExpression[^1];

        while (prefixUnaryOps.Count > prefixUnaryOpsStartingLn)
        {
            var prefixOp = prefixUnaryOps.Pop();
            lastNode = new AstUnaryOperation(lastNode, prefixOp.OperationInfo, true, prefixOp.Token);
        }
                
        postfixExpression[^1] = lastNode;
    }

    private static bool IsUnaryOperatorInAppropriatePosition(AstUnaryOperationInfo unaryOperationInfo, NodeKind lastNodeKind)
    {
        var isInPrefixPosition = lastNodeKind == NodeKind.None || 
                                 lastNodeKind == NodeKind.Operator;
        
        var isInPostfixPosition = lastNodeKind == NodeKind.Operand;

        return unaryOperationInfo.Align == AstUnaryOperationAlign.Any ||
                (isInPrefixPosition && unaryOperationInfo.Align == AstUnaryOperationAlign.Prefix) ||
                (isInPostfixPosition && unaryOperationInfo.Align == AstUnaryOperationAlign.Postfix);
    }
    
    private enum NodeKind
    {
        None,
        Operand,
        Operator
    }

    private void FlushLeftToRightOperators(int operatorsStackStartingLn)
    {
        while (_operatorsStack.Count > operatorsStackStartingLn && 
               _operatorsStack.Peek().OperationAssociativity == AstOperationAssociativity.LeftToRight)
            _postfixExpression.Add(_operatorsStack.Pop());
    }
    
    private void PushOperatorToStack(OperationData op, int operatorsStackStartingLn)
    {
        while (_operatorsStack.Count > operatorsStackStartingLn && 
               ShouldPopOperatorFromStack(_operatorsStack.Peek(), op))
            _postfixExpression.Add(_operatorsStack.Pop());
                
        _operatorsStack.Push(op);
    }

    private static bool ShouldPopOperatorFromStack(OperationData stackOpInfo, OperationData newOperator)
    {
        return stackOpInfo.OperationAssociativity == AstOperationAssociativity.LeftToRight &&
               stackOpInfo.OperationOrder >= newOperator.OperationOrder;
    }
    
    private abstract class OperationData : IAstNode
    {
        public int OperationOrder { get; }
        public AstOperationAssociativity OperationAssociativity { get; }
        
        private readonly Token _token;

        protected OperationData(Token token, int operationOrder, AstOperationAssociativity operationAssociativity)
        {
            OperationOrder = operationOrder;
            OperationAssociativity = operationAssociativity;
            _token = token;
        }
        
        public Token GetToken() => _token;
    }
    
    private class TernaryOperationData : OperationData
    {
        public TernaryOperationData(Token token, int operationOrder) : 
            base(token, operationOrder, AstOperationAssociativity.RightToLeft) {}
        
    }
    
    private class BinaryOperationData : OperationData
    {
        public AstBinaryOperationInfo OperationInfo { get; }

        public BinaryOperationData(AstBinaryOperationInfo operationInfo, Token token) :
            base(token, operationInfo.OperationOrder, operationInfo.OperationAssociativity)
        {
            OperationInfo = operationInfo;
        }
    }
    
    private class UnaryOperationData
    {
        public AstUnaryOperationInfo OperationInfo { get; }
        public Token Token { get; }

        public UnaryOperationData(AstUnaryOperationInfo operationInfo, Token token)
        {
            OperationInfo = operationInfo;
            Token = token;
        }
    }
}