using LightJS.Errors;
using LightJS.Tokenizer;

namespace LightJS.Ast;

public class LjsAstBuilder
{
    private readonly string _sourceCodeString;
    private readonly TokensIterator _tokensIterator;
    
    private readonly List<ILjsAstNode> _postfixExpression = new();
    private readonly Stack<Op> _operatorsStack = new();
    private readonly Stack<ILjsAstNode> _locals = new();

    /// <summary>
    /// save nodes positions in source code (line number, col number)
    /// </summary>
    private readonly Dictionary<ILjsAstNode, LjsToken> _tokensMap = new();

    public LjsAstBuilder(string sourceCodeString)
    {
        if (string.IsNullOrEmpty(sourceCodeString))
        {
            throw new ArgumentException("input string is null or empty");
        }
        
        var ljsTokenizer = new LjsTokenizer(sourceCodeString);
        var tokens = ljsTokenizer.ReadTokens();
        
        _sourceCodeString = sourceCodeString;
        _tokensIterator = new TokensIterator(tokens);
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
        _tokensIterator = new TokensIterator(tokens);
    }

    public LjsAstModel Build()
    {
        if (!_tokensIterator.HasNextToken)
        {
            throw new Exception("no tokens");
        }

        var node = ProcessMainBlock();
        
        return new LjsAstModel(node, _tokensMap);

    }

    private void RegisterNodePosition(ILjsAstNode node, LjsToken token)
    {
        _tokensMap[node] = token;
    }

    private void ReplaceNodePosition(ILjsAstNode prevNode, ILjsAstNode newNode)
    {
        if (_tokensMap.TryGetValue(prevNode, out var token))
        {
            _tokensMap.Remove(prevNode);
            _tokensMap[newNode] = token;
        }
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

        public Op(LjsToken token, OpType opType, int priority)
        {
            Token = token;
            OpType = opType;
            Priority = priority;
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
        SwitchExpressionElement = 1 << 7,
        
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
                 tokenType == LjsTokenType.OpColon) ||
                
                ((terminationType & StopSymbolType.SwitchExpressionElement) != 0 &&
                 (tokenType == LjsTokenType.Break || tokenType == LjsTokenType.Case || tokenType == LjsTokenType.Default))
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
                currentType == LjsTokenType.OpBracketClose ||
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

        if (!_tokensIterator.HasNextToken) 
            return firstExpression;
        
        var sq = new LjsAstSequence();
        sq.AddNode(firstExpression);

        while (_tokensIterator.HasNextToken)
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

        if (allowEmptyBlock && _tokensIterator.NextToken.TokenType == LjsTokenType.OpBracketClose)
        {
            CheckExpectedNextAndMoveForward(LjsTokenType.OpBracketClose);
            return LjsAstEmptyNode.Instance;
        }
        
        var firstExpression = ProcessCodeLine(StopSymbolType.BracketClose);

        SkipRedundantSemicolons();
        CheckEarlyEof();

        if (_tokensIterator.NextToken.TokenType == LjsTokenType.OpBracketClose)
        {
            CheckExpectedNextAndMoveForward(LjsTokenType.OpBracketClose);
            return firstExpression;
        }
        
        var sq = new LjsAstSequence();
        sq.AddNode(firstExpression);

        while (_tokensIterator.HasNextToken &&
               _tokensIterator.NextToken.TokenType != LjsTokenType.OpBracketClose)
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
        if (!_tokensIterator.HasNextToken)
        {
            throw new LjsSyntaxError("unexpected EOF", _tokensIterator.CurrentToken.Position);
        }
    }

    private void SkipRedundantSemicolons()
    {
        while (_tokensIterator.HasNextToken &&
               _tokensIterator.NextToken.TokenType == LjsTokenType.OpSemicolon)
        {
            CheckExpectedNextAndMoveForward(LjsTokenType.OpSemicolon);
        }
    }

    private ILjsAstNode ProcessCodeLine(StopSymbolType stopSymbolType)
    {
        CheckEarlyEof();

        var nextToken = _tokensIterator.NextToken;

        var expressionStopSymbolType = 
            stopSymbolType | StopSymbolType.Auto | StopSymbolType.Semicolon;
        
        switch (nextToken.TokenType)
        {
            case LjsTokenType.Const:
            case LjsTokenType.Var:
            case LjsTokenType.Let:
                return ProcessVariableDeclaration(expressionStopSymbolType, nextToken.TokenType);
            
            case LjsTokenType.If:
                return ProcessIfBlock(stopSymbolType);
            
            case LjsTokenType.Switch:
                return ProcessSwitchBlock(stopSymbolType);
            
            case LjsTokenType.While:
                return ProcessWhileBlock(stopSymbolType);
            
            case LjsTokenType.Break:
                CheckExpectedNextAndMoveForward(LjsTokenType.Break);
                
                var breakNode = new LjsAstBreak();
                RegisterNodePosition(breakNode, _tokensIterator.CurrentToken);

                return breakNode;
            
            case LjsTokenType.Continue:
                CheckExpectedNextAndMoveForward(LjsTokenType.Continue);
                
                var continueNode = new LjsAstContinue();
                RegisterNodePosition(continueNode, _tokensIterator.CurrentToken);

                return continueNode;
            
            case LjsTokenType.For:
                return ProcessForLoop(stopSymbolType);
            
            case LjsTokenType.Return:

                CheckExpectedNextAndMoveForward(LjsTokenType.Return);
                
                var returnNodeToken = _tokensIterator.CurrentToken;

                var returnExpression = LjsAstEmptyNode.Instance;
                
                if (_tokensIterator.HasNextToken &&
                    ShouldProcessReturnStatementExpression(
                        _tokensIterator.CurrentToken, _tokensIterator.NextToken))
                {
                    returnExpression = ProcessExpression(
                        expressionStopSymbolType | StopSymbolType.BracketClose);
                }
                
                var returnNode = new LjsAstReturn(returnExpression);
                RegisterNodePosition(returnNode, returnNodeToken);

                return returnNode;
            
            case LjsTokenType.Function:
                
                CheckExpectedNextAndMoveForward(LjsTokenType.Function);
                
                var functionToken = _tokensIterator.CurrentToken;
                
                CheckExpectedNextAndMoveForward(LjsTokenType.Identifier);

                var funcName = LjsTokenizerUtils.GetTokenStringValue(
                    _sourceCodeString, _tokensIterator.CurrentToken);
                
                var funcDeclaration = ProcessFunctionDeclaration(funcName);
                
                RegisterNodePosition(funcDeclaration, functionToken);
                
                return funcDeclaration;
            
            default:
                return ProcessExpression(expressionStopSymbolType);
        }
    }

    private void CheckExpectedNext(LjsTokenType tokenType)
    {
        CheckEarlyEof();
        
        if (_tokensIterator.NextToken.TokenType != tokenType)
        {
            throw new LjsSyntaxError($"expected {tokenType}", _tokensIterator.NextToken.Position);
        }
    }
    
    private void CheckExpectedNextAndMoveForward(LjsTokenType tokenType)
    {
        CheckExpectedNext(tokenType);
        
        _tokensIterator.MoveForward();
    }

    private ILjsAstNode ProcessCommaSeparatedExpressions(StopSymbolType stopSymbol)
    {
        var stopSymbolType = stopSymbol | StopSymbolType.Comma;
        
        var exp = ProcessExpression(stopSymbolType);

        if (_tokensIterator.NextToken.TokenType != LjsTokenType.OpComma)
        {
            return exp;
        }
        
        var seq = new LjsAstSequence();
        seq.AddNode(exp);

        while (_tokensIterator.NextToken.TokenType == LjsTokenType.OpComma)
        {
            CheckExpectedNextAndMoveForward(LjsTokenType.OpComma);

            var nextExp = ProcessExpression(stopSymbolType);
            
            seq.AddNode(nextExp);
        }

        return seq;
    }

    private ILjsAstNode ProcessVariableDeclaration(StopSymbolType stopSymbol, LjsTokenType tokenType)
    {
        CheckExpectedNextAndMoveForward(tokenType);
        
        CheckExpectedNextAndMoveForward(LjsTokenType.Identifier);

        var firstVarNameToken = _tokensIterator.CurrentToken;
        var firstVarValue = LjsAstEmptyNode.Instance;

        if (_tokensIterator.NextToken.TokenType == LjsTokenType.OpAssign)
        {
            CheckExpectedNextAndMoveForward(LjsTokenType.OpAssign);
            firstVarValue = ProcessExpression(stopSymbol | StopSymbolType.Comma);
        }

        var variableKind = GetVariableDeclarationKind(tokenType);
        
        var firstVar = new LjsAstVariableDeclaration(
            LjsTokenizerUtils.GetTokenStringValue(_sourceCodeString, firstVarNameToken),
            firstVarValue, variableKind);
        
        RegisterNodePosition(firstVar, firstVarNameToken);
        
        if (_tokensIterator.NextToken.TokenType != LjsTokenType.OpComma)
        {
            return firstVar;
        }

        var seq = new LjsAstSequence();
        seq.AddNode(firstVar);
        
        while (_tokensIterator.NextToken.TokenType == LjsTokenType.OpComma)
        {
            CheckExpectedNextAndMoveForward(LjsTokenType.OpComma);
            
            CheckExpectedNextAndMoveForward(LjsTokenType.Identifier);
            
            var nextVarToken = _tokensIterator.CurrentToken;
            var nextVarValue = LjsAstEmptyNode.Instance;
            
            if (_tokensIterator.NextToken.TokenType == LjsTokenType.OpAssign)
            {
                CheckExpectedNextAndMoveForward(LjsTokenType.OpAssign);
                
                nextVarValue = ProcessExpression(stopSymbol | StopSymbolType.Comma);
            }
            
            var nextVar = new LjsAstVariableDeclaration(
                LjsTokenizerUtils.GetTokenStringValue(_sourceCodeString, nextVarToken),
                nextVarValue, variableKind);
            
            RegisterNodePosition(nextVar, nextVarToken);
            
            
            seq.AddNode(nextVar);
        }

        return seq;

    }

    private static bool IsVariableDeclaration(LjsTokenType tokenType) =>
        tokenType == LjsTokenType.Var ||
        tokenType == LjsTokenType.Let ||
        tokenType == LjsTokenType.Const;

    private static LjsAstVariableKind GetVariableDeclarationKind(LjsTokenType tokenType) => tokenType switch
    {
        LjsTokenType.Var => LjsAstVariableKind.Var,
        LjsTokenType.Let => LjsAstVariableKind.Let,
        LjsTokenType.Const => LjsAstVariableKind.Const,
        _ => throw new Exception($"invalid variable declaration token type {tokenType}")
    };

    private ILjsAstNode ProcessForLoop(StopSymbolType stopSymbol)
    {
        CheckExpectedNextAndMoveForward(LjsTokenType.For);
        CheckExpectedNextAndMoveForward(LjsTokenType.OpParenthesesOpen);

        var initExpr = LjsAstEmptyNode.Instance;
        var condExpr = LjsAstEmptyNode.Instance;
        var iterExpr = LjsAstEmptyNode.Instance;

        if (_tokensIterator.NextToken.TokenType != LjsTokenType.OpSemicolon)
        {
            initExpr = IsVariableDeclaration(_tokensIterator.NextToken.TokenType)
                ? ProcessVariableDeclaration(StopSymbolType.Semicolon, _tokensIterator.NextToken.TokenType)
                : ProcessExpression(StopSymbolType.Semicolon);
        }
        
        CheckExpectedNextAndMoveForward(LjsTokenType.OpSemicolon);
        
        if (_tokensIterator.NextToken.TokenType != LjsTokenType.OpSemicolon)
        {
            condExpr = ProcessExpression(StopSymbolType.Semicolon);
        }
        
        CheckExpectedNextAndMoveForward(LjsTokenType.OpSemicolon);
        
        if (_tokensIterator.NextToken.TokenType != LjsTokenType.OpParenthesesClose)
        {
            iterExpr = ProcessCommaSeparatedExpressions(StopSymbolType.ParenthesesClose);
        }
        
        CheckExpectedNextAndMoveForward(LjsTokenType.OpParenthesesClose);
        
        var hasBrackets =
            _tokensIterator.NextToken.TokenType == LjsTokenType.OpBracketOpen;
        
        var mainBody = hasBrackets ? 
            ProcessBlockInBrackets() : 
            ProcessCodeLine(stopSymbol | StopSymbolType.Semicolon | StopSymbolType.Auto);
        
        SkipRedundantSemicolons();
        
        var forLoop = new LjsAstForLoop(
            initExpr, condExpr, iterExpr, mainBody);

        return forLoop;
    }

    private ILjsAstNode ProcessSwitchBlock(StopSymbolType terminationType)
    {
        CheckExpectedNextAndMoveForward(LjsTokenType.Switch);

        CheckExpectedNextAndMoveForward(LjsTokenType.OpParenthesesOpen);
        
        var condition =
            ProcessExpression(StopSymbolType.ParenthesesClose);
        
        CheckExpectedNextAndMoveForward(LjsTokenType.OpParenthesesClose);
        
        CheckExpectedNextAndMoveForward(LjsTokenType.OpBracketOpen);

        var seq = new LjsAstSequence();

        while (_tokensIterator.NextToken.TokenType != LjsTokenType.OpBracketClose)
        {
            CheckEarlyEof();
            
            var nextTokenType = _tokensIterator.NextToken.TokenType;

            switch (nextTokenType)
            {
                case LjsTokenType.Case:
                    
                    CheckExpectedNextAndMoveForward(LjsTokenType.Case);

                    var caseToken = _tokensIterator.CurrentToken;
                    
                    var e = ProcessExpression(
                        StopSymbolType.Colon, ProcessExpressionMode.StopAtStopSymbol);

                    var switchCase = new LjsAstSwitchCase(e);
                    RegisterNodePosition(switchCase, caseToken);
                    
                    seq.AddNode(switchCase);
                    
                    break;
                
                case LjsTokenType.Default:
                    CheckExpectedNextAndMoveForward(LjsTokenType.Default);
                    CheckExpectedNextAndMoveForward(LjsTokenType.OpColon);
                    
                    var astSwitchDefault = new LjsAstSwitchDefault();
                    RegisterNodePosition(astSwitchDefault, _tokensIterator.CurrentToken);
                    
                    seq.AddNode(astSwitchDefault);
                    break;
                case LjsTokenType.Break:
                    CheckExpectedNextAndMoveForward(LjsTokenType.Break);
                    
                    var astBreak = new LjsAstBreak();
                    RegisterNodePosition(astBreak, _tokensIterator.CurrentToken);
                    
                    SkipRedundantSemicolons();
                    
                    seq.AddNode(astBreak);
                    break;
                
                default:
                    var c = ProcessCodeLine(StopSymbolType.Semicolon | StopSymbolType.BracketClose);
                    SkipRedundantSemicolons();
                    seq.AddNode(c);
                    break;
            }
        }
        
        CheckExpectedNextAndMoveForward(LjsTokenType.OpBracketClose);

        return new LjsAstSwitchBlock(condition, seq);
    }

    private ILjsAstNode ProcessWhileBlock(StopSymbolType terminationType)
    {
        CheckExpectedNextAndMoveForward(LjsTokenType.While);

        CheckExpectedNextAndMoveForward(LjsTokenType.OpParenthesesOpen);
        
        var condition =
            ProcessExpression(StopSymbolType.ParenthesesClose);
        
        CheckExpectedNextAndMoveForward(LjsTokenType.OpParenthesesClose);
        
        var hasBrackets =
            _tokensIterator.NextToken.TokenType == LjsTokenType.OpBracketOpen;
        
        var mainBody = hasBrackets ? 
            ProcessBlockInBrackets() : 
            ProcessCodeLine(terminationType | StopSymbolType.Semicolon | StopSymbolType.Auto);
        
        SkipRedundantSemicolons();
        
        var whileBlock = new LjsAstWhileLoop(condition, mainBody);

        return whileBlock;
    }

    private ILjsAstNode ProcessIfBlock(StopSymbolType terminationType)
    {
        CheckExpectedNextAndMoveForward(LjsTokenType.If);

        CheckExpectedNextAndMoveForward(LjsTokenType.OpParenthesesOpen);

        var mainCondition =
            ProcessExpression(StopSymbolType.ParenthesesClose);
        
        CheckExpectedNextAndMoveForward(LjsTokenType.OpParenthesesClose);
        
        var hasBrackets =
            _tokensIterator.NextToken.TokenType == LjsTokenType.OpBracketOpen;

        var mainBody = hasBrackets ? 
            ProcessBlockInBrackets() : 
            ProcessCodeLine(terminationType | StopSymbolType.Semicolon | StopSymbolType.Auto);
        
        SkipRedundantSemicolons();
        

        var ifBlock = new LjsAstIfBlock(
            new LjsAstConditionalExpression(mainCondition, mainBody));

        while (_tokensIterator.NextToken.TokenType == LjsTokenType.ElseIf)
        {
            CheckExpectedNextAndMoveForward(LjsTokenType.ElseIf);
            
            CheckExpectedNextAndMoveForward(LjsTokenType.OpParenthesesOpen);

            var altCondition =
                ProcessExpression(StopSymbolType.ParenthesesClose);
            
            CheckExpectedNextAndMoveForward(LjsTokenType.OpParenthesesClose);

            hasBrackets =
                _tokensIterator.NextToken.TokenType == LjsTokenType.OpBracketOpen;

            var altBody = hasBrackets
                ? ProcessBlockInBrackets()
                : ProcessCodeLine(terminationType | StopSymbolType.Semicolon | StopSymbolType.Auto);
            
            SkipRedundantSemicolons();

            var conditionalExpression = new LjsAstConditionalExpression(altCondition, altBody);
            
            ifBlock.ConditionalAlternatives.Add(
                conditionalExpression);
        }

        if (_tokensIterator.NextToken.TokenType == LjsTokenType.Else)
        {
            CheckExpectedNextAndMoveForward(LjsTokenType.Else);
            
            CheckEarlyEof();

            hasBrackets =
                _tokensIterator.NextToken.TokenType == LjsTokenType.OpBracketOpen;

            var elseBody = hasBrackets
                ? ProcessBlockInBrackets()
                : ProcessCodeLine(terminationType | StopSymbolType.Semicolon | StopSymbolType.Auto);
            
            SkipRedundantSemicolons();

            ifBlock.ElseBlock = elseBody;
        }

        return ifBlock;
    }

    private LjsAstObjectLiteral ProcessObjectLiteral()
    {
        // current token = {
        if (_tokensIterator.NextToken.TokenType == LjsTokenType.OpBracketClose)
        {
            CheckExpectedNextAndMoveForward(LjsTokenType.OpBracketClose);
            return new LjsAstObjectLiteral();
        }
        
        var obj = new LjsAstObjectLiteral();

        while (_tokensIterator.NextToken.TokenType != LjsTokenType.OpBracketClose)
        {
            CheckEarlyEof();

            var nextTokenType = _tokensIterator.NextToken.TokenType;

            var propName = string.Empty;
            
            if (nextTokenType == LjsTokenType.Identifier)
            {
                CheckExpectedNextAndMoveForward(LjsTokenType.Identifier);
                propName = LjsTokenizerUtils.GetTokenStringValue(
                    _sourceCodeString, _tokensIterator.CurrentToken);
            }
            else if (nextTokenType == LjsTokenType.StringLiteral)
            {
                CheckExpectedNextAndMoveForward(LjsTokenType.StringLiteral);
                propName = LjsTokenizerUtils.GetTokenStringValue(_sourceCodeString, _tokensIterator.CurrentToken);
            }
            else if (nextTokenType == LjsTokenType.IntDecimal)
            {
                CheckExpectedNextAndMoveForward(LjsTokenType.IntDecimal);
                propName = LjsTokenizerUtils.GetTokenStringValue(_sourceCodeString, _tokensIterator.CurrentToken);
                
            }
            else
            {
                throw new LjsSyntaxError("unexpected token", _tokensIterator.NextToken.Position);
            }
            
            CheckExpectedNextAndMoveForward(LjsTokenType.OpColon);
            
            var propertyValue = 
                ProcessExpression(StopSymbolType.Comma | StopSymbolType.BracketClose);
            
            obj.AddNode(new LjsAstObjectLiteralProperty(propName, propertyValue));
            
            CheckEarlyEof();

            switch (_tokensIterator.NextToken.TokenType)
            {
                case LjsTokenType.OpComma:
                    CheckExpectedNextAndMoveForward(LjsTokenType.OpComma);
                    break;
                case LjsTokenType.OpBracketClose:
                    // we'll stop here
                    break;
                default:
                    throw new LjsSyntaxError("unexpected token", _tokensIterator.NextToken.Position);
            }
        }

        CheckExpectedNextAndMoveForward(LjsTokenType.OpBracketClose);

        return obj;
    }
    
    private LjsAstArrayLiteral ProcessArrayLiteral()
    {
        // current token = [
        if (_tokensIterator.NextToken.TokenType == LjsTokenType.OpSquareBracketsClose)
        {
            CheckExpectedNextAndMoveForward(LjsTokenType.OpSquareBracketsClose);
            return new LjsAstArrayLiteral();
        }

        var arr = new LjsAstArrayLiteral();
        
        while (_tokensIterator.NextToken.TokenType != LjsTokenType.OpSquareBracketsClose)
        {
            CheckEarlyEof();
            
            var element = 
                ProcessExpression(StopSymbolType.Comma | StopSymbolType.SquareBracketsClose);
            
            arr.AddNode(element);
            
            CheckEarlyEof();

            switch (_tokensIterator.NextToken.TokenType)
            {
                case LjsTokenType.OpComma:
                    CheckExpectedNextAndMoveForward(LjsTokenType.OpComma);
                    break;
                case LjsTokenType.OpSquareBracketsClose:
                    // we'll stop here
                    break;
                default:
                    throw new LjsSyntaxError("unexpected token", _tokensIterator.NextToken.Position);
            }
        }
        
        CheckExpectedNextAndMoveForward(LjsTokenType.OpSquareBracketsClose);

        return arr;
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
        CheckEarlyEof();
        
        var operatorsStackStartingLn = _operatorsStack.Count;
        var postfixExpressionStartingLn = _postfixExpression.Count;
        
        var lastProcessedToken = default(LjsToken);
        
        var processFinished = false;

        while (_tokensIterator.HasNextToken && !processFinished)
        {
            _tokensIterator.MoveForward();

            var token = _tokensIterator.CurrentToken;
            var prevToken = lastProcessedToken.TokenType != LjsTokenType.None ?
                _tokensIterator.PrevToken : lastProcessedToken;
            var nextToken = _tokensIterator.NextToken;

            if (IsStopSymbol(token.TokenType, stopSymbolType))
            {
                if (mode == ProcessExpressionMode.StopBeforeStopSymbol)
                {
                    _tokensIterator.MoveBackward();
                }
                processFinished = true;
                break;
            }
            
            if (!processFinished &&
                (stopSymbolType & StopSymbolType.Auto) != 0 && 
                prevToken.TokenType != LjsTokenType.None &&
                ShouldAutoTerminateExpression(prevToken, token))
            {
                _tokensIterator.MoveBackward();
                processFinished = true;
                break;
            }

            lastProcessedToken = token;
            
            if (token.TokenType == LjsTokenType.Identifier)
            {
                var getVar = new LjsAstGetVar(
                    LjsTokenizerUtils.GetTokenStringValue(_sourceCodeString, token));

                RegisterNodePosition(getVar, token);
                
                _postfixExpression.Add(getVar);
            }
            else if (token.TokenType == LjsTokenType.This)
            {
                var getThis = new LjsAstGetThis();
                
                RegisterNodePosition(getThis, token);
                
                _postfixExpression.Add(getThis);
            }
            
            else if (LjsAstBuilderUtils.IsLiteral(token.TokenType))
            {
                var literalNode = LjsAstBuilderUtils.CreateLiteralNode(token, _sourceCodeString);
                RegisterNodePosition(literalNode, token);
                
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

                var ternaryIfOp = new Op(token, OpType.TernaryIf,
                    LjsAstBuilderUtils.GetOperatorPriority(token.TokenType, false));
                
                RegisterNodePosition(ternaryIfOp, token);
                
                PushOperatorToStack(ternaryIfOp, operatorsStackStartingLn);
                
                break;
            }
            else if (token.TokenType == LjsTokenType.Function)
            {
                var functionDeclaration = ProcessFunctionDeclaration();
                RegisterNodePosition(functionDeclaration, token);
                
                _postfixExpression.Add(functionDeclaration);
            }
            
            else if (token.TokenType == LjsTokenType.OpSquareBracketsOpen)
            {
                if (IsPropertyAccess(prevToken.TokenType))
                {
                    var propAccessNode = ProcessExpression(
                        StopSymbolType.SquareBracketsClose, 
                        ProcessExpressionMode.StopAtStopSymbol);

                    var propAccessOp = new Op(
                        token, OpType.PropAccess | OpType.Binary, 
                        LjsAstBuilderUtils.PropertyAccessOperatorsPriority);
                    
                    RegisterNodePosition(propAccessNode, token);
                    
                    PushOperatorToStack(propAccessOp, operatorsStackStartingLn);
                    
                    
                    _postfixExpression.Add(propAccessNode);
                }
                else
                {
                    _postfixExpression.Add(ProcessArrayLiteral());
                }
            }
            
            else if (token.TokenType == LjsTokenType.OpBracketOpen)
            {
                // object literal
                _postfixExpression.Add(ProcessObjectLiteral());
            }
            
            else if (token.TokenType == LjsTokenType.OpParenthesesOpen)
            {
                if (IsFunctionCall(prevToken.TokenType))
                {
                    var argumentsList = new LjsAstFunctionCallArguments();
                    
                    if (nextToken.TokenType == LjsTokenType.OpParenthesesClose)
                    {
                        // func call without arguments
                        
                        var funcCallOp = new Op(token, OpType.FuncCall | OpType.Binary,
                            LjsAstBuilderUtils.FuncCallOperatorPriority);
                        
                        RegisterNodePosition(funcCallOp, token);
                        
                        PushOperatorToStack(funcCallOp, operatorsStackStartingLn);
                        
                        _postfixExpression.Add(argumentsList);
                        
                        CheckExpectedNextAndMoveForward(LjsTokenType.OpParenthesesClose);
                    }
                    else
                    {
                        var funcArg = ProcessExpression(
                            StopSymbolType.FuncCall, ProcessExpressionMode.StopAtStopSymbol);
                        
                        argumentsList.AddNode(funcArg);
                        
                        while (_tokensIterator.CurrentToken.TokenType == LjsTokenType.OpComma)
                        {
                            funcArg = ProcessExpression(
                                StopSymbolType.FuncCall, ProcessExpressionMode.StopAtStopSymbol);
                            argumentsList.AddNode(funcArg);
                        }

                        var funcCallOp = new Op(token, OpType.FuncCall | OpType.Binary, 
                            LjsAstBuilderUtils.FuncCallOperatorPriority);
                        
                        RegisterNodePosition(funcCallOp, token);
                        
                        PushOperatorToStack(funcCallOp, operatorsStackStartingLn);
                        
                        _postfixExpression.Add(argumentsList);
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
                
                var assignOp = new Op(token, OpType.Assign, 0);
                
                RegisterNodePosition(assignOp, token);
                
                _postfixExpression.Add(assignOp);

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

                if (isUnaryPrefix && _operatorsStack.Count != 0 && _operatorsStack.Peek().IsUnaryPrefix)
                {
                    throw new LjsSyntaxError($"invalid operation {token.TokenType}", token.Position);
                }

                var opPriority = LjsAstBuilderUtils.GetOperatorPriority(token.TokenType, isUnary);
                
                var binaryOp = new Op(token, opType, opPriority); 
                
                RegisterNodePosition(binaryOp, token);
                PushOperatorToStack(binaryOp, operatorsStackStartingLn);
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
                (stopSymbolType & StopSymbolType.Eof) != 0 && !_tokensIterator.HasNextToken;

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
                    var argsNode = _locals.Pop();
                    var funcNode = _locals.Pop();
                    
                    if (argsNode is not LjsAstFunctionCallArguments argsSeq)
                        throw new LjsInternalError($"broken function call");
                    
                    var functionCall = new LjsAstFunctionCall(funcNode, argsSeq);
                    
                    ReplaceNodePosition(op, functionCall);
                        
                    _locals.Push(functionCall);
                }
                // property access
                else if ((op.OpType & OpType.PropAccess) != 0)
                {
                    var propNameNode =  _locals.Pop();
                    var operand = _locals.Pop();

                    var getProperty = new LjsAstGetProperty(propNameNode, operand);
                    
                    ReplaceNodePosition(op, getProperty);
                    
                    _locals.Push(getProperty);
                }
                else if ((op.OpType & OpType.TernaryIf) != 0)
                {
                    var falseExpression = _locals.Pop();
                    var trueExpression = _locals.Pop();
                    var condition = _locals.Pop();
                    var ternaryIfOperation = new LjsAstTernaryIfOperation(condition, trueExpression, falseExpression);
                    ReplaceNodePosition(op, ternaryIfOperation);
                    _locals.Push(ternaryIfOperation);
                }
                
                // unary operation
                else if (op.IsUnary)
                {
                    var operand = _locals.Pop();

                    if (op.TokenType is LjsTokenType.OpIncrement or LjsTokenType.OpDecrement)
                    {
                        var incrementSign = op.TokenType switch
                        {
                            LjsTokenType.OpIncrement => LjsAstIncrementSign.Plus,
                            LjsTokenType.OpDecrement => LjsAstIncrementSign.Minus,
                            _ => throw new LjsInternalError($"invalid increment token type {op.TokenType}")
                        };
                        
                        var incrementOrder = 
                            op.IsUnaryPrefix ? LjsAstIncrementOrder.Prefix : LjsAstIncrementOrder.Postfix;
                        
                        // replace getter nodes by setter nodes
                        switch (operand)
                        {
                            case LjsAstGetVar getVar:
                                var setVar = new LjsAstIncrementVar(getVar.VarName, incrementSign, incrementOrder);
                                ReplaceNodePosition(getVar, setVar);
                                _locals.Push(setVar);
                                break;
                            
                            case LjsAstGetNamedProperty getNamedProp:
                                
                                var setNamedProperty = new LjsAstIncrementNamedProperty(
                                    getNamedProp.PropertyName, getNamedProp.PropertySource, 
                                    incrementSign, incrementOrder);
                                
                                ReplaceNodePosition(getNamedProp, setNamedProperty);
                                _locals.Push(setNamedProperty);
                                break;
                            
                            case LjsAstGetProperty getProp:
                                var setProperty = new LjsAstIncrementProperty(
                                    getProp.PropertyName, getProp.PropertySource, 
                                    incrementSign, incrementOrder);
                                ReplaceNodePosition(getProp, setProperty);
                                _locals.Push(setProperty);
                                break;
                            default:
                                throw new LjsSyntaxError("invalid increment", op.Token.Position);
                        }
                    }
                    else
                    {
                        var unaryOperation = new LjsAstUnaryOperation(
                            operand, LjsAstBuilderUtils.GetUnaryOperationType(op.TokenType));
                    
                        ReplaceNodePosition(op, unaryOperation);
                    
                        _locals.Push(unaryOperation);
                    }
                    
                    
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
                            var getNamedProperty = new LjsAstGetNamedProperty(getVar.VarName, left);
                            ReplaceNodePosition(op, getNamedProperty);
                            _locals.Push(getNamedProperty);
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
                                var setVar = new LjsAstSetVar(getVar.VarName, right, LjsAstBuilderUtils.GetAssignMode(op.TokenType));
                                ReplaceNodePosition(getVar, setVar);
                                _locals.Push(setVar);
                                break;
                            case LjsAstGetNamedProperty getNamedProp:
                                var setNamedProperty = new LjsAstSetNamedProperty(
                                    getNamedProp.PropertyName, getNamedProp.PropertySource, 
                                    right, LjsAstBuilderUtils.GetAssignMode(op.TokenType));
                                ReplaceNodePosition(getNamedProp, setNamedProperty);
                                _locals.Push(setNamedProperty);
                                break;
                            case LjsAstGetProperty getProp:
                                var setProperty = new LjsAstSetProperty(
                                    getProp.PropertyName, getProp.PropertySource, 
                                    right, LjsAstBuilderUtils.GetAssignMode(op.TokenType));
                                ReplaceNodePosition(getProp, setProperty);
                                _locals.Push(setProperty);
                                break;
                            default:
                                throw new LjsSyntaxError("invalid assign", op.Token.Position);
                        }
                    }
                    else
                    {
                        var binaryOperation = new LjsAstBinaryOperation(
                            left, right, LjsAstBuilderUtils.GetBinaryOperationType(op.TokenType));
                        
                        ReplaceNodePosition(op, binaryOperation);
                        
                        _locals.Push(binaryOperation);
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

    private ILjsAstNode ProcessFunctionDeclaration(string functionName = "")
    {
        _functionDeclarationParameters.Clear();
        
        CheckExpectedNextAndMoveForward(LjsTokenType.OpParenthesesOpen);

        while (_tokensIterator.HasNextToken && 
               _tokensIterator.NextToken.TokenType != LjsTokenType.OpParenthesesClose)
        {
            CheckExpectedNextAndMoveForward(LjsTokenType.Identifier);

            var argNameToken = _tokensIterator.CurrentToken;
            var defaultValue = LjsAstEmptyNode.Instance;

            if (argNameToken.TokenType != LjsTokenType.Identifier)
                throw new LjsSyntaxError("expected identifier", argNameToken.Position);

            if (_tokensIterator.NextToken.TokenType == LjsTokenType.OpAssign)
            {
                CheckExpectedNextAndMoveForward(LjsTokenType.OpAssign);
                
                if (!LjsAstBuilderUtils.IsLiteral(_tokensIterator.NextToken.TokenType))
                {
                    throw new LjsSyntaxError("unexpected token", _tokensIterator.NextToken.Position);
                }
                
                _tokensIterator.MoveForward();
                    
                defaultValue = LjsAstBuilderUtils.CreateLiteralNode(
                    _tokensIterator.CurrentToken, _sourceCodeString);
            }
            
            _functionDeclarationParameters.Add(new LjsAstFunctionDeclarationParameter(
                LjsTokenizerUtils.GetTokenStringValue(_sourceCodeString, argNameToken),
                defaultValue
                ));

            if (_tokensIterator.NextToken.TokenType == LjsTokenType.OpComma)
            {
                CheckExpectedNextAndMoveForward(LjsTokenType.OpComma);
            }
            else if (_tokensIterator.NextToken.TokenType == LjsTokenType.OpParenthesesClose)
            {
                break; // arguments section finish
            }
            else
            {
                throw new LjsSyntaxError("unexpected token", _tokensIterator.NextToken.Position);
            }
        }
        
        CheckEarlyEof();
        
        CheckExpectedNextAndMoveForward(LjsTokenType.OpParenthesesClose);

        var parameters = new LjsAstFunctionDeclarationParameter[_functionDeclarationParameters.Count];

        for (var i = 0; i < parameters.Length; i++)
        {
            parameters[i] = _functionDeclarationParameters[i];
        }

        var functionBody = ProcessBlockInBrackets();

        ILjsAstNode funcDeclaration = string.IsNullOrEmpty(functionName) ? 
            new LjsAstAnonymousFunctionDeclaration(parameters, functionBody) : 
            new LjsAstNamedFunctionDeclaration(functionName, parameters, functionBody);

        return funcDeclaration;

    }

    private class TokensIterator
    {
        private readonly List<LjsToken> _tokens;

        private int _currentIndex = -1;
    
        public TokensIterator(List<LjsToken> tokens)
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