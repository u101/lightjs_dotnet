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
    private readonly Dictionary<string, int> _namedFunctionsMap = new();
    private readonly List<FunctionData> _functionsList = new();

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
        var context = new FunctionData(0);
        
        _functionsList.Add(context);
        
        ProcessNode(_astModel.RootNode, context);

        context.Instructions.Add(
            new LjsInstruction(LjsInstructionCode.Halt));

        var functions = _functionsList.Select(fd => new LjsFunctionData(
            fd.FunctionIndex,
            fd.Instructions.Instructions.ToArray(), 
            fd.Args.ToArray(), 
            fd.LocalVars.ToArray()
        )).ToArray();
        
        return new LjsProgram(
            _constants, functions, _namedFunctionsMap);
    }

    private sealed class InstructionsList
    {
        private readonly List<LjsInstruction> _instructions = new();
    
        public IReadOnlyList<LjsInstruction> Instructions => _instructions;
    
        public int Count => _instructions.Count;

        public void Add(LjsInstruction instruction)
        {
            _instructions.Add(instruction);
        }

        public void SetAt(LjsInstruction instruction, int index)
        {
            _instructions[index] = instruction;
        }

        public LjsInstruction LastInstruction => 
            _instructions.Count > 0 ? _instructions[^1] : default;
    }
    
    private sealed class FunctionData
    {
        private readonly FunctionData? _parentData;
        public InstructionsList Instructions { get; } = new();
        public int FunctionIndex { get; }
        
        public List<LjsFunctionArgument> Args { get; } = new();
        public List<LjsLocalVarPointer> LocalVars => _localVars;

        private readonly Dictionary<string, int> _localVarIndices = new();
        private readonly List<LjsLocalVarPointer> _localVars = new();

        public FunctionData(int functionIndex)
        {
            FunctionIndex = functionIndex;
            _parentData = null;
        }

        private FunctionData(int functionIndex, FunctionData parentData)
        {
            FunctionIndex = functionIndex;
            _parentData = parentData;
        }

        public int AddLocal(string name)
        {
            if (_localVarIndices.ContainsKey(name))
                throw new LjsCompilerError($"duplicate var name {name}");
            
            var index = _localVars.Count;
            _localVars.Add(new LjsLocalVarPointer(index, name));
            _localVarIndices[name] = index;
            return index;
        }

        public bool HasLocal(string name) => _localVarIndices.ContainsKey(name);

        public int GetLocal(string name) => _localVarIndices.TryGetValue(name, out var i) ? i : -1;

        public bool HasLocalInHierarchy(string name) => _localVarIndices.ContainsKey(name) ||
                                                        (_parentData != null &&
                                                         _parentData.HasLocalInHierarchy(name));

        public (int, int) GetLocalInHierarchy(string name)
        {
            if (_localVarIndices.TryGetValue(name, out var i))
            {
                return (i, FunctionIndex);
            }

            return _parentData?.GetLocalInHierarchy(name) ?? (-1, -1);
        }

        public FunctionData CreateChild(int functionIndex) => 
            new(functionIndex, this);
    }

    private void ProcessFunction(
        LjsAstFunctionDeclaration functionDeclaration, 
        FunctionData functionData)
    {

        var parameters = functionDeclaration.Parameters;

        for (var i = 0; i < parameters.Length; i++)
        {
            var p = parameters[i];
            var defaultValue = GetFunctionParameterDefaultValue(p.DefaultValue);
            
            functionData.Args.Add(new LjsFunctionArgument(p.Name, defaultValue));

            functionData.AddLocal(p.Name);
        }
        
        ProcessNode(functionDeclaration.FunctionBody, functionData);

        if (functionData.Instructions.Count == 0 ||
            functionData.Instructions.LastInstruction.Code != LjsInstructionCode.Return)
        {
            functionData.Instructions.Add(new LjsInstruction(LjsInstructionCode.ConstUndef));
            functionData.Instructions.Add(new LjsInstruction(LjsInstructionCode.Return));
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

    private FunctionData CreateNamedFunction(
        LjsAstNamedFunctionDeclaration namedFunctionDeclaration, 
        FunctionData parentFunction)
    {
        var namedFunctionIndex = _functionsList.Count;
        var namedFunc = parentFunction.CreateChild(namedFunctionIndex);

        _functionsList.Add(namedFunc);
        _namedFunctionsMap[namedFunctionDeclaration.Name] = namedFunctionIndex;
        return namedFunc;
    }
    
    private FunctionData CreateAnonFunction(FunctionData parentFunction)
    {
        var functionIndex = _functionsList.Count;
        var func = parentFunction.CreateChild(functionIndex);

        _functionsList.Add(func);
        return func;
    }

    private FunctionData GetNamedFunction(LjsAstNamedFunctionDeclaration namedFunctionDeclaration) =>
        _functionsList[_namedFunctionsMap[namedFunctionDeclaration.Name]];

    private (FunctionData, int) GetOrCreateNamedFunctionData(
        LjsAstNamedFunctionDeclaration namedFunctionDeclaration,
        FunctionData parentFunction)
    {
        if (_namedFunctionsMap.ContainsKey(namedFunctionDeclaration.Name))
        {
            var functionIndex = _namedFunctionsMap[namedFunctionDeclaration.Name];
            var functionData = _functionsList[functionIndex];
            return (functionData, functionIndex);
        }
        else
        {
            var functionIndex = _functionsList.Count;
            var functionData = CreateNamedFunction(namedFunctionDeclaration, parentFunction);
            return (functionData, functionIndex);
        }
    }

    private void ProcessNode(
        ILjsAstNode node, 
        FunctionData functionData,
        ICollection<int>? jumpToTheStartPlaceholdersIndices = null, 
        ICollection<int>? jumpToTheEndPlaceholdersIndices = null)
    {
        var instructions = functionData.Instructions;

        switch (node)
        {
            case LjsAstAnonymousFunctionDeclaration funcDeclaration:
                
                var anonFunc = CreateAnonFunction(functionData);
                
                ProcessFunction(funcDeclaration, anonFunc);
                
                instructions.Add(new LjsInstruction(LjsInstructionCode.FuncRef, anonFunc.FunctionIndex));
                
                break;
            
            case LjsAstNamedFunctionDeclaration namedFunctionDeclaration:
                
                var (namedFunc, namedFuncIndex) = 
                    GetOrCreateNamedFunctionData(namedFunctionDeclaration, functionData);

                if (namedFunc != GetNamedFunction(namedFunctionDeclaration))
                {
                    throw new LjsCompilerError($"duplicate function names {namedFunctionDeclaration.Name}");
                }
                
                ProcessFunction(namedFunctionDeclaration, namedFunc);
                break;
            
            case LjsAstFunctionCall functionCall:

                var specifiedArgumentsCount = functionCall.Arguments.Count;
                
                foreach (var n in functionCall.Arguments.ChildNodes)
                {
                    ProcessNode(n, functionData);
                }
                
                ProcessNode(functionCall.FunctionGetter, functionData);
                
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.FuncCall, specifiedArgumentsCount));
                
                break;
            
            case LjsAstReturn astReturn:

                if (astReturn.ReturnValue != LjsAstEmptyNode.Instance)
                {
                    ProcessNode(astReturn.ReturnValue, functionData);
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
            
            case LjsAstBreak _:
                
                if (jumpToTheEndPlaceholdersIndices == null)
                    throw new LjsCompilerError("unexpected break statement");
                
                jumpToTheEndPlaceholdersIndices.Add(instructions.Count);
                instructions.Add(default);
                
                break;
            
            case LjsAstContinue _:
                if (jumpToTheStartPlaceholdersIndices == null)
                    throw new LjsCompilerError("unexpected continue statement");
                jumpToTheStartPlaceholdersIndices.Add(instructions.Count);
                instructions.Add(default);
                break;
            
            case LjsAstLiteral<int> lit:
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.ConstInt, lit.Value));
                break;
            
            case LjsAstLiteral<double> lit:
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.ConstDouble, 
                    _constants.AddDoubleConstant(lit.Value)));
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
                
                ProcessNode(binaryOperation.LeftOperand, functionData);
                ProcessNode(binaryOperation.RightOperand, functionData);

                instructions.Add(new LjsInstruction(LjsCompileUtils.GetBinaryOpCode(binaryOperation.OperatorType)));
                
                break;
            
            case LjsAstUnaryOperation unaryOperation:

                switch (unaryOperation.OperatorType)
                {
                    case LjsAstUnaryOperationType.Plus:
                        // just skip, because unary plus does nothing
                        ProcessNode(unaryOperation.Operand, functionData);
                        break;
                    
                    case LjsAstUnaryOperationType.Minus:
                        ProcessNode(unaryOperation.Operand, functionData);
                        instructions.Add(new LjsInstruction(LjsInstructionCode.Minus));
                        break;
                    case LjsAstUnaryOperationType.LogicalNot:
                        ProcessNode(unaryOperation.Operand, functionData);
                        instructions.Add(new LjsInstruction(LjsInstructionCode.Not));
                        break;
                    
                    case LjsAstUnaryOperationType.BitNot:
                        ProcessNode(unaryOperation.Operand, functionData);
                        instructions.Add(new LjsInstruction(LjsInstructionCode.BitNot));
                        break;
                    
                    default:
                        throw new LjsCompilerError(
                            $"unsupported unary operator type {unaryOperation.OperatorType}");
                }
                
                break;
            
            case LjsAstVariableDeclaration variableDeclaration:

                var varIndex = functionData.AddLocal(variableDeclaration.Name);

                if (variableDeclaration.Value != LjsAstEmptyNode.Instance)
                {
                    ProcessNode(variableDeclaration.Value, functionData);
                    
                    instructions.Add(new LjsInstruction(LjsInstructionCode.VarInit, varIndex));
                }
                
                break;
            
            case LjsAstGetVar getVar:
                AddVarLoadInstruction(functionData, getVar);
                break;
            
            case LjsAstSetVar setVar:
                AddVarStoreInstructions(functionData, setVar);
                break;
            
            case LjsAstIncrementVar incrementVar:
                AddVarIncrementInstruction(functionData, incrementVar);
                break;
            
            case LjsAstIfBlock ifBlock:
                
                ProcessNode(ifBlock.MainBlock.Condition, functionData);
                
                // indices of empty placeholder instructions to be replaced with actual jump instructions 
                var ifEndIndices = LjsCompileUtils.GetTemporaryIntList();

                var ifConditionalJumpIndex = instructions.Count;
                
                // if false jump to next condition or to the else block or to the end
                instructions.Add(default);
                
                ProcessNode(ifBlock.MainBlock.Expression, functionData, 
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
                        
                        ProcessNode(alternative.Condition, functionData);
                        
                        ifConditionalJumpIndex = instructions.Count;
                        instructions.Add(default);
                        
                        ProcessNode(alternative.Expression, functionData,
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
                    
                    ProcessNode(ifBlock.ElseBlock, functionData,
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
                
                ProcessNode(forLoop.InitExpression, functionData);
                
                var loopStartIndex = instructions.Count;
                var loopConditionalJumpIndex = -1;
                
                if (forLoop.Condition != LjsAstEmptyNode.Instance)
                {
                    ProcessNode(forLoop.Condition, functionData);
                
                    loopConditionalJumpIndex = instructions.Count;
                    instructions.Add(default);
                }
                
                // for break statements inside
                var loopEndIndices = LjsCompileUtils.GetTemporaryIntList();
                var loopContinueIndices = LjsCompileUtils.GetTemporaryIntList();

                ProcessNode(forLoop.Body, functionData,
                    loopContinueIndices, loopEndIndices);

                var loopIteratorIndex = instructions.Count;
                
                ProcessNode(forLoop.IterationExpression, functionData);
                
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
                
                ProcessNode(whileLoop.Condition, functionData);

                var whileConditionalJumpIndex = instructions.Count;
                instructions.Add(default);
                
                // for break statements inside
                var whileEndIndices = LjsCompileUtils.GetTemporaryIntList();
                var whileContinueIndices = LjsCompileUtils.GetTemporaryIntList();
                
                ProcessNode(whileLoop.Body, functionData, 
                    whileContinueIndices, whileEndIndices);
                
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.Jump, whileStartIndex));

                var whileEndIndex = instructions.Count;

                instructions.SetAt(new LjsInstruction(
                    LjsInstructionCode.JumpIfFalse, whileEndIndex), 
                    whileConditionalJumpIndex);

                foreach (var i in whileContinueIndices)
                {
                    instructions.SetAt(new LjsInstruction(
                        LjsInstructionCode.Jump, whileStartIndex), i);
                }
                
                foreach (var i in whileEndIndices)
                {
                    instructions.SetAt(new LjsInstruction(
                        LjsInstructionCode.Jump, whileEndIndex), i);
                }
                
                LjsCompileUtils.ReleaseTemporaryIntList(whileEndIndices);
                LjsCompileUtils.ReleaseTemporaryIntList(whileContinueIndices);
                break;
            
            case LjsAstSequence sequence:

                foreach (var n in 
                         sequence.ChildNodes.OfType<LjsAstNamedFunctionDeclaration>())
                {
                    CreateNamedFunction(n, functionData);
                }
                
                foreach (var n in sequence.ChildNodes)
                {
                    ProcessNode(n, functionData, 
                        jumpToTheStartPlaceholdersIndices, jumpToTheEndPlaceholdersIndices);
                }
                
                break;
            
            case LjsAstGetNamedProperty astGetNamedProperty:

                ProcessNode(astGetNamedProperty.PropertySource, functionData);
                
                instructions.Add(GetStringConstInstruction(astGetNamedProperty.PropertyName));

                instructions.Add(new LjsInstruction(LjsInstructionCode.GetProp));
                break;
            
            case LjsAstGetProperty astGetProperty:

                ProcessNode(astGetProperty.PropertySource, functionData);
                ProcessNode(astGetProperty.PropertyName, functionData);

                instructions.Add(new LjsInstruction(LjsInstructionCode.GetProp));
                break;
            
            case LjsAstSetNamedProperty astSetNamedProperty:
                
                if (astSetNamedProperty.AssignMode == LjsAstAssignMode.Normal)
                {
                    ProcessNode(astSetNamedProperty.AssignmentExpression, functionData);
                }
                else
                {
                    ProcessNode(astSetNamedProperty.PropertySource, functionData);
                    instructions.Add(GetStringConstInstruction(astSetNamedProperty.PropertyName));
                    instructions.Add(new LjsInstruction(LjsInstructionCode.GetProp));
                    
                    ProcessNode(astSetNamedProperty.AssignmentExpression, functionData);
                    
                    instructions.Add(new LjsInstruction(
                        LjsCompileUtils.GetComplexAssignmentOpCode(astSetNamedProperty.AssignMode)));
                }
                
                ProcessNode(astSetNamedProperty.PropertySource, functionData);
                instructions.Add(GetStringConstInstruction(astSetNamedProperty.PropertyName));
                
                instructions.Add(new LjsInstruction(LjsInstructionCode.SetProp));
                
                break;
            
            case LjsAstSetProperty astSetProperty:

                if (astSetProperty.AssignMode == LjsAstAssignMode.Normal)
                {
                    ProcessNode(astSetProperty.AssignmentExpression, functionData);
                }
                else
                {
                    ProcessNode(astSetProperty.PropertySource, functionData);
                    ProcessNode(astSetProperty.PropertyName, functionData);
                    instructions.Add(new LjsInstruction(LjsInstructionCode.GetProp));
                    
                    ProcessNode(astSetProperty.AssignmentExpression, functionData);
                    
                    instructions.Add(new LjsInstruction(
                        LjsCompileUtils.GetComplexAssignmentOpCode(astSetProperty.AssignMode)));
                }
                
                ProcessNode(astSetProperty.PropertySource, functionData);
                ProcessNode(astSetProperty.PropertyName, functionData);
                
                instructions.Add(new LjsInstruction(LjsInstructionCode.SetProp));
                
                break;
            
            case LjsAstObjectLiteral objectLiteral:
                
                foreach (var prop in objectLiteral.ChildNodes)
                {
                    ProcessNode(prop.Value, functionData);
                    instructions.Add(GetStringConstInstruction(prop.Name));
                }
                
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.NewDictionary, objectLiteral.Count));
                
                break;
            
            case LjsAstArrayLiteral arrayLiteral:
                
                foreach (var e in arrayLiteral.ChildNodes)
                {
                    ProcessNode(e, functionData);
                }
                
                instructions.Add(new LjsInstruction(LjsInstructionCode.NewArray, arrayLiteral.Count));
                
                break;
            
            case LjsAstGetThis _:
                
                instructions.Add(new LjsInstruction(LjsInstructionCode.GetThis));
                
                break;
            
            
            default:
                throw new LjsCompilerError($"unsupported ast node {node.GetType().Name}");
        }
    }

    private LjsInstruction GetStringConstInstruction(string s) => new(
        LjsInstructionCode.ConstString,
        _constants.AddStringConstant(s));
    
    
    /////////// vars init/load/store
    
    private void AddVarLoadInstruction(FunctionData data, LjsAstGetVar getVar)
    {
        var instructions = data.Instructions;

        if (data.HasLocal(getVar.VarName))
        {
            instructions.Add(new LjsInstruction(
                LjsInstructionCode.VarLoad, data.GetLocal(getVar.VarName)));
        }
        else if (_namedFunctionsMap.ContainsKey(getVar.VarName))
        {
            instructions.Add(new LjsInstruction(LjsInstructionCode.FuncRef, _namedFunctionsMap[getVar.VarName]));
        }
        else if (data.HasLocalInHierarchy(getVar.VarName))
        {
            var (varIndex, functionIndex) = data.GetLocalInHierarchy(getVar.VarName);
            var instructionArg = LjsRuntimeUtils.CombineTwoShorts(varIndex, functionIndex);
            
            instructions.Add(new LjsInstruction(LjsInstructionCode.ParentVarLoad, instructionArg));
            
        }
        else
        {
            instructions.Add(new LjsInstruction(
                LjsInstructionCode.ExtLoad, _constants.AddStringConstant(getVar.VarName)));
        }
    }

    private LjsInstruction CreateVarLoadInstruction(string varName, FunctionData data)
    {
        var localVarIndex = data.GetLocal(varName);
        var isLocal = localVarIndex != -1;

        if (isLocal) 
            return new LjsInstruction(LjsInstructionCode.VarLoad, localVarIndex);

        if (data.HasLocalInHierarchy(varName))
        {
            var (varIndex, functionIndex) = data.GetLocalInHierarchy(varName);
            var instructionArg = LjsRuntimeUtils.CombineTwoShorts(varIndex, functionIndex);
            
            return new LjsInstruction(LjsInstructionCode.ParentVarLoad, instructionArg);
        }
        
        return new LjsInstruction(
                LjsInstructionCode.ExtLoad, _constants.AddStringConstant(varName));
    }
    
    private LjsInstruction CreateVarStoreInstruction(string varName, FunctionData data)
    {
        var localVarIndex = data.GetLocal(varName);
        var isLocal = localVarIndex != -1;
        
        if (isLocal) 
            return new LjsInstruction(LjsInstructionCode.VarStore, localVarIndex);
        
        if (data.HasLocalInHierarchy(varName))
        {
            var (varIndex, functionIndex) = data.GetLocalInHierarchy(varName);
            var instructionArg = LjsRuntimeUtils.CombineTwoShorts(varIndex, functionIndex);
            
            return new LjsInstruction(LjsInstructionCode.ParentVarStore, instructionArg);
        }
        
        return new LjsInstruction(
                LjsInstructionCode.ExtStore, _constants.AddStringConstant(varName));
    }
    
    private LjsInstruction CreateVarInitInstruction(string varName, FunctionData data)
    {
        var localVarIndex = data.GetLocal(varName);
        var isLocal = localVarIndex != -1;
        
        if (isLocal) 
            return new LjsInstruction(LjsInstructionCode.VarInit, localVarIndex);
        
        if (data.HasLocalInHierarchy(varName))
        {
            var (varIndex, functionIndex) = data.GetLocalInHierarchy(varName);
            var instructionArg = LjsRuntimeUtils.CombineTwoShorts(varIndex, functionIndex);
            
            return new LjsInstruction(LjsInstructionCode.ParentVarInit, instructionArg);
        }
        
        return new LjsInstruction(
                LjsInstructionCode.ExtStore, _constants.AddStringConstant(varName));
    }
    
    private void AddVarIncrementInstruction(FunctionData data, LjsAstIncrementVar incrementVar)
    {
        var instructions = data.Instructions;

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
                instructions.Add(CreateVarStoreInstruction(incrementVar.VarName, data));
                break;
                    
            case LjsAstIncrementOrder.Postfix:
                instructions.Add(CreateVarInitInstruction(incrementVar.VarName, data));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void AddVarStoreInstructions(FunctionData data, LjsAstSetVar setVar)
    {
        var instructions = data.Instructions;
        
        var localVarIndex = data.GetLocal(setVar.VarName);
        var isLocal = localVarIndex != -1;
                
        if (!isLocal && _namedFunctionsMap.ContainsKey(setVar.VarName))
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
        
        instructions.Add(CreateVarStoreInstruction(setVar.VarName, data));
    }
    
    
}