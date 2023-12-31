using static App16.LightJS.Runtime.LjsTypesCoercionUtil;
using static App16.LightJS.Runtime.LjsBasicOperationsHelper;

using App16.LightJS.Errors;
using App16.LightJS.Program;

namespace App16.LightJS.Runtime;

public sealed class LjsRuntime
{
    private readonly LjsProgram _program;
    private readonly LjsProgramConstants _constants;
    private readonly Stack<LjsObject> _stack = new();
    private readonly LjsPointersStack _thisPointersStack = new();

    private readonly List<FunctionContext> _functionCallStack = new();
    private readonly List<LjsObject> _functionThisPointers = new();

    private readonly List<LjsObject> _locals = new();

    private bool _isExecutionCalled = false;
    private bool _isRunning = false;

    private readonly LjsObject _globalObject = new();

    private readonly Dictionary<string, LjsObject> _externals;

    public LjsRuntime(LjsProgram program)
    {
        _program = program;
        _constants = program.Constants;
        _externals = LjsLanguageApi.CreateApiDictionary();
    }

    public void AddExternal(string name, LjsObject obj)
    {
        _externals[name] = obj;
    }

    internal bool HasLocal(string name) => IsValidLocalIndex(GetLocalIndex(name));

    private int GetLocalIndex(string name)
    {
        if (_functionCallStack.Count == 0) return -1;
        
        var functionContext = _functionCallStack[0];
        
        if (functionContext.FunctionIndex != 0) return -1;
        
        var mainFunctionData = _program.MainFunctionData;

        var locals = mainFunctionData.Locals;

        foreach (var p in locals)
        {
            if (p.VarKind == LjsLocalVarKind.Var && p.Name == name) return p.Index;
        }

        return -1;
    }
    
    private bool IsValidLocalIndex(int localIndex) => localIndex >= 0 && localIndex < _locals.Count;
    
    /// <summary>
    /// Returns value of the local var in main function with specified name;
    /// Returns LjsObject.Undefined if local var not found or not on the stack;
    /// Please use HasLocal to check if var with specified name exists in current context
    /// </summary>
    internal LjsObject GetLocal(string name)
    {
        var localIndex = GetLocalIndex(name);

        return IsValidLocalIndex(localIndex) ? _locals[localIndex] : LjsObject.Undefined;
    }

    /// <summary>
    /// Returns true if local var is successfully set, false otherwise
    /// </summary>
    internal bool SetLocal(string name, LjsObject value)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("name is null or empty", nameof(name));
        
        if (value == null)
            throw new ArgumentNullException(nameof(value));
            
        var localIndex = GetLocalIndex(name);
        
        if (!IsValidLocalIndex(localIndex)) return false;

        _locals[localIndex] = value;
        return true;
    }

    public bool CanInvoke(string functionName)
    {
        if (_isRunning) return false;

        if (!_program.ContainsFunction(functionName)) return false;
        
        if (_functionCallStack.Count == 0) return false;
        
        var functionContext = _functionCallStack[0];
        
        return functionContext.FunctionIndex == 0;
    }
    
    
    private readonly struct FunctionContext
    {
        public int FunctionIndex { get; }
        public int InstructionIndex { get; }
        
        public int LocalsCount { get; }
        public int LocalsOffset { get; }

        public FunctionContext(
            int functionIndex, 
            int instructionIndex, 
            int localsCount,
            int localsOffset)
        {
            FunctionIndex = functionIndex;
            InstructionIndex = instructionIndex;
            LocalsCount = localsCount;
            LocalsOffset = localsOffset;
        }

        public FunctionContext NextInstruction => 
            new(FunctionIndex, InstructionIndex + 1, LocalsCount, LocalsOffset);
        
        public FunctionContext JumpToInstruction(int instructionIndex) =>
            new(FunctionIndex, instructionIndex, LocalsCount, LocalsOffset);

        public int GetNextFunctionLocalsOffset() => LocalsOffset + LocalsCount;
    }
    
    private FunctionContext StartFunction(int index, int localsCount, LjsObject thisObject)
    {
        var localsOffset = _functionCallStack.Count != 0 ? 
            _functionCallStack[^1].GetNextFunctionLocalsOffset() : 0;


        var context = new FunctionContext(
            index,0, localsCount, localsOffset);
        
        _functionCallStack.Add(context);
        _functionThisPointers.Add(thisObject);

        for (var i = 0; i < localsCount; i++)
        {
            _locals.Add(LjsObject.Undefined);
        }

        return context;
    }

    private void StopFunction()
    {
        var fc = _functionCallStack[^1];
        _locals.RemoveRange(fc.LocalsOffset, fc.LocalsCount);
        _functionCallStack.RemoveAt(_functionCallStack.Count - 1);
        _functionThisPointers.RemoveAt(_functionThisPointers.Count - 1);
    }

    private FunctionContext GetParentFunctionContext(int functionIndex)
    {
        for (var i = _functionCallStack.Count - 1; i >= 0; --i)
        {
            var fc = _functionCallStack[i];
            if (fc.FunctionIndex == functionIndex) return fc;
        }

        throw new LjsInternalError($"function with index {functionIndex} not found in call stack");
    }

    private void ExtStore(string varName, LjsObject v)
    {
        throw new LjsRuntimeError($"ExtStore {varName} not supported");
    }
    
    private LjsObject ExtLoad(string varName)
    {
        if (_externals.ContainsKey(varName))
        {
            return _externals[varName];
        }

        throw new LjsRuntimeError($"external object not found: {varName}");
    }

    public LjsObject Execute()
    {
        if (_isExecutionCalled)
            throw new Exception("you can not call Execute more then once");

        _isExecutionCalled = true;

        // start main function at instruction 0
        var mainFunc = _program.GetFunction(0);

        StartFunction(0, mainFunc.LocalsCount, _globalObject);

        ExecuteInternal();
        
        var executionResult = 
            (_stack.Count > 0) ? _stack.Pop() : LjsObject.Undefined;

        return executionResult;
    }
    
    private void ExecuteInternal(int haltOnFunctionStackCount = 0)
    {
        if (_isRunning)
            throw new Exception("concurrent execution access");
        
        _isRunning = true;

        var varName = string.Empty;
        var varIndex = -1;
        var v = LjsObject.Undefined;

        var execute = true;

        while (execute)
        {
            var fCtx = _functionCallStack[^1];
            var ff = _program.GetFunction(fCtx.FunctionIndex);
            var functionThisObj = _functionThisPointers[^1];
            
            var jump = false;
            var instruction = ff.Instructions[fCtx.InstructionIndex];
            var instructionCode = instruction.Code;

            switch (instructionCode)
            {
                case LjsInstructionCode.Jump:
                    _functionCallStack[^1] = fCtx.JumpToInstruction(instruction.Argument);
                    jump = true;
                    break;
                
                case LjsInstructionCode.JumpIfFalse:
                    var jumpConditionObj = _stack.Pop();
                    var jumpCondition = ToBool(jumpConditionObj);
                    if (!jumpCondition)
                    {
                        _functionCallStack[^1] = fCtx.JumpToInstruction(instruction.Argument);
                        jump = true;
                    }
                    break;
                
                case LjsInstructionCode.FuncRef:
                    _stack.Push(new LjsFunctionPointer(instruction.Argument));
                    break;

                case LjsInstructionCode.FuncCall:
                    
                    var funcRef = _stack.Pop();
                    var funcRefStackIndex = _stack.Count;
                    
                    var argsCount = instruction.Argument;
                    
                    switch (funcRef)
                    {
                        case LjsFunctionPointer functionPointer:
                            // move instruction pointer
                            _functionCallStack[^1] = fCtx.NextInstruction;
                            
                            var f = _program.GetFunction(functionPointer.FunctionIndex);
                            
                            var fc = StartFunction(
                                functionPointer.FunctionIndex, f.LocalsCount, 
                                _thisPointersStack.TryGetPointer(funcRefStackIndex, out var funcThisPointer) ? 
                                    funcThisPointer : functionThisObj);

                            // remove arguments from stack that can not be used by specified function
                            while (argsCount > f.Arguments.Length)
                            {
                                _stack.Pop();
                                --argsCount;
                            }
                        
                            for (var j = f.Arguments.Length - 1; j >= 0; --j)
                            {
                                var arg = f.Arguments[j];
                                _locals[fc.LocalsOffset + j] = 
                                    j < argsCount ? _stack.Pop() : arg.DefaultValue;
                            }

                            jump = true;
                            break;
                        
                        case LjsFunction extFunc:
                            
                            // remove arguments from stack that can not be used by specified function
                            while (argsCount > extFunc.ArgumentsCount)
                            {
                                _stack.Pop();
                                --argsCount;
                            }

                            var args = LjsRuntimeUtils.GetTemporaryObjectsList();
                            var argsIndexOffset = 0;
                            
                            if (extFunc.MemberType == LjsMemberType.InstanceMember)
                            {
                                argsIndexOffset = 1;

                                if (!_thisPointersStack.TryGetPointer(funcRefStackIndex, out var thisPointer))
                                {
                                    throw new LjsInternalError("filed to get this pointer from stack");
                                }
                                
                                args.Add(thisPointer);
                            }

                            while (args.Count < extFunc.ArgumentsCount)
                            {
                                args.Add(LjsObject.Undefined);
                            }
                            
                            for (var j = extFunc.ArgumentsCount - 1; j >= argsIndexOffset; --j)
                            {
                                args[j] = j < argsCount + argsIndexOffset ? _stack.Pop() : LjsObject.Undefined;
                            }
                            
                            var extFuncResult = extFunc.Invoke(args);
                            
                            _stack.Push(extFuncResult);
                            
                            LjsRuntimeUtils.ReleaseTemporaryObjectsList(args);
                            
                            break;
                        
                        default:
                            throw new LjsRuntimeError("not a function");
                    }
                    
                    break;
                
                case LjsInstructionCode.Return:
                    // we treat return in main function as hault
                    if (fCtx.FunctionIndex != 0)
                    {
                        StopFunction();
                        if (haltOnFunctionStackCount == _functionCallStack.Count)
                        {
                            execute = false;
                        }
                        else
                        {
                            jump = true;
                        }
                    }
                    else
                    {
                        execute = false;
                    }
                    break;
                    
                
                case LjsInstructionCode.Halt:
                    execute = false;
                    break;
                
                case LjsInstructionCode.ConstInt:
                    _stack.Push(new LjsInteger(instruction.Argument));
                    break;
                case LjsInstructionCode.ConstIntZero:
                    _stack.Push(LjsInteger.Zero);
                    break;
                case LjsInstructionCode.ConstIntOne:
                    _stack.Push(LjsInteger.One);
                    break;
                case LjsInstructionCode.ConstIntMinusOne:
                    _stack.Push(LjsInteger.MinusOne);
                    break;
                case LjsInstructionCode.ConstDouble:
                    _stack.Push(new LjsDouble(_constants.GetDoubleConstant(instruction.Argument)));
                    break;
                case LjsInstructionCode.ConstDoubleZero:
                    _stack.Push(LjsDouble.Zero);
                    break;
                case LjsInstructionCode.ConstDoubleNaN:
                    _stack.Push(LjsDouble.NaN);
                    break;
                case LjsInstructionCode.ConstString:
                    _stack.Push(new LjsString(_constants.GetStringConstant(instruction.Argument)));
                    break;
                case LjsInstructionCode.ConstStringEmpty:
                    _stack.Push(LjsString.Empty);
                    break;
                case LjsInstructionCode.ConstTrue:
                    _stack.Push(LjsBoolean.True);
                    break;
                case LjsInstructionCode.ConstFalse:
                    _stack.Push(LjsBoolean.False);
                    break;
                
                case LjsInstructionCode.ConstNull:
                    _stack.Push(LjsObject.Null);
                    break;
                
                case LjsInstructionCode.ConstUndef:
                    _stack.Push(LjsObject.Undefined);
                    break;
                
                case LjsInstructionCode.Copy:
                    _stack.Push(_stack.Peek());
                    break;
                
                case LjsInstructionCode.Discard:
                    _stack.Pop();
                    break;

                // simple arithmetic
                case LjsInstructionCode.Add:
                case LjsInstructionCode.Sub:
                case LjsInstructionCode.Mul:
                case LjsInstructionCode.Div:
                case LjsInstructionCode.Mod:
                case LjsInstructionCode.Pow:
                    var right = _stack.Pop();
                    var left = _stack.Pop();
                    _stack.Push(ExecuteArithmeticOperation(left, right, instructionCode));
                    break;
                // bitwise ops
                case LjsInstructionCode.BitAnd:
                case LjsInstructionCode.BitOr:
                case LjsInstructionCode.BitShiftLeft:
                case LjsInstructionCode.BitSShiftRight:
                case LjsInstructionCode.BitUShiftRight:
                    var bitsOperandRight = _stack.Pop();
                    var bitsOperandLeft = _stack.Pop();
                    _stack.Push(
                        ExecuteBitwiseOperation(bitsOperandLeft, bitsOperandRight, instructionCode));
                    break;
                // compare ops
                case LjsInstructionCode.Gt:
                case LjsInstructionCode.Gte:
                case LjsInstructionCode.Lt:
                case LjsInstructionCode.Lte:
                case LjsInstructionCode.Eq:
                case LjsInstructionCode.Eqs:
                case LjsInstructionCode.Neq:
                case LjsInstructionCode.Neqs:
                    var compareRight = _stack.Pop();
                    var compareLeft = _stack.Pop();
                    _stack.Push(
                        ExecuteComparisonOperation(compareLeft, compareRight, instructionCode));
                    break;
                // compare ops
                case LjsInstructionCode.And:
                case LjsInstructionCode.Or:
                    var flagRight = _stack.Pop();
                    var flagLeft = _stack.Pop();
                    _stack.Push(
                        ExecuteLogicalOperation(flagLeft, flagRight, instructionCode));
                    break;
                // unary ops
                case LjsInstructionCode.Minus:
                case LjsInstructionCode.BitNot:
                case LjsInstructionCode.Not:
                    var unaryOperand = _stack.Pop();
                    _stack.Push(ExecuteUnaryOperation(unaryOperand, instructionCode));
                    break;
                
                // vars 
                
                case LjsInstructionCode.ExtStore:
                    varName = _constants.GetStringConstant(instruction.Argument);
                    
                    v = _stack.Peek();
                    
                    ExtStore(varName, v);
                    break;
                
                case LjsInstructionCode.VarStore:
                    varIndex = instruction.Argument;
                    
                    v = _stack.Peek();
                    
                    _locals[fCtx.LocalsOffset + varIndex] = v;
                    break;
                
                case LjsInstructionCode.ExtLoad:
                    
                    varName = _constants.GetStringConstant(instruction.Argument);

                    v = ExtLoad(varName);
                    
                    _stack.Push(v);
                    
                    break;
                
                case LjsInstructionCode.VarLoad:
                    
                    varIndex = instruction.Argument;

                    v = _locals[fCtx.LocalsOffset + varIndex];
                    
                    _stack.Push(v);
                    
                    break;
                
                case LjsInstructionCode.ParentVarLoad:

                    varIndex = LjsRuntimeUtils.GetLocalIndex(instruction.Argument);
                    
                    var funcIndex3 = LjsRuntimeUtils.GetFunctionIndex(instruction.Argument);
                    var parentFc3 = GetParentFunctionContext(funcIndex3);
                    
                    v = _locals[parentFc3.LocalsOffset + varIndex];
                    
                    _stack.Push(v);
                    break;
                case LjsInstructionCode.ParentVarStore:
                    v = _stack.Peek();
                    
                    varIndex = LjsRuntimeUtils.GetLocalIndex(instruction.Argument);
                    
                    var funcIndex2 = LjsRuntimeUtils.GetFunctionIndex(instruction.Argument);
                    var parentFc2 = GetParentFunctionContext(funcIndex2);
                    
                    _locals[parentFc2.LocalsOffset + varIndex] = v;
                    break;
                
                case LjsInstructionCode.SetProp:
                    ExecuteSetPropertyInstruction();
                    break;
                
                case LjsInstructionCode.GetProp:
                    ExecuteGetPropertyInstruction();
                    break;
                
                case LjsInstructionCode.GetThis:
                    _stack.Push(functionThisObj);
                    break;
                
                case LjsInstructionCode.NewDictionary:

                    var propsCount = instruction.Argument;
                    var newDict = new LjsDictionary();

                    for (var i = 0; i < propsCount; i++)
                    {
                        var name = _stack.Pop();
                        var val = _stack.Pop();
                        newDict.Set(name.ToString(), val);
                    }
                    
                    _stack.Push(newDict);
                    
                    break;
                
                case LjsInstructionCode.NewArray:
                    
                    var elementsCount = instruction.Argument;
                    
                    var newArr = new LjsArray(elementsCount);

                    for (var i = elementsCount - 1; i >= 0; --i)
                    {
                        newArr[i] = _stack.Pop();
                    }
                    _stack.Push(newArr);
                    break;
                    
                default:
                    throw new LjsInternalError($"unsupported op code {instructionCode}");
                    
            }
            
            _thisPointersStack.Clear(_stack.Count);

            if (execute && !jump)
            {
                _functionCallStack[^1] = fCtx.NextInstruction;
            }
        }

        _isRunning = false;
    }

    private void ExecuteSetPropertyInstruction()
    {
        var propId = _stack.Pop();
        var propSource = _stack.Pop();
        var propValue = _stack.Pop();
        
        var typeInfo = propSource.GetTypeInfo();

        var propNameStr = propId is LjsString ? propId.ToString() : string.Empty;
        
        if (!string.IsNullOrEmpty(propNameStr) && typeInfo.HasMember(propNameStr))
        {
            var member = typeInfo.GetMember(propId.ToString());

            if (member is LjsProperty prop && (prop.AccessType & LjsPropertyAccessType.Write) != 0)
            {
                prop.Set(propSource, propValue);
            }
            else
            {
                throw new LjsRuntimeError($"property {propNameStr} not write enabled");
            }
        }
        else if (propSource is LjsDictionary d)
        {
            d.Set(propId, propValue);
        }
        
        else if (propSource is ILjsArray a && propId is LjsNumber n)
        {
            a.Set(n.IntegerValue, propValue);
        }

        else
        {
            throw new LjsRuntimeError($"{propSource} has no property with name {propId}");
        }
        
        _stack.Push(propValue);
    }

    private void ExecuteGetPropertyInstruction()
    {
        var propId = _stack.Pop();
        var propSource = _stack.Pop();

        var typeInfo = propSource.GetTypeInfo();

        var propNameStr = propId is LjsString ? propId.ToString() : string.Empty;

        if (!string.IsNullOrEmpty(propNameStr) && typeInfo.HasMember(propNameStr))
        {
            var member = typeInfo.GetMember(propId.ToString());

            switch (member)
            {
                case LjsFunction memberFunc:
                    
                    if (memberFunc.MemberType == LjsMemberType.InstanceMember)
                    {
                        _thisPointersStack.PushPointer(_stack.Count, propSource);
                    }

                    _stack.Push(member);
                    break;
                case LjsProperty prop:
                    if ((prop.AccessType & LjsPropertyAccessType.Read) != 0)
                    {
                        _stack.Push(prop.Get(propSource));
                    }
                    else
                    {
                        throw new LjsRuntimeError($"property {propNameStr} not read enabled");
                    }

                    break;
            }
        }
        else if (propSource is ILjsDictionary d)
        {
            var propValue = d.Get(propId);
            if (propValue is LjsFunctionPointer)
            {
                _thisPointersStack.PushPointer(_stack.Count, propSource);
            }

            _stack.Push(propValue);
        }
        
        else if (propSource is ILjsArray a && propId is LjsNumber n)
        {
            var propValue = a.Get(n.IntegerValue);

            _stack.Push(propValue);
        }

        else
        {
            throw new LjsRuntimeError($"{propSource} has no property with name {propId}");
        }
    }

    #region Invoke Script Function

    private void ThrowFunctionInvocationError(string functionName)
    {
        if (_isRunning)
            throw new Exception("can not call function while script is running");

        if (!_program.ContainsFunction(functionName)) 
            throw new Exception($"function {functionName} not found");

        if (_functionCallStack.Count == 0)
            throw new Exception("invalid execution state. " +
                                "You should call LjsRuntime.Execute() method before to initialize runtime. " +
                                "Remember not to call LjsRuntime.Execute() method more then once");
        
        var functionContext = _functionCallStack[0];

        if (functionContext.FunctionIndex != 0)
            throw new Exception("invalid internal execution state. runtime is broken");
    }

    private LjsObject ExecuteFunctionAndGetResult()
    {
        ExecuteInternal(1);
        return _stack.Pop();
    }
    
    public LjsObject Invoke(string functionName)
    {
        if (!CanInvoke(functionName)) 
            ThrowFunctionInvocationError(functionName);

        var functionData = _program.GetFunction(functionName);

        StartFunction(functionData.FunctionIndex, functionData.LocalsCount, _globalObject);

        return ExecuteFunctionAndGetResult();
    }
    
    public LjsObject Invoke(string functionName, LjsObject arg0)
    {
        if (!CanInvoke(functionName)) 
            ThrowFunctionInvocationError(functionName);

        var functionData = _program.GetFunction(functionName);

        var ctx = StartFunction(
            functionData.FunctionIndex, functionData.LocalsCount, _globalObject);

        var argsLn = functionData.Arguments.Length;
        
        if (argsLn >= 1) _locals[ctx.LocalsOffset + 0] = arg0;
        
        return ExecuteFunctionAndGetResult();
    }
    
    public LjsObject Invoke(string functionName, LjsObject arg0, LjsObject arg1)
    {
        if (!CanInvoke(functionName)) 
            ThrowFunctionInvocationError(functionName);

        var functionData = _program.GetFunction(functionName);

        var ctx = StartFunction(
            functionData.FunctionIndex, functionData.LocalsCount, _globalObject);

        var argsLn = functionData.Arguments.Length;
        
        if (argsLn >= 1) _locals[ctx.LocalsOffset + 0] = arg0;
        if (argsLn >= 2) _locals[ctx.LocalsOffset + 1] = arg1;
        
        return ExecuteFunctionAndGetResult();
    }
    
    public LjsObject Invoke(string functionName, LjsObject arg0, LjsObject arg1, LjsObject arg2)
    {
        if (!CanInvoke(functionName)) 
            ThrowFunctionInvocationError(functionName);

        var functionData = _program.GetFunction(functionName);

        var ctx = StartFunction(
            functionData.FunctionIndex, functionData.LocalsCount, _globalObject);

        var argsLn = functionData.Arguments.Length;
        
        if (argsLn >= 1) _locals[ctx.LocalsOffset + 0] = arg0;
        if (argsLn >= 2) _locals[ctx.LocalsOffset + 1] = arg1;
        if (argsLn >= 3) _locals[ctx.LocalsOffset + 2] = arg2;
        
        return ExecuteFunctionAndGetResult();
    }
    
    public LjsObject Invoke(string functionName, 
        LjsObject arg0, LjsObject arg1, LjsObject arg2, LjsObject arg3)
    {
        if (!CanInvoke(functionName)) 
            ThrowFunctionInvocationError(functionName);

        var functionData = _program.GetFunction(functionName);

        var ctx = StartFunction(
            functionData.FunctionIndex, functionData.LocalsCount, _globalObject);

        var argsLn = functionData.Arguments.Length;
        
        if (argsLn >= 1) _locals[ctx.LocalsOffset + 0] = arg0;
        if (argsLn >= 2) _locals[ctx.LocalsOffset + 1] = arg1;
        if (argsLn >= 3) _locals[ctx.LocalsOffset + 2] = arg2;
        if (argsLn >= 4) _locals[ctx.LocalsOffset + 3] = arg3;
        
        return ExecuteFunctionAndGetResult();
    }
    
    public LjsObject Invoke(string functionName, params LjsObject[] args)
    {
        if (!CanInvoke(functionName)) 
            ThrowFunctionInvocationError(functionName);

        var functionData = _program.GetFunction(functionName);

        var ctx = StartFunction(
            functionData.FunctionIndex, functionData.LocalsCount, _globalObject);

        var argsLn = functionData.Arguments.Length;
        
        for (var i = 0; i < argsLn && i < args.Length; i++)
        {
            _locals[ctx.LocalsOffset + i] = args[i];
        }
        
        return ExecuteFunctionAndGetResult();
    }

    #endregion
    
    
}