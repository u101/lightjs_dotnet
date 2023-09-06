using LightJS.Ast;
using LightJS.Errors;
using LightJS.Program;
using LightJS.Runtime;
using LightJS.Tokenizer;

namespace LightJS.Compiler;

public class LjsCompiler
{
    private readonly LjsAstModel _astModel;
    private readonly LjsProgramConstants _constants = new();
    private readonly List<LjsCompilerContext> _functionsList = new();

    public LjsCompiler(string sourceCodeString)
    {
        if (string.IsNullOrEmpty(sourceCodeString))
        {
            throw new ArgumentException("input string is null or empty");
        }

        var astModelBuilder = new LjsAstBuilder(sourceCodeString);
        
        _astModel = astModelBuilder.Build();
    }

    public LjsCompiler(string sourceCodeString, List<LjsToken> tokens)
    {
        if (string.IsNullOrEmpty(sourceCodeString))
        {
            throw new ArgumentException("input string is null or empty");
        }
        
        if (tokens == null)
            throw new ArgumentNullException(nameof(tokens));

        if (tokens.Count == 0)
            throw new ArgumentException("empty tokens list");
        
        var astModelBuilder = new LjsAstBuilder(sourceCodeString, tokens);
        
        _astModel = astModelBuilder.Build();
    }
    
    public LjsCompiler(LjsAstModel astModel)
    {
        _astModel = astModel;
    }

    public LjsProgram Compile()
    {
        var mainFunc = new LjsCompilerFunctionContext(0);
        
        var context = new LjsCompilerContext(mainFunc, _functionsList);
        
        _functionsList.Add(context);
        
        ProcessNode(_astModel.RootNode, context);

        mainFunc.Instructions.Add(
            new LjsInstruction(LjsInstructionCode.Halt));

        var functions = _functionsList.Select(ctx => new LjsFunctionData(
            ctx.FunctionContext.FunctionIndex,
            ctx.FunctionContext.Instructions.Instructions.ToArray(), 
            ctx.FunctionContext.FunctionArgs.ToArray(), 
            ctx.Locals.Pointers.ToArray()
        )).ToArray();
        
        return new LjsProgram(
            _constants, functions, context.NamedFunctionsMap);
    }
    private void ProcessFunction(
        LjsAstFunctionDeclaration functionDeclaration, 
        LjsCompilerContext context)
    {

        var parameters = functionDeclaration.Parameters;
        var functionContext = context.FunctionContext;

        for (var i = 0; i < parameters.Length; i++)
        {
            var p = parameters[i];
            var defaultValue = GetFunctionParameterDefaultValue(p.DefaultValue);

            AssertArgumentNameIsUniq(p.Name, functionDeclaration, context);

            
            functionContext.FunctionArgs.Add(new LjsFunctionArgument(p.Name, defaultValue));

            context.Locals.Add(p.Name, LjsLocalVarKind.Argument);
        }
        
        ProcessNode(functionDeclaration.FunctionBody, context);

        
        
        if (functionContext.Instructions.Count == 0 ||
            functionContext.Instructions.LastInstruction.Code != LjsInstructionCode.Return)
        {
            functionContext.Instructions.Add(new LjsInstruction(LjsInstructionCode.ConstUndef));
            functionContext.Instructions.Add(new LjsInstruction(LjsInstructionCode.Return));
        }
    }

    private static LjsObject GetFunctionParameterDefaultValue(ILjsAstNode node) => node switch
    {
        LjsAstNull _ => LjsObject.Null,
        LjsAstUndefined _ => LjsObject.Undefined,
        LjsAstLiteral<int> i => new LjsInteger(i.Value),
        LjsAstLiteral<double> i => new LjsDouble(i.Value),
        LjsAstLiteral<string> i => new LjsString(i.Value),
        LjsAstLiteral<bool> i => i.Value ? LjsBoolean.True : LjsBoolean.False,
        _ => LjsObject.Undefined
    };

    private LjsInstruction GetIntLiteralInstruction(LjsAstLiteral<int> lit) => lit.Value switch
    {
        0 => new LjsInstruction(LjsInstructionCode.ConstIntZero),
        1 => new LjsInstruction(LjsInstructionCode.ConstIntOne),
        -1 => new LjsInstruction(LjsInstructionCode.ConstIntMinusOne),
        _ => new LjsInstruction(LjsInstructionCode.ConstInt, lit.Value)
    };

    private LjsInstruction GetDoubleLiteralInstruction(LjsAstLiteral<double> lit)
    {
        var v = lit.Value;
        return v switch
        {
            double.NaN => new LjsInstruction(LjsInstructionCode.ConstDoubleNaN),
            0 => new LjsInstruction(LjsInstructionCode.ConstDoubleZero),
            _ => new LjsInstruction(LjsInstructionCode.ConstDouble, _constants.AddDoubleConstant(v))
        };
    }
    

    private void ProcessNode(
        ILjsAstNode node, 
        LjsCompilerContext context,
        ICollection<int>? jumpToTheStartPlaceholdersIndices = null, 
        ICollection<int>? jumpToTheEndPlaceholdersIndices = null)
    {
        var instructions = context.FunctionContext.Instructions;

        switch (node)
        {
            case LjsAstAnonymousFunctionDeclaration funcDeclaration:
                
                var anonFunc = context.CreateAnonFunctionContext();
                
                ProcessFunction(funcDeclaration, anonFunc);
                
                instructions.Add(new LjsInstruction(LjsInstructionCode.FuncRef, anonFunc.FunctionContext.FunctionIndex));
                
                break;
            
            case LjsAstNamedFunctionDeclaration namedFunctionDeclaration:
                
                var (namedFunc, namedFuncIndex) = context.
                    GetOrCreateNamedFunctionContext(namedFunctionDeclaration);
                
                ProcessFunction(namedFunctionDeclaration, namedFunc);
                break;
            
            case LjsAstFunctionCall functionCall:

                var specifiedArgumentsCount = functionCall.Arguments.Count;
                
                foreach (var n in functionCall.Arguments.ChildNodes)
                {
                    ProcessNode(n, context);
                }
                
                ProcessNode(functionCall.FunctionGetter, context);
                
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.FuncCall, specifiedArgumentsCount));
                
                break;
            
            case LjsAstReturn astReturn:

                if (astReturn.ReturnValue != LjsAstEmptyNode.Instance)
                {
                    ProcessNode(astReturn.ReturnValue, context);
                }
                else
                {
                    instructions.Add(new LjsInstruction(LjsInstructionCode.ConstUndef));
                }
                
                instructions.Add(new LjsInstruction(LjsInstructionCode.Return));
                break;
            
            case LjsAstEmptyNode _:
                // do nothing
                break;
            
            case LjsAstBreak astBreak:
                
                AssertBreakStatementHasJumpPoint(astBreak, jumpToTheEndPlaceholdersIndices);
                
                jumpToTheEndPlaceholdersIndices!.Add(instructions.Count);
                instructions.Add(default);
                
                break;
            
            case LjsAstContinue astContinue:
                AssertContinueStatementHasJumpPoint(astContinue, jumpToTheStartPlaceholdersIndices);
                
                jumpToTheStartPlaceholdersIndices!.Add(instructions.Count);
                instructions.Add(default);
                break;
            
            case LjsAstLiteral<int> lit:
                instructions.Add(GetIntLiteralInstruction(lit));
                break;
            
            case LjsAstLiteral<double> lit:
                instructions.Add(GetDoubleLiteralInstruction(lit));
                break;
            
            case LjsAstLiteral<string> lit:
                instructions.Add(GetStringConstInstruction(lit.Value));
                break;
            
            case LjsAstNull _:
                instructions.Add(
                    new LjsInstruction(LjsInstructionCode.ConstNull));
                break;
            case LjsAstUndefined _:
                instructions.Add(
                    new LjsInstruction(LjsInstructionCode.ConstUndef));
                break;
            
            case LjsAstLiteral<bool> lit:
                instructions.Add(new LjsInstruction(
                    lit.Value ? LjsInstructionCode.ConstTrue : LjsInstructionCode.ConstFalse));
                break;
            
            case LjsAstBinaryOperation binaryOperation:
                
                ProcessNode(binaryOperation.LeftOperand, context);
                ProcessNode(binaryOperation.RightOperand, context);

                instructions.Add(new LjsInstruction(LjsCompileUtils.GetBinaryOpCode(binaryOperation.OperatorType)));
                
                break;
            
            case LjsAstUnaryOperation unaryOperation:

                switch (unaryOperation.OperatorType)
                {
                    case LjsAstUnaryOperationType.Plus:
                        // just skip, because unary plus does nothing
                        ProcessNode(unaryOperation.Operand, context);
                        break;
                    
                    case LjsAstUnaryOperationType.Minus:
                        ProcessNode(unaryOperation.Operand, context);
                        instructions.Add(new LjsInstruction(LjsInstructionCode.Minus));
                        break;
                    case LjsAstUnaryOperationType.LogicalNot:
                        ProcessNode(unaryOperation.Operand, context);
                        instructions.Add(new LjsInstruction(LjsInstructionCode.Not));
                        break;
                    
                    case LjsAstUnaryOperationType.BitNot:
                        ProcessNode(unaryOperation.Operand, context);
                        instructions.Add(new LjsInstruction(LjsInstructionCode.BitNot));
                        break;
                    
                    default:
                        throw CreateUnknownUnaryOperatorException(unaryOperation);
                }
                
                break;
            
            case LjsAstVariableDeclaration variableDeclaration:

                var localVarKind = LjsCompileUtils.GetVarKind(variableDeclaration.VariableKind);

                AssertVariableNameIsUniq(variableDeclaration, context);
                
                
                
                var varIndex = context.Locals.Add(
                    variableDeclaration.Name, 
                    localVarKind);

                if (variableDeclaration.Value != LjsAstEmptyNode.Instance)
                {
                    ProcessNode(variableDeclaration.Value, context);
                    
                    instructions.Add(new LjsInstruction(LjsInstructionCode.VarInit, varIndex));
                }
                else
                {
                    if (localVarKind == LjsLocalVarKind.Const)
                        throw new LjsCompilerError($"const {variableDeclaration.Name} must be initialized");
                }
                
                break;
            
            case LjsAstGetVar getVar:
                AddVarLoadInstruction(context, getVar);
                break;
            
            case LjsAstSetVar setVar:
                AddVarStoreInstructions(context, setVar);
                break;
            
            case LjsAstIncrementVar incrementVar:
                AddVarIncrementInstruction(context, incrementVar);
                break;
            
            case LjsAstIfBlock ifBlock:
                
                ProcessNode(ifBlock.MainBlock.Condition, context);
                
                // indices of empty placeholder instructions to be replaced with actual jump instructions 
                var ifEndIndices = LjsCompileUtils.GetTemporaryIntList();

                var ifConditionalJumpIndex = instructions.Count;
                
                // if false jump to next condition or to the else block or to the end
                instructions.Add(default);
                
                ProcessNode(ifBlock.MainBlock.Expression, context.CreateLocalContext(), 
                    jumpToTheStartPlaceholdersIndices, jumpToTheEndPlaceholdersIndices);
                
                ifEndIndices.Add(instructions.Count);
                instructions.Add(default);

                if (ifBlock.ConditionalAlternatives.Count != 0)
                {
                    foreach (var alternative in ifBlock.ConditionalAlternatives)
                    {
                        // set previous jump instruction
                        instructions.SetAt(new LjsInstruction(
                            LjsInstructionCode.JumpIfFalse, instructions.Count), 
                            ifConditionalJumpIndex);
                        
                        ProcessNode(alternative.Condition, context);
                        
                        ifConditionalJumpIndex = instructions.Count;
                        instructions.Add(default);
                        
                        ProcessNode(alternative.Expression, context.CreateLocalContext(),
                            jumpToTheStartPlaceholdersIndices, jumpToTheEndPlaceholdersIndices);
                        
                        ifEndIndices.Add(instructions.Count);
                        instructions.Add(default);
                    }
                }

                if (ifBlock.ElseBlock != null)
                {
                    instructions.SetAt(new LjsInstruction(
                        LjsInstructionCode.JumpIfFalse, instructions.Count), 
                        ifConditionalJumpIndex);

                    ifConditionalJumpIndex = -1;
                    
                    ProcessNode(ifBlock.ElseBlock, context.CreateLocalContext(),
                        jumpToTheStartPlaceholdersIndices, jumpToTheEndPlaceholdersIndices);
                }
                
                var ifBlockEndIndex = instructions.Count;

                if (ifConditionalJumpIndex != -1)
                {
                    instructions.SetAt(new LjsInstruction(
                        LjsInstructionCode.JumpIfFalse, ifBlockEndIndex), 
                        ifConditionalJumpIndex);
                }

                foreach (var i in ifEndIndices)
                {
                    instructions.SetAt(new LjsInstruction(
                        LjsInstructionCode.Jump, ifBlockEndIndex), i);
                }
                
                LjsCompileUtils.ReleaseTemporaryIntList(ifEndIndices);
                
                break;
            
            case LjsAstForLoop forLoop:

                var forLoopContext = context.CreateLocalContext();
                
                ProcessNode(forLoop.InitExpression, forLoopContext);
                
                var loopStartIndex = instructions.Count;
                var loopConditionalJumpIndex = -1;
                
                if (forLoop.Condition != LjsAstEmptyNode.Instance)
                {
                    ProcessNode(forLoop.Condition, forLoopContext);
                
                    loopConditionalJumpIndex = instructions.Count;
                    instructions.Add(default);
                }
                
                // for break statements inside
                var loopEndIndices = LjsCompileUtils.GetTemporaryIntList();
                var loopContinueIndices = LjsCompileUtils.GetTemporaryIntList();

                ProcessNode(forLoop.Body, forLoopContext,
                    loopContinueIndices, loopEndIndices);

                var loopIteratorIndex = instructions.Count;
                
                ProcessNode(forLoop.IterationExpression, forLoopContext);
                
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.Jump, loopStartIndex));

                var loopEndIndex = instructions.Count;

                if (loopConditionalJumpIndex != -1)
                {
                    instructions.SetAt(new LjsInstruction(
                            LjsInstructionCode.JumpIfFalse, loopEndIndex), 
                        loopConditionalJumpIndex);
                }
                
                foreach (var i in loopContinueIndices)
                {
                    instructions.SetAt(new LjsInstruction(
                        LjsInstructionCode.Jump, loopIteratorIndex), i);
                }
                
                foreach (var i in loopEndIndices)
                {
                    instructions.SetAt(new LjsInstruction(
                        LjsInstructionCode.Jump, loopEndIndex), i);
                }
                
                LjsCompileUtils.ReleaseTemporaryIntList(loopEndIndices);
                LjsCompileUtils.ReleaseTemporaryIntList(loopContinueIndices);
                break;
            
            case LjsAstWhileLoop whileLoop:
                
                var whileStartIndex = instructions.Count;
                
                ProcessNode(whileLoop.Condition, context);

                var whileConditionalJumpIndex = instructions.Count;
                instructions.Add(default);
                
                // for break statements inside
                var whileEndIndices = LjsCompileUtils.GetTemporaryIntList();
                var whileContinueIndices = LjsCompileUtils.GetTemporaryIntList();
                
                ProcessNode(whileLoop.Body, context.CreateLocalContext(), 
                    whileContinueIndices, whileEndIndices);
                
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.Jump, whileStartIndex));

                var whileEndIndex = instructions.Count;

                instructions.SetAt(new LjsInstruction(
                    LjsInstructionCode.JumpIfFalse, whileEndIndex), 
                    whileConditionalJumpIndex);

                SetJumps(whileContinueIndices, whileStartIndex, instructions);
                SetJumps(whileEndIndices, whileEndIndex, instructions);
                
                LjsCompileUtils.ReleaseTemporaryIntList(whileEndIndices);
                LjsCompileUtils.ReleaseTemporaryIntList(whileContinueIndices);
                break;
            
            case LjsAstSequence sequence:

                foreach (var n in 
                         sequence.ChildNodes.OfType<LjsAstNamedFunctionDeclaration>())
                {
                    context.CreateNamedFunctionContext(n);
                }
                
                foreach (var n in sequence.ChildNodes)
                {
                    ProcessNode(n, context, 
                        jumpToTheStartPlaceholdersIndices, jumpToTheEndPlaceholdersIndices);
                }
                
                break;
            
            case LjsAstGetNamedProperty astGetNamedProperty:

                ProcessNode(astGetNamedProperty.PropertySource, context);
                
                instructions.Add(GetStringConstInstruction(astGetNamedProperty.PropertyName));

                instructions.Add(new LjsInstruction(LjsInstructionCode.GetProp));
                break;
            
            case LjsAstGetProperty astGetProperty:

                ProcessNode(astGetProperty.PropertySource, context);
                ProcessNode(astGetProperty.PropertyName, context);

                instructions.Add(new LjsInstruction(LjsInstructionCode.GetProp));
                break;
            
            case LjsAstSetNamedProperty astSetNamedProperty:
                
                if (astSetNamedProperty.AssignMode == LjsAstAssignMode.Normal)
                {
                    ProcessNode(astSetNamedProperty.AssignmentExpression, context);
                }
                else
                {
                    ProcessNode(astSetNamedProperty.PropertySource, context);
                    instructions.Add(GetStringConstInstruction(astSetNamedProperty.PropertyName));
                    instructions.Add(new LjsInstruction(LjsInstructionCode.GetProp));
                    
                    ProcessNode(astSetNamedProperty.AssignmentExpression, context);
                    
                    instructions.Add(new LjsInstruction(
                        LjsCompileUtils.GetComplexAssignmentOpCode(astSetNamedProperty.AssignMode)));
                }
                
                ProcessNode(astSetNamedProperty.PropertySource, context);
                instructions.Add(GetStringConstInstruction(astSetNamedProperty.PropertyName));
                
                instructions.Add(new LjsInstruction(LjsInstructionCode.SetProp));
                
                break;
            
            case LjsAstSetProperty astSetProperty:

                if (astSetProperty.AssignMode == LjsAstAssignMode.Normal)
                {
                    ProcessNode(astSetProperty.AssignmentExpression, context);
                }
                else
                {
                    ProcessNode(astSetProperty.PropertySource, context);
                    ProcessNode(astSetProperty.PropertyName, context);
                    instructions.Add(new LjsInstruction(LjsInstructionCode.GetProp));
                    
                    ProcessNode(astSetProperty.AssignmentExpression, context);
                    
                    instructions.Add(new LjsInstruction(
                        LjsCompileUtils.GetComplexAssignmentOpCode(astSetProperty.AssignMode)));
                }
                
                ProcessNode(astSetProperty.PropertySource, context);
                ProcessNode(astSetProperty.PropertyName, context);
                
                instructions.Add(new LjsInstruction(LjsInstructionCode.SetProp));
                
                break;
            
            case LjsAstObjectLiteral objectLiteral:
                
                foreach (var prop in objectLiteral.ChildNodes)
                {
                    ProcessNode(prop.Value, context);
                    instructions.Add(GetStringConstInstruction(prop.Name));
                }
                
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.NewDictionary, objectLiteral.Count));
                
                break;
            
            case LjsAstArrayLiteral arrayLiteral:
                
                foreach (var e in arrayLiteral.ChildNodes)
                {
                    ProcessNode(e, context);
                }
                
                instructions.Add(new LjsInstruction(LjsInstructionCode.NewArray, arrayLiteral.Count));
                
                break;
            
            case LjsAstGetThis _:
                
                instructions.Add(new LjsInstruction(LjsInstructionCode.GetThis));
                
                break;
            
            case LjsAstSwitchBlock switchBlock:
                var body = switchBlock.Body;
                
                if (!body.IsEmpty)
                {
                    

                    var switchContext = context.CreateLocalContext();

                    var switchBreakIndices = LjsCompileUtils.GetTemporaryIntList();
                    var caseFalseJumpIndices = LjsCompileUtils.GetTemporaryIntList();
                    var caseTrueJumpIndices = LjsCompileUtils.GetTemporaryIntList();

                    var defaultIsReached = false;
                    var blockEnd = false;
                    var prevNode = LjsAstEmptyNode.Instance;
                    
                    for (var i = 0; i < body.Count && !blockEnd; i++)
                    {
                        var n = body[i];
                        switch (n)
                        {
                            case LjsAstSwitchCase switchCase:
                                if (defaultIsReached) 
                                    ThrowUnexpectedSwitchCase(switchCase);

                                if (prevNode is LjsAstSwitchCase)
                                {
                                    caseTrueJumpIndices.Add(instructions.Count);
                                    instructions.Add(default);
                                }
                                
                                SetFalseJumps(caseFalseJumpIndices, instructions.Count, instructions);
                                caseFalseJumpIndices.Clear();
                                
                                ProcessNode(switchBlock.Expression, context);
                                ProcessNode(switchCase.Value, switchContext);
                                instructions.Add(new LjsInstruction(LjsInstructionCode.Eq));
                                
                                caseFalseJumpIndices.Add(instructions.Count);
                                instructions.Add(default);
                                break;
                            
                            case LjsAstBreak _:
                                if (defaultIsReached)
                                {
                                    blockEnd = true;
                                }
                                else
                                {
                                    SetJumps(caseTrueJumpIndices, instructions.Count, instructions);
                                    caseTrueJumpIndices.Clear();
                                    
                                    switchBreakIndices.Add(instructions.Count);
                                    instructions.Add(default);
                                }
                                break;
                            
                            case LjsAstSwitchDefault _:
                                defaultIsReached = true;

                                SetFalseJumps(caseFalseJumpIndices, instructions.Count, instructions);
                                caseFalseJumpIndices.Clear();
                                
                                SetJumps(caseTrueJumpIndices, instructions.Count, instructions);
                                caseTrueJumpIndices.Clear();
                                
                                break;
                            
                            default:
                                
                                SetJumps(caseTrueJumpIndices, instructions.Count, instructions);
                                caseTrueJumpIndices.Clear();
                                
                                ProcessNode(
                                    n, switchContext, 
                                    null, 
                                    switchBreakIndices);
                                
                                break;
                        }

                        prevNode = n;
                    }

                    SetJumps(caseTrueJumpIndices, instructions.Count, instructions);
                    SetJumps(switchBreakIndices, instructions.Count, instructions);
                    SetFalseJumps(caseFalseJumpIndices, instructions.Count, instructions);
                    
                    LjsCompileUtils.ReleaseTemporaryIntList(caseTrueJumpIndices);
                    LjsCompileUtils.ReleaseTemporaryIntList(caseFalseJumpIndices);
                    LjsCompileUtils.ReleaseTemporaryIntList(switchBreakIndices);
                }
                break;
            
            
            default:
                throw new LjsCompilerError($"unsupported ast node {node.GetType().Name}");
        }
    }

    private static void SetFalseJumps(
        IReadOnlyList<int> indices, int jumpIndex, LjsInstructionsList instructionsList)
    {
        for (var i = 0; i < indices.Count; i++)
        {
            var j = indices[i];
            instructionsList.SetAt(new LjsInstruction(LjsInstructionCode.JumpIfFalse, jumpIndex), j);
        }
    }
    
    private static void SetJumps(
        IReadOnlyList<int> indices, int jumpIndex, LjsInstructionsList instructionsList)
    {
        for (var i = 0; i < indices.Count; i++)
        {
            var j = indices[i];
            instructionsList.SetAt(new LjsInstruction(LjsInstructionCode.Jump, jumpIndex), j);
        }
    }

    private LjsInstruction GetStringConstInstruction(string s) => !string.IsNullOrEmpty(s)
        ? new LjsInstruction(LjsInstructionCode.ConstString, _constants.AddStringConstant(s))
        : new LjsInstruction(LjsInstructionCode.ConstStringEmpty);
    
    
    /////////// vars init/load/store
    
    private void AddVarLoadInstruction(LjsCompilerContext data, LjsAstGetVar getVar)
    {
        var instructions = data.FunctionContext.Instructions;

        if (data.Locals.Has(getVar.VarName))
        {
            instructions.Add(new LjsInstruction(
                LjsInstructionCode.VarLoad, data.Locals.GetPointer(getVar.VarName).Index));
        }
        else if (data.HasFunctionWithName(getVar.VarName))
        {
            instructions.Add(new LjsInstruction(LjsInstructionCode.FuncRef, data.GetFunctionIndex(getVar.VarName)));
        }
        else if (data.HasLocalInHierarchy(getVar.VarName))
        {
            var (localVarPointer, functionIndex) = data.GetLocalInHierarchy(getVar.VarName);
            var instructionArg = LjsRuntimeUtils.CombineTwoShorts(localVarPointer.Index, functionIndex);
            
            instructions.Add(new LjsInstruction(LjsInstructionCode.ParentVarLoad, instructionArg));
            
        }
        else
        {
            instructions.Add(new LjsInstruction(
                LjsInstructionCode.ExtLoad, _constants.AddStringConstant(getVar.VarName)));
        }
    }

    private LjsInstruction CreateVarLoadInstruction(string varName, LjsCompilerContext data)
    {

        if (data.Locals.Has(varName)) 
            return new LjsInstruction(
                LjsInstructionCode.VarLoad, data.Locals.GetPointer(varName).Index);

        if (data.HasLocalInHierarchy(varName))
        {
            var (localVarPointer, functionIndex) = data.GetLocalInHierarchy(varName);
            
            var instructionArg = LjsRuntimeUtils.CombineTwoShorts(
                localVarPointer.Index, functionIndex);
            
            return new LjsInstruction(LjsInstructionCode.ParentVarLoad, instructionArg);
        }
        
        return new LjsInstruction(
                LjsInstructionCode.ExtLoad, _constants.AddStringConstant(varName));
    }
    
    private LjsInstruction CreateVarStoreInstruction(
        string varName, LjsCompilerContext data, ILjsAstNode node)
    {
        
        if (data.Locals.Has(varName))
        {
            var localVarPointer = data.Locals.GetPointer(varName);
            
            AssertPointerIsWritable(localVarPointer, node);

            return new LjsInstruction(LjsInstructionCode.VarStore, localVarPointer.Index);
        } 
            
        
        if (data.HasLocalInHierarchy(varName))
        {
            var (localPointer, functionIndex) = data.GetLocalInHierarchy(varName);

            AssertPointerIsWritable(localPointer, node);

            var instructionArg = LjsRuntimeUtils.CombineTwoShorts(localPointer.Index, functionIndex);
            
            return new LjsInstruction(LjsInstructionCode.ParentVarStore, instructionArg);
        }
        
        return new LjsInstruction(
                LjsInstructionCode.ExtStore, _constants.AddStringConstant(varName));
    }

    private void AssertPointerIsWritable(LjsLocalVarPointer varPointer, ILjsAstNode node)
    {
        if (varPointer.VarKind == LjsLocalVarKind.Const)
        {
            throw new LjsCompilerError($"invalid const {varPointer.Name} assign");
            // todo add token position info
        }
    }
    
    private LjsInstruction CreateVarInitInstruction(
        string varName, LjsCompilerContext data, ILjsAstNode node)
    {

        if (data.Locals.Has(varName))
        {
            var varPointer = data.Locals.GetPointer(varName);

            AssertPointerIsWritable(varPointer, node);
            
            return new LjsInstruction(LjsInstructionCode.VarInit, varPointer.Index);
        }
        
        if (data.HasLocalInHierarchy(varName))
        {
            var (localVarPointer, functionIndex) = data.GetLocalInHierarchy(varName);
            
            AssertPointerIsWritable(localVarPointer, node);
            
            var instructionArg = LjsRuntimeUtils.CombineTwoShorts(
                localVarPointer.Index, functionIndex);
            
            return new LjsInstruction(LjsInstructionCode.ParentVarInit, instructionArg);
        }
        
        return new LjsInstruction(
                LjsInstructionCode.ExtStore, _constants.AddStringConstant(varName));
    }
    
    private void AddVarIncrementInstruction(LjsCompilerContext data, LjsAstIncrementVar incrementVar)
    {
        var instructions = data.FunctionContext.Instructions;

        var varLoadInstruction = CreateVarLoadInstruction(incrementVar.VarName, data);
                
        if (incrementVar.Order == LjsAstIncrementOrder.Postfix)
        {
            // we leave old var value on stack
            instructions.Add(varLoadInstruction);
        }
        
        instructions.Add(varLoadInstruction);
        instructions.Add(new LjsInstruction(LjsInstructionCode.ConstInt, 1));
        instructions.Add(new LjsInstruction(LjsCompileUtils.GetIncrementOpCode(incrementVar.Sign)));
                
        switch (incrementVar.Order)
        {
            case LjsAstIncrementOrder.Prefix:
                instructions.Add(CreateVarStoreInstruction(incrementVar.VarName, data, incrementVar));
                break;
                    
            case LjsAstIncrementOrder.Postfix:
                instructions.Add(CreateVarInitInstruction(incrementVar.VarName, data, incrementVar));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void AddVarStoreInstructions(LjsCompilerContext data, LjsAstSetVar setVar)
    {
        var instructions = data.FunctionContext.Instructions;

        var isLocal = data.Locals.Has(setVar.VarName);
                
        if (!isLocal && data.HasFunctionWithName(setVar.VarName))
        {
            throw new LjsCompilerError($"named function assign {setVar.VarName}");
        }
                
        if (setVar.AssignMode == LjsAstAssignMode.Normal)
        {
            ProcessNode(setVar.Expression, data);
        }
        else
        {
            instructions.Add(CreateVarLoadInstruction(setVar.VarName, data));
                    
            ProcessNode(setVar.Expression, data);
                    
            instructions.Add(new LjsInstruction(
                LjsCompileUtils.GetComplexAssignmentOpCode(setVar.AssignMode)));
        }
        
        instructions.Add(CreateVarStoreInstruction(setVar.VarName, data, setVar));
    }
    
    
    // ERRORS

    private void AssertVariableNameIsUniq(
        LjsAstVariableDeclaration variableDeclaration, LjsCompilerContext context)
    {
        var varKind = LjsCompileUtils.GetVarKind(variableDeclaration.VariableKind);

        if (varKind == LjsLocalVarKind.Let)
        {
            if (context.Locals.Has(variableDeclaration.Name, false))
            {
                throw new LjsCompilerError($"duplicate {varKind} name {variableDeclaration.Name}");
            }
        }
        else
        {
            if (context.Locals.Has(variableDeclaration.Name))
            {
                throw new LjsCompilerError($"duplicate {varKind} name {variableDeclaration.Name}");
            }
        }

        
    }

    private void AssertArgumentNameIsUniq(
        string argumentName,
        LjsAstFunctionDeclaration functionDeclaration,
        LjsCompilerContext context)
    {
        if (context.Locals.Has(argumentName))
        {
            throw new LjsCompilerError($"duplicate argument name {argumentName}");
        }
    }

    private void AssertBreakStatementHasJumpPoint(LjsAstBreak node, ICollection<int>? jumpIndices)
    {
        if (jumpIndices == null)
            throw new LjsCompilerError("unexpected break statement");
    }
    
    private void AssertContinueStatementHasJumpPoint(LjsAstContinue node, ICollection<int>? jumpIndices)
    {
        if (jumpIndices == null)
            throw new LjsCompilerError("unexpected continue statement");
    }

    private LjsCompilerError CreateUnknownUnaryOperatorException(LjsAstUnaryOperation unaryOperation) => new(
            $"unsupported unary operator type {unaryOperation.OperatorType}");

    private void ThrowUnexpectedSwitchCase(ILjsAstNode node)
    {
        throw new LjsCompilerError("unexpected switch case after default");
    }
}