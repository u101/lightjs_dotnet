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
    private readonly List<LjsFunctionData> _functionsList = new();

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
        var mainFunc = new LjsFunctionData();
        
        _functionsList.Add(mainFunc);

        var context = new FunctionContext(mainFunc.InstructionsList, 0);
        
        ProcessNode(_astModel.RootNode, context);

        mainFunc.LocalsCount = context.LocalsCount;
        
        mainFunc.InstructionsList.Add(
            new LjsInstruction(LjsInstructionCode.Halt));
        
        return new LjsProgram(
            _constants, _functionsList, _namedFunctionsMap);
    }
    
    private class FunctionContext
    {
        private readonly FunctionContext? _parentContext;
        public LjsInstructionsList Instructions { get; }
        public int FunctionIndex { get; }

        private readonly Dictionary<string, int> _localVarIndices = new();
        
        public FunctionContext(LjsInstructionsList instructions, int functionIndex)
        {
            Instructions = instructions;
            FunctionIndex = functionIndex;
            _parentContext = null;
        }

        private FunctionContext(LjsInstructionsList instructions, int functionIndex, FunctionContext parentContext)
        {
            Instructions = instructions;
            FunctionIndex = functionIndex;
            _parentContext = parentContext;
        }

        public int LocalsCount => _localVarIndices.Count;

        public int AddLocal(string name)
        {
            var index = _localVarIndices.Count;
            _localVarIndices[name] = index;
            return index;
        }

        public bool HasLocal(string name) => _localVarIndices.ContainsKey(name);

        public int GetLocal(string name) => _localVarIndices.TryGetValue(name, out var i) ? i : -1;

        public bool HasLocalInHierarchy(string name) => _localVarIndices.ContainsKey(name) ||
                                                        (_parentContext != null &&
                                                         _parentContext.HasLocalInHierarchy(name));

        public (int, int) GetLocalInHierarchy(string name)
        {
            if (_localVarIndices.TryGetValue(name, out var i))
            {
                return (i, FunctionIndex);
            }

            return _parentContext?.GetLocalInHierarchy(name) ?? (-1, -1);
        }

        public FunctionContext CreateChild(LjsInstructionsList instructions, int functionIndex) => 
            new(instructions, functionIndex, this);
    }

    private void ProcessFunction(
        LjsFunctionData f, 
        LjsAstFunctionDeclaration functionDeclaration, 
        FunctionContext parentContext,
        int functionIndex)
    {
        
        var childContext = parentContext.CreateChild(f.InstructionsList, functionIndex);

        var parameters = functionDeclaration.Parameters;

        for (var i = 0; i < parameters.Length; i++)
        {
            var p = parameters[i];
            var defaultValue = GetFunctionParameterDefaultValue(p.DefaultValue);
            
            f.Args.Add(new LjsFunctionArg(p.Name, defaultValue));

            childContext.AddLocal(p.Name);
        }
        
        ProcessNode(functionDeclaration.FunctionBody, childContext);

        if (f.InstructionsList.Count == 0 ||
            f.InstructionsList.LastInstruction.Code != LjsInstructionCode.Return)
        {
            f.InstructionsList.Add(new LjsInstruction(LjsInstructionCode.ConstUndef));
            f.InstructionsList.Add(new LjsInstruction(LjsInstructionCode.Return));
        }

        f.LocalsCount = childContext.LocalsCount;
    }

    private static LjsObject GetFunctionParameterDefaultValue(ILjsAstNode node) => node switch
    {
        LjsAstNull _ => LjsObject.Null,
        LjsAstUndefined _ => LjsObject.Undefined,
        LjsAstLiteral<int> i => new LjsValue<int>(i.Value),
        LjsAstLiteral<double> i => new LjsValue<double>(i.Value),
        LjsAstLiteral<string> i => new LjsValue<string>(i.Value),
        LjsAstLiteral<bool> i => i.Value ? LjsValue.True : LjsValue.False,
        _ => LjsObject.Undefined
    };

    private LjsFunctionData CreateNamedFunction(LjsAstNamedFunctionDeclaration namedFunctionDeclaration)
    {
        var namedFunc = new LjsFunctionData();
        var namedFunctionIndex = _functionsList.Count;
                
        _functionsList.Add(namedFunc);
        _namedFunctionsMap[namedFunctionDeclaration.Name] = namedFunctionIndex;
        return namedFunc;
    }

    private LjsFunctionData GetNamedFunction(LjsAstNamedFunctionDeclaration namedFunctionDeclaration) =>
        _functionsList[_namedFunctionsMap[namedFunctionDeclaration.Name]];

    private (LjsFunctionData, int) GetOrCreateNamedFunctionData(LjsAstNamedFunctionDeclaration namedFunctionDeclaration)
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
            var functionData = CreateNamedFunction(namedFunctionDeclaration);
            return (functionData, functionIndex);
        }
    }

    private void ProcessNode(
        ILjsAstNode node, 
        FunctionContext context,
        int startIndex = -1, 
        ICollection<int>? jumpToTheEndPlaceholdersIndices = null)
    {
        var instructions = context.Instructions;

        switch (node)
        {
            case LjsAstAnonymousFunctionDeclaration funcDeclaration:
                
                var anonFunc = new LjsFunctionData();
                var anonFunctionIndex = _functionsList.Count;
                
                _functionsList.Add(anonFunc);
                
                ProcessFunction(anonFunc, funcDeclaration, context, anonFunctionIndex);
                
                instructions.Add(new LjsInstruction(LjsInstructionCode.FuncRef, anonFunctionIndex));
                
                break;
            
            case LjsAstNamedFunctionDeclaration namedFunctionDeclaration:
                var (namedFunc, namedFuncIndex) = GetOrCreateNamedFunctionData(namedFunctionDeclaration);

                if (namedFunc != GetNamedFunction(namedFunctionDeclaration))
                {
                    throw new LjsCompilerError($"duplicate function names {namedFunctionDeclaration.Name}");
                }
                
                ProcessFunction(namedFunc, namedFunctionDeclaration, context, namedFuncIndex);
                break;
            
            case LjsAstFunctionCall functionCall:

                var specifiedArgumentsCount = functionCall.Arguments.Count;
                
                foreach (var n in functionCall.Arguments)
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
            
            case LjsAstBreak _:
                
                if (jumpToTheEndPlaceholdersIndices == null)
                    throw new LjsCompilerError("unexpected break statement");
                
                jumpToTheEndPlaceholdersIndices.Add(instructions.Count);
                instructions.Add(default);
                
                break;
            
            case LjsAstContinue _:
                if (startIndex == -1)
                    throw new LjsCompilerError("unexpected continue statement");
                instructions.Add(new LjsInstruction(LjsInstructionCode.Jump, startIndex));
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
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.ConstString, 
                    _constants.AddStringConstant(lit.Value)));
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
                        throw new LjsCompilerError(
                            $"unsupported unary operator type {unaryOperation.OperatorType}");
                }
                
                break;
            
            case LjsAstVariableDeclaration variableDeclaration:

                var varIndex = context.AddLocal(variableDeclaration.Name);

                if (variableDeclaration.Value != LjsAstEmptyNode.Instance)
                {
                    ProcessNode(variableDeclaration.Value, context);
                    
                    instructions.Add(new LjsInstruction(LjsInstructionCode.VarInit, varIndex));
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
                
                ProcessNode(ifBlock.MainBlock.Expression, context, startIndex, jumpToTheEndPlaceholdersIndices);
                
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
                        
                        ProcessNode(alternative.Expression, context, startIndex, jumpToTheEndPlaceholdersIndices);
                        
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
                    
                    ProcessNode(ifBlock.ElseBlock, context, startIndex, jumpToTheEndPlaceholdersIndices);
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
            
            case LjsAstWhileLoop whileLoop:
                
                var whileStartIndex = instructions.Count;
                
                ProcessNode(whileLoop.Condition, context);

                var whileConditionalJumpIndex = instructions.Count;
                instructions.Add(default);
                
                // for break statements inside
                var whileEndIndices = LjsCompileUtils.GetTemporaryIntList();
                
                ProcessNode(whileLoop.Body, context, whileStartIndex, whileEndIndices);
                
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.Jump, whileStartIndex));

                var whileEndIndex = instructions.Count;

                instructions.SetAt(new LjsInstruction(
                    LjsInstructionCode.JumpIfFalse, whileEndIndex), 
                    whileConditionalJumpIndex);
                
                foreach (var i in whileEndIndices)
                {
                    instructions.SetAt(new LjsInstruction(
                        LjsInstructionCode.Jump, whileEndIndex), i);
                }
                
                LjsCompileUtils.ReleaseTemporaryIntList(whileEndIndices);
                break;
            
            case LjsAstSequence sequence:

                foreach (var n in 
                         sequence.ChildNodes.OfType<LjsAstNamedFunctionDeclaration>())
                {
                    CreateNamedFunction(n);
                }
                
                foreach (var n in sequence.ChildNodes)
                {
                    ProcessNode(n, context, startIndex, jumpToTheEndPlaceholdersIndices);
                }
                
                break;
            
            
            default:
                throw new LjsCompilerError("unsupported ast node");
        }
    }
    
    /////////// vars init/load/store
    
    private void AddVarLoadInstruction(FunctionContext context, LjsAstGetVar getVar)
    {
        var instructions = context.Instructions;

        if (context.HasLocal(getVar.VarName))
        {
            instructions.Add(new LjsInstruction(
                LjsInstructionCode.VarLoad, context.GetLocal(getVar.VarName)));
        }
        else if (_namedFunctionsMap.ContainsKey(getVar.VarName))
        {
            instructions.Add(new LjsInstruction(LjsInstructionCode.FuncRef, _namedFunctionsMap[getVar.VarName]));
        }
        else if (context.HasLocalInHierarchy(getVar.VarName))
        {
            var (varIndex, functionIndex) = context.GetLocalInHierarchy(getVar.VarName);
            var instructionArg = LjsRuntimeUtils.CombineLocalIndexAndFunctionIndex(varIndex, functionIndex);
            
            instructions.Add(new LjsInstruction(LjsInstructionCode.ParentVarLoad, instructionArg));
            
        }
        else
        {
            instructions.Add(new LjsInstruction(
                LjsInstructionCode.ExtLoad, _constants.AddStringConstant(getVar.VarName)));
        }
    }

    private LjsInstruction CreateVarLoadInstruction(string varName, FunctionContext context)
    {
        var localVarIndex = context.GetLocal(varName);
        var isLocal = localVarIndex != -1;

        if (isLocal) 
            return new LjsInstruction(LjsInstructionCode.VarLoad, localVarIndex);

        if (context.HasLocalInHierarchy(varName))
        {
            var (varIndex, functionIndex) = context.GetLocalInHierarchy(varName);
            var instructionArg = LjsRuntimeUtils.CombineLocalIndexAndFunctionIndex(varIndex, functionIndex);
            
            return new LjsInstruction(LjsInstructionCode.ParentVarLoad, instructionArg);
        }
        
        return new LjsInstruction(
                LjsInstructionCode.ExtLoad, _constants.AddStringConstant(varName));
    }
    
    private LjsInstruction CreateVarStoreInstruction(string varName, FunctionContext context)
    {
        var localVarIndex = context.GetLocal(varName);
        var isLocal = localVarIndex != -1;
        
        if (isLocal) 
            return new LjsInstruction(LjsInstructionCode.VarStore, localVarIndex);
        
        if (context.HasLocalInHierarchy(varName))
        {
            var (varIndex, functionIndex) = context.GetLocalInHierarchy(varName);
            var instructionArg = LjsRuntimeUtils.CombineLocalIndexAndFunctionIndex(varIndex, functionIndex);
            
            return new LjsInstruction(LjsInstructionCode.ParentVarStore, instructionArg);
        }
        
        return new LjsInstruction(
                LjsInstructionCode.ExtStore, _constants.AddStringConstant(varName));
    }
    
    private LjsInstruction CreateVarInitInstruction(string varName, FunctionContext context)
    {
        var localVarIndex = context.GetLocal(varName);
        var isLocal = localVarIndex != -1;
        
        if (isLocal) 
            return new LjsInstruction(LjsInstructionCode.VarInit, localVarIndex);
        
        if (context.HasLocalInHierarchy(varName))
        {
            var (varIndex, functionIndex) = context.GetLocalInHierarchy(varName);
            var instructionArg = LjsRuntimeUtils.CombineLocalIndexAndFunctionIndex(varIndex, functionIndex);
            
            return new LjsInstruction(LjsInstructionCode.ParentVarInit, instructionArg);
        }
        
        return new LjsInstruction(
                LjsInstructionCode.ExtStore, _constants.AddStringConstant(varName));
    }
    
    private void AddVarIncrementInstruction(FunctionContext context, LjsAstIncrementVar incrementVar)
    {
        var instructions = context.Instructions;

        var varLoadInstruction = CreateVarLoadInstruction(incrementVar.VarName, context);
                
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
                instructions.Add(CreateVarStoreInstruction(incrementVar.VarName, context));
                break;
                    
            case LjsAstIncrementOrder.Postfix:
                instructions.Add(CreateVarInitInstruction(incrementVar.VarName, context));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void AddVarStoreInstructions(FunctionContext context, LjsAstSetVar setVar)
    {
        var instructions = context.Instructions;
        
        var localVarIndex = context.GetLocal(setVar.VarName);
        var isLocal = localVarIndex != -1;
                
        if (!isLocal && _namedFunctionsMap.ContainsKey(setVar.VarName))
        {
            throw new LjsCompilerError($"named function assign {setVar.VarName}");
        }
                
        if (setVar.AssignMode == LjsAstAssignMode.Normal)
        {
            ProcessNode(setVar.Expression, context);
        }
        else
        {
            instructions.Add(CreateVarLoadInstruction(setVar.VarName, context));
                    
            ProcessNode(setVar.Expression, context);
                    
            instructions.Add(new LjsInstruction(
                LjsCompileUtils.GetComplexAssignmentOpCode(setVar.AssignMode)));
        }
        
        instructions.Add(CreateVarStoreInstruction(setVar.VarName, context));
    }
    
    
}