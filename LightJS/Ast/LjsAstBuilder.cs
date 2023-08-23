using LightJS.Tokenizer;

namespace LightJS.Ast;

public class LjsAstBuilder
{
    private readonly string _sourceCodeString;
    private readonly TokensReader _tokensReader;
    
    private readonly List<ILjsAstNode> _postfixExpression = new();
    private readonly Stack<Op> _operatorsStack = new();
    private readonly Stack<ILjsAstNode> _locals = new();

    /// <summary>
    /// save nodes positions in source code (line number, col number)
    /// </summary>
    private readonly Dictionary<ILjsAstNode, LjsTokenPosition> _tokenPositionsMap = new();

    public LjsAstBuilder(string sourceCodeString)
    {
        if (string.IsNullOrEmpty(sourceCodeString))
        {
            throw new ArgumentException("input string is null or empty");
        }
        
        var ljsTokenizer = new LjsTokenizer(sourceCodeString);
        var tokens = ljsTokenizer.ReadTokens();
        
        _sourceCodeString = sourceCodeString;
        _tokensReader = new TokensReader(tokens);
    }
    
    public LjsAstBuilder(string sourceCodeString, List<LjsToken> tokens)
    {
        if (string.IsNullOrEmpty(sourceCodeString))
        {
            throw new ArgumentException("input string is null or empty");
        }
        
        if (tokens == null)
            throw new ArgumentNullException(nameof(tokens));

        if (tokens.Count == 0)
            throw new ArgumentException("empty tokens list");
        
        _sourceCodeString = sourceCodeString;
        _tokensReader = new TokensReader(tokens);
    }

    public LjsAstModel Build()
    {
        if (!_tokensReader.HasNextToken)
        {
            throw new Exception("no tokens");
        }

        var node = ProcessMainBlock();
        
        return new LjsAstModel(node, _tokenPositionsMap);

    }
    
    [Flags]
    private enum OpType
    {
        None = 0,
        Binary = 1 << 0,
        UnaryPrefix = 1 << 1,
        UnaryPostfix = 1 << 2,
        Assign = 1 << 3,
        FuncCall = 1 << 5,
        PropAccess = 1 << 6,
        TernaryIf = 1 << 7
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
        public int Priority { get; }
        public int InnerMembersCount { get; }

        public Op(LjsToken token, OpType opType, int priority, int innerMembersCount = 0)
        {
            Token = token;
            OpType = opType;
            Priority = priority;
            InnerMembersCount = innerMembersCount;
        }
    }
    
    [Flags]
    private enum StopSymbolType
    {
        None = 0,
        Eof = 1 << 0,
        Semicolon = 1 << 1,
        ParenthesesClose = 1 << 2,
        Comma = 1 << 3,
        Colon = 1 << 4,
        SquareBracketsClose = 1 << 5,
        BracketClose = 1 << 6,
        
        Auto = 1 << 8,

        FuncCall = ParenthesesClose | Comma,
    }

    private static bool IsStopSymbol(
        LjsTokenType tokenType, StopSymbolType terminationType)
    {
        return
                ((terminationType & StopSymbolType.ParenthesesClose) != 0 &&
                 tokenType == LjsTokenType.OpParenthesesClose) ||

                ((terminationType & StopSymbolType.Semicolon) != 0 &&
                 tokenType == LjsTokenType.OpSemicolon) ||

                ((terminationType & StopSymbolType.Comma) != 0 &&
                 tokenType == LjsTokenType.OpComma) ||
            
                ((terminationType & StopSymbolType.SquareBracketsClose) != 0 &&
                 tokenType == LjsTokenType.OpSquareBracketsClose) ||
            
                ((terminationType & StopSymbolType.BracketClose) != 0 &&
                 tokenType == LjsTokenType.OpBracketClose) ||

                ((terminationType & StopSymbolType.Colon) != 0 &&
                 tokenType == LjsTokenType.OpColon)
            ;
    }

    private static bool ShouldProcessReturnStatementExpression(LjsToken currentToken, LjsToken nextToken)
    {
        if (nextToken.TokenType == LjsTokenType.OpSemicolon) return false;
        
        if (nextToken.Position.Line == currentToken.Position.Line) return true;
        
        var nextType = nextToken.TokenType;

        if (nextType == LjsTokenType.Function) return true;
        
        if (LjsAstBuilderUtils.IsKeyword(nextType)) return false;

        return nextType == LjsTokenType.Identifier ||
               nextType == LjsTokenType.OpParenthesesOpen ||
               nextType == LjsTokenType.OpSquareBracketsOpen ||
               nextType == LjsTokenType.OpBracketOpen ||
               LjsAstBuilderUtils.IsLiteral(nextType) ||
               LjsAstBuilderUtils.IsPossiblyPrefixUnaryOperator(nextType);
    }
    
    private static bool ShouldAutoTerminateExpression(LjsToken currentToken, LjsToken nextToken)
    {
        if (nextToken.Position.Line == currentToken.Position.Line) return false;

        var currentType = currentToken.TokenType;
        var nextType = nextToken.TokenType;
        
        if (LjsAstBuilderUtils.IsKeyword(nextType)) return true;

        return (currentType == LjsTokenType.Identifier ||
                currentType == LjsTokenType.OpParenthesesClose ||
                currentType == LjsTokenType.OpSquareBracketsClose ||
                LjsAstBuilderUtils.IsLiteral(currentType)) &&
               (nextType == LjsTokenType.Identifier ||
                LjsAstBuilderUtils.IsLiteral(nextType) ||
                LjsAstBuilderUtils.IsDefinitelyPrefixUnaryOperator(nextType));
    }

    private ILjsAstNode ProcessMainBlock()
    {
        // process main block of code
        
        SkipRedundantSemicolons();
        
        CheckEarlyEof();
        
        var firstExpression = ProcessCodeLine(StopSymbolType.Eof);
        
        SkipRedundantSemicolons();

        if (!_tokensReader.HasNextToken) 
            return firstExpression;
        
        var sq = new LjsAstSequence();
        sq.AddNode(firstExpression);

        while (_tokensReader.HasNextToken)
        {
            sq.AddNode(ProcessCodeLine(StopSymbolType.Eof));
            SkipRedundantSemicolons();
        }

        return sq;
    }

    private ILjsAstNode ProcessBlockInBrackets(bool allowEmptyBlock = true)
    {
        // starting token - just before brackets open
        
        CheckExpectedNextAndMoveForward(LjsTokenType.OpBracketOpen);
        
        SkipRedundantSemicolons();
        
        CheckEarlyEof();

        if (allowEmptyBlock && _tokensReader.NextToken.TokenType == LjsTokenType.OpBracketClose)
        {
            _tokensReader.MoveForward();
            return LjsAstEmptyNode.Instance;
        }
        
        var firstExpression = ProcessCodeLine(StopSymbolType.BracketClose);

        SkipRedundantSemicolons();
        CheckEarlyEof();

        if (_tokensReader.NextToken.TokenType == LjsTokenType.OpBracketClose)
        {
            _tokensReader.MoveForward();
            return firstExpression;
        }
        
        var sq = new LjsAstSequence();
        sq.AddNode(firstExpression);

        while (_tokensReader.HasNextToken &&
               _tokensReader.NextToken.TokenType != LjsTokenType.OpBracketClose)
        {
            sq.AddNode(ProcessCodeLine(StopSymbolType.BracketClose));
            SkipRedundantSemicolons();
            CheckEarlyEof();
        }
        
        CheckExpectedNextAndMoveForward(LjsTokenType.OpBracketClose);

        return sq;
    }
    private void CheckEarlyEof()
    {
        if (!_tokensReader.HasNextToken)
        {
            throw new LjsSyntaxError("unexpected EOF", _tokensReader.CurrentToken.Position);
        }
    }

    private void SkipRedundantSemicolons()
    {
        while (_tokensReader.HasNextToken &&
               _tokensReader.NextToken.TokenType == LjsTokenType.OpSemicolon)
        {
            _tokensReader.MoveForward();
        }
    }

    private ILjsAstNode ProcessCodeLine(StopSymbolType stopSymbolType)
    {
        CheckEarlyEof();

        var nextToken = _tokensReader.NextToken;

        var expressionStopSymbolType = 
            stopSymbolType | StopSymbolType.Auto | StopSymbolType.Semicolon;
        
        switch (nextToken.TokenType)
        {
            case LjsTokenType.Const:
                // todo ast const declaration
                throw new NotImplementedException();
            
            case LjsTokenType.Var:
                // todo ast var declaration
                throw new NotImplementedException();
            
            case LjsTokenType.If:
                return ProcessIfBlock(stopSymbolType);
            
            case LjsTokenType.Switch:
                // todo ast switch block
                throw new NotImplementedException();
            
            case LjsTokenType.While:
                // todo ast while loop
                throw new NotImplementedException();
            
            case LjsTokenType.For:
                // todo ast for loop (for(;;) && for(x in y))
                throw new NotImplementedException();
            
            case LjsTokenType.Return:
                
                _tokensReader.MoveForward();

                var returnExpression = LjsAstEmptyNode.Instance;
                
                if (_tokensReader.HasNextToken &&
                    ShouldProcessReturnStatementExpression(
                        _tokensReader.CurrentToken, _tokensReader.NextToken))
                {
                    returnExpression = ProcessExpression(
                        expressionStopSymbolType | StopSymbolType.BracketClose);
                }
                
                return new LjsAstReturn(returnExpression);
            
            case LjsTokenType.Function:
                
                _tokensReader.MoveForward();

                if (_tokensReader.NextToken.TokenType != LjsTokenType.Identifier)
                {
                    throw new LjsSyntaxError("expected identifier", _tokensReader.NextToken.Position);
                }
                
                _tokensReader.MoveForward();

                var funcName = LjsTokenizerUtils.GetTokenStringValue(
                    _sourceCodeString, _tokensReader.CurrentToken);

                return ProcessFunctionDeclaration(funcName);
            
            default:
                return ProcessExpression(expressionStopSymbolType);
        }
    }

    private void CheckExpectedNext(LjsTokenType tokenType)
    {
        CheckEarlyEof();
        
        if (_tokensReader.NextToken.TokenType != tokenType)
        {
            throw new LjsSyntaxError($"expected {tokenType}", _tokensReader.NextToken.Position);
        }
    }
    
    private void CheckExpectedNextAndMoveForward(LjsTokenType tokenType)
    {
        CheckExpectedNext(tokenType);
        
        _tokensReader.MoveForward();
    }

    private ILjsAstNode ProcessIfBlock(StopSymbolType terminationType)
    {
        _tokensReader.MoveForward();

        CheckExpectedNextAndMoveForward(LjsTokenType.OpParenthesesOpen);

        var mainCondition =
            ProcessExpression(StopSymbolType.ParenthesesClose);
        
        CheckExpectedNextAndMoveForward(LjsTokenType.OpParenthesesClose);
        
        var hasBrackets =
            _tokensReader.NextToken.TokenType == LjsTokenType.OpBracketOpen;

        var mainBody = hasBrackets ? 
            ProcessBlockInBrackets() : 
            ProcessCodeLine(terminationType | StopSymbolType.Semicolon | StopSymbolType.Auto);
        
        SkipRedundantSemicolons();
        

        var ifBlock = new LjsAstIfBlock(
            new LjsAstConditionalExpression(mainCondition, mainBody));

        while (_tokensReader.NextToken.TokenType == LjsTokenType.ElseIf)
        {
            _tokensReader.MoveForward();
            
            CheckExpectedNextAndMoveForward(LjsTokenType.OpParenthesesOpen);
            
            var altCondition =
                ProcessExpression(StopSymbolType.ParenthesesClose);
            
            CheckExpectedNextAndMoveForward(LjsTokenType.OpParenthesesClose);

            hasBrackets =
                _tokensReader.NextToken.TokenType == LjsTokenType.OpBracketOpen;

            var altBody = hasBrackets
                ? ProcessBlockInBrackets()
                : ProcessCodeLine(terminationType | StopSymbolType.Semicolon | StopSymbolType.Auto);
            
            SkipRedundantSemicolons();

            ifBlock.ConditionalAlternatives.Add(
                new LjsAstConditionalExpression(altCondition, altBody));
        }

        if (_tokensReader.NextToken.TokenType == LjsTokenType.Else)
        {
            _tokensReader.MoveForward();
            
            CheckEarlyEof();

            hasBrackets =
                _tokensReader.NextToken.TokenType == LjsTokenType.OpBracketOpen;

            var elseBody = hasBrackets
                ? ProcessBlockInBrackets()
                : ProcessCodeLine(terminationType | StopSymbolType.Semicolon | StopSymbolType.Auto);
            
            SkipRedundantSemicolons();

            ifBlock.ElseBlock = elseBody;
        }

        return ifBlock;
    }
    
    private enum ProcessExpressionMode
    {
        StopBeforeStopSymbol,
        StopAtStopSymbol
    }
    
    private ILjsAstNode ProcessExpression(
        StopSymbolType stopSymbolType, 
        ProcessExpressionMode mode = ProcessExpressionMode.StopBeforeStopSymbol)
    {
        var operatorsStackStartingLn = _operatorsStack.Count;
        var postfixExpressionStartingLn = _postfixExpression.Count;
        
        var lastProcessedToken = default(LjsToken);
        
        var processFinished = false;

        while (_tokensReader.HasNextToken && !processFinished)
        {
            _tokensReader.MoveForward();

            var token = _tokensReader.CurrentToken;
            var prevToken = lastProcessedToken.TokenType != LjsTokenType.None ?
                _tokensReader.PrevToken : lastProcessedToken;
            var nextToken = _tokensReader.NextToken;

            if (IsStopSymbol(token.TokenType, stopSymbolType))
            {
                if (mode == ProcessExpressionMode.StopBeforeStopSymbol)
                {
                    _tokensReader.MoveBackward();
                }
                processFinished = true;
                break;
            }
            
            if (!processFinished &&
                (stopSymbolType & StopSymbolType.Auto) != 0 && 
                prevToken.TokenType != LjsTokenType.None &&
                ShouldAutoTerminateExpression(prevToken, token))
            {
                _tokensReader.MoveBackward();
                processFinished = true;
                break;
            }

            lastProcessedToken = token;
            
            if (token.TokenType == LjsTokenType.Identifier)
            {
                var getVar = new LjsAstGetVar(
                    LjsTokenizerUtils.GetTokenStringValue(_sourceCodeString, token));
                
                _tokenPositionsMap[getVar] = token.Position;
                
                _postfixExpression.Add(getVar);
            }
            else if (LjsAstBuilderUtils.IsLiteral(token.TokenType))
            {
                var literalNode = LjsAstBuilderUtils.CreateLiteralNode(token, _sourceCodeString);

                _tokenPositionsMap[literalNode] = token.Position;
                
                _postfixExpression.Add( literalNode);
            }
            
            else if (token.TokenType == LjsTokenType.OpQuestionMark)
            {
                var trueExpressionNode = ProcessExpression(
                    StopSymbolType.Colon, ProcessExpressionMode.StopAtStopSymbol);
                
                var falseExpressionNode = ProcessExpression(stopSymbolType, mode);

                processFinished = true;

                _postfixExpression.Add(trueExpressionNode);
                _postfixExpression.Add(falseExpressionNode);
                
                PushOperatorToStack(
                    new Op(token, OpType.TernaryIf,
                        LjsAstBuilderUtils.GetOperatorPriority(token.TokenType, false)),
                    operatorsStackStartingLn);
                
                break;
            }
            else if (token.TokenType == LjsTokenType.Function)
            {
                var functionDeclaration = ProcessFunctionDeclaration();
                
                _postfixExpression.Add(functionDeclaration);
            }
            
            else if (token.TokenType == LjsTokenType.OpSquareBracketsOpen)
            {
                if (IsPropertyAccess(prevToken.TokenType))
                {
                    var propAccessNode = ProcessExpression(
                        StopSymbolType.SquareBracketsClose, 
                        ProcessExpressionMode.StopAtStopSymbol);

                    _postfixExpression.Add(propAccessNode);
                    
                    _operatorsStack.Push(new Op(
                        token, OpType.PropAccess | OpType.UnaryPostfix, 
                        LjsAstBuilderUtils.FuncCallOperatorPriority, 1));
                }
                else
                {
                    throw new NotImplementedException();
                    // todo check if array initialization
                }
            }
            
            else if (token.TokenType == LjsTokenType.OpParenthesesOpen)
            {
                if (IsFunctionCall(prevToken.TokenType))
                {
                    if (nextToken.TokenType == LjsTokenType.OpParenthesesClose)
                    {
                        // func call without arguments
                        PushOperatorToStack(
                            new Op(token, OpType.FuncCall | OpType.UnaryPostfix,
                                LjsAstBuilderUtils.FuncCallOperatorPriority), 
                            
                            operatorsStackStartingLn);
                        
                        _tokensReader.MoveForward(); // skip closing parentheses
                    }
                    else
                    {
                        var argumentsCount = 1;
                        
                        var funcArg = ProcessExpression(
                            StopSymbolType.FuncCall, ProcessExpressionMode.StopAtStopSymbol);
                        
                        _postfixExpression.Add(funcArg);
                        
                        while (_tokensReader.CurrentToken.TokenType == LjsTokenType.OpComma)
                        {
                            funcArg = ProcessExpression(
                                StopSymbolType.FuncCall, ProcessExpressionMode.StopAtStopSymbol);
                            _postfixExpression.Add(funcArg);
                            ++argumentsCount;
                        }
                        
                        PushOperatorToStack(
                            new Op(token, OpType.FuncCall | OpType.UnaryPostfix, 
                                LjsAstBuilderUtils.FuncCallOperatorPriority, argumentsCount),
                            operatorsStackStartingLn);
                    }
                    
                }
                else
                {
                    var enclosedOperation = ProcessExpression(
                        StopSymbolType.FuncCall, ProcessExpressionMode.StopAtStopSymbol);
                    
                    _postfixExpression.Add(enclosedOperation);
                }
            }
            else if (LjsAstBuilderUtils.IsAssignOperator(token.TokenType))
            {
                // we do recursive assignment operations processing for preserving right to left order of operations in assignment chain
                var assignExpression = ProcessExpression(
                    stopSymbolType, mode);
                
                while (_operatorsStack.Count > operatorsStackStartingLn)
                {
                    _postfixExpression.Add(_operatorsStack.Pop());
                }
                
                _postfixExpression.Add(assignExpression);
                _postfixExpression.Add(new Op(token, OpType.Assign, 0));
                

                processFinished = true;
                
                break;
                
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
                }
                
                // todo check isUnary equals isDefinitelyUnary() method and throw error if needed

                var opPriority = LjsAstBuilderUtils.GetOperatorPriority(token.TokenType, isUnary);
                
                PushOperatorToStack(
                    new Op(token, opType, opPriority), 
                    operatorsStackStartingLn);
            }
            else
            {
                throw new LjsSyntaxError($"unexpected token {token.TokenType}", token.Position);
            }
        }
        
        // check correct exit in modes
        if (!processFinished)
        {
            var eofTermination = 
                (stopSymbolType & StopSymbolType.Eof) != 0 && !_tokensReader.HasNextToken;

            if (!eofTermination)
            {
                throw new LjsSyntaxError("unterminated expression", lastProcessedToken.Position);
            }
        }
        
        while (_operatorsStack.Count > operatorsStackStartingLn)
        {
            _postfixExpression.Add(_operatorsStack.Pop());
        }
        
        // --------- CREATE NODES
        
        _locals.Clear();
        
        for (var i = postfixExpressionStartingLn; i < _postfixExpression.Count; i++)
        {
            var node = _postfixExpression[i];
            
            if (node is Op op)
            {

                // function call operation
                if ((op.OpType & OpType.FuncCall) != 0)
                {
                    var argumentsCount = op.InnerMembersCount;

                    if (argumentsCount == 0)
                    {
                        var operand = _locals.Pop();
                    
                        _locals.Push(new LjsAstFunctionCall(operand));
                    }
                    else
                    {
                        var args = new ILjsAstNode[argumentsCount];
                        
                        for (var j = 0; j < argumentsCount; j++)
                        {
                            args[j] = _locals.Pop();
                        }
                        
                        var operand = _locals.Pop();
                        
                        _locals.Push(new LjsAstFunctionCall(operand, args));
                    }
                }
                // property access
                else if ((op.OpType & OpType.PropAccess) != 0)
                {
                    var propNameNode =  _locals.Pop();
                    var operand = _locals.Pop();
                    
                    _locals.Push(new LjsAstGetProperty(propNameNode, operand));
                }
                else if ((op.OpType & OpType.TernaryIf) != 0)
                {
                    var falseExpression = _locals.Pop();
                    var trueExpression = _locals.Pop();
                    var condition = _locals.Pop();
                    _locals.Push(new LjsAstTernaryIfOperation(condition, trueExpression, falseExpression));
                }
                
                // unary operation
                else if (op.IsUnary)
                {
                    var operand = _locals.Pop();
                    _locals.Push(new LjsAstUnaryOperation(
                        operand, LjsAstBuilderUtils.GetUnaryOperationType(op.TokenType, op.IsUnaryPrefix)));
                }
                else
                {
                    if (_locals.Count < 2)
                    {
                        throw new LjsSyntaxError("invalid binary operation", op.Token.Position);
                    }
                    
                    // binary operation
                    var right = _locals.Pop();
                    var left = _locals.Pop();

                    if (op.TokenType == LjsTokenType.OpDot)
                    {
                        // convert get var node into get named property
                        if (right is LjsAstGetVar getVar)
                        {
                            _locals.Push(new LjsAstGetNamedProperty(getVar.VarName, left));
                        }
                        else
                        {
                            throw new LjsSyntaxError("invalid property access", op.Token.Position);
                        }
                    }

                    else if ((op.OpType & OpType.Assign) != 0)
                    {
                        // replace getter nodes by setter nodes
                        switch (left)
                        {
                            case LjsAstGetVar getVar:
                                _locals.Push(new LjsAstSetVar(getVar.VarName, right, LjsAstBuilderUtils.GetAssignMode(op.TokenType)));
                                break;
                            case LjsAstGetNamedProperty getNamedProp:
                                _locals.Push(new LjsAstSetNamedProperty(
                                    getNamedProp.PropertyName, getNamedProp.PropertySource, 
                                    right, LjsAstBuilderUtils.GetAssignMode(op.TokenType)));
                                break;
                            case LjsAstGetProperty getProp:
                                _locals.Push(new LjsAstSetProperty(
                                    getProp.PropertyName, getProp.PropertySource, 
                                    right, LjsAstBuilderUtils.GetAssignMode(op.TokenType)));
                                break;
                            default:
                                throw new LjsSyntaxError("invalid assign", op.Token.Position);
                        }
                    }
                    else
                    {
                        _locals.Push(new LjsAstBinaryOperation(
                            left, right, LjsAstBuilderUtils.GetBinaryOperationType(op.TokenType)));
                    }
                    
                    
                }
                
            }
            else
            {
                _locals.Push(node);
            }
        }
        
        _postfixExpression.RemoveRange(
            postfixExpressionStartingLn, _postfixExpression.Count - postfixExpressionStartingLn);
        
        return _locals.Pop();
    }

    private void PushOperatorToStack(Op op, int operatorsStackStartingLn)
    {
        while (_operatorsStack.Count > operatorsStackStartingLn && 
               (_operatorsStack.Peek().Priority >= op.Priority))
            _postfixExpression.Add(_operatorsStack.Pop());
                
        _operatorsStack.Push(op);
    }
    
    private static bool IsFunctionCall(LjsTokenType prevTokenType) => prevTokenType is
        LjsTokenType.Identifier or
        LjsTokenType.OpParenthesesClose or
        LjsTokenType.OpSquareBracketsClose;

    private static bool IsPropertyAccess(LjsTokenType prevTokenType) => prevTokenType is
        LjsTokenType.Identifier or
        LjsTokenType.OpParenthesesClose or
        LjsTokenType.OpSquareBracketsClose;

    private readonly List<LjsAstFunctionDeclarationParameter> _functionDeclarationParameters = new();

    private ILjsAstNode ProcessFunctionDeclaration(string functionName = null)
    {
        _functionDeclarationParameters.Clear();
        
        if (_tokensReader.NextToken.TokenType != LjsTokenType.OpParenthesesOpen)
        {
            throw new LjsSyntaxError("expected '(' after function", _tokensReader.NextToken.Position);
        }
                
        _tokensReader.MoveForward();

        while (_tokensReader.HasNextToken && 
               _tokensReader.NextToken.TokenType != LjsTokenType.OpParenthesesClose)
        {
            
            _tokensReader.MoveForward();

            var argNameToken = _tokensReader.CurrentToken;
            var defaultValue = LjsAstEmptyNode.Instance;

            if (argNameToken.TokenType != LjsTokenType.Identifier)
                throw new LjsSyntaxError("expected identifier", argNameToken.Position);

            if (_tokensReader.NextToken.TokenType == LjsTokenType.OpAssign)
            {
                _tokensReader.MoveForward();
                
                if (!LjsAstBuilderUtils.IsLiteral(_tokensReader.NextToken.TokenType))
                {
                    throw new LjsSyntaxError("unexpected token", _tokensReader.NextToken.Position);
                }
                
                _tokensReader.MoveForward();
                    
                defaultValue = LjsAstBuilderUtils.CreateLiteralNode(
                    _tokensReader.CurrentToken, _sourceCodeString);
            }
            
            _functionDeclarationParameters.Add(new LjsAstFunctionDeclarationParameter(
                LjsTokenizerUtils.GetTokenStringValue(_sourceCodeString, argNameToken),
                defaultValue
                ));

            if (_tokensReader.NextToken.TokenType == LjsTokenType.OpComma)
            {
                _tokensReader.MoveForward();
            }
            else if (_tokensReader.NextToken.TokenType == LjsTokenType.OpParenthesesClose)
            {
                break; // arguments section finish
            }
            else
            {
                throw new LjsSyntaxError("unexpected token", _tokensReader.NextToken.Position);
            }
        }
        
        CheckEarlyEof();
        
        _tokensReader.MoveForward(); // skip closing parentheses

        var parameters = new LjsAstFunctionDeclarationParameter[_functionDeclarationParameters.Count];

        for (var i = 0; i < parameters.Length; i++)
        {
            parameters[i] = _functionDeclarationParameters[i];
        }

        var functionBody = ProcessBlockInBrackets();

        return string.IsNullOrEmpty(functionName) ? 
            new LjsAstFunctionDeclaration(parameters, functionBody) : 
            new LjsAstNamedFunctionDeclaration(functionName, parameters, functionBody);

    }

    private class TokensReader
    {
        private readonly List<LjsToken> _tokens;

        private int _currentIndex = -1;
    
        public TokensReader(List<LjsToken> tokens)
        {
            _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
        }

        public LjsToken CurrentToken => 
            _currentIndex >= 0 && _currentIndex < _tokens.Count ? 
                _tokens[_currentIndex] : default;

        public LjsToken NextToken => 
            _currentIndex + 1 < _tokens.Count ? 
                _tokens[_currentIndex + 1] : default;

        public LjsToken PrevToken => 
            _currentIndex > 0 ?
                _tokens[_currentIndex - 1] : default;

        public bool HasNextToken => _currentIndex + 1 < _tokens.Count;

        public void MoveForward()
        {
            if (!HasNextToken)
            {
                throw new IndexOutOfRangeException();
            }
        
            ++_currentIndex;
        }
        
        public void MoveBackward()
        {
            if (_currentIndex <= 0)
            {
                throw new IndexOutOfRangeException();
            }
        
            --_currentIndex;
        }
        
    }
    
}