using LightJS.Errors;
using LightJS.Program;

namespace LightJS.Runtime;

public sealed class LjsRuntime
{
    private readonly LjsProgram _program;
    private readonly LjsProgramConstants _constants;
    private readonly Stack<LjsObject> _stack = new();
    
    private readonly List<FunctionContext> _functionCallStack = new();

    private readonly List<LjsObject> _locals = new();

    public LjsRuntime(LjsProgram program)
    {
        _program = program;
        _constants = program.Constants;
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
    
    private FunctionContext StartFunction(int index, int localsCount)
    {
        var localsOffset = _functionCallStack.Count != 0 ? 
            _functionCallStack[^1].GetNextFunctionLocalsOffset() : 0;


        var context = new FunctionContext(
            index,0, localsCount, localsOffset);
        
        _functionCallStack.Add(context);
        
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
        throw new NotImplementedException($"ExtStore {varName}");
    }
    
    private LjsObject ExtLoad(string varName)
    {
        throw new NotImplementedException($"ExtLoad {varName}");
    }
    
    public LjsObject Execute()
    {
        // start main function at instruction 0
        var mainFunc = _program.GetFunction(0);

        StartFunction(0, mainFunc.LocalsCount);

        var varName = string.Empty;
        var varIndex = -1;
        var v = LjsObject.Undefined;

        var execute = true;

        while (execute)
        {
            var fCtx = _functionCallStack[^1];
            var ff = _program.GetFunction(fCtx.FunctionIndex);
            
            var jump = false;
            var instruction = ff.InstructionsList.Instructions[fCtx.InstructionIndex];
            var instructionCode = instruction.Code;

            switch (instructionCode)
            {
                case LjsInstructionCode.Jump:
                    _functionCallStack[^1] = fCtx.JumpToInstruction(instruction.Argument);
                    jump = true;
                    break;
                
                case LjsInstructionCode.JumpIfFalse:
                    var jumpConditionObj = _stack.Pop();
                    var jumpCondition = LjsRuntimeUtils.ToBool(jumpConditionObj);
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

                    if (funcRef is LjsFunctionPointer functionPointer)
                    {
                        // move instruction pointer
                        _functionCallStack[^1] = fCtx.NextInstruction;
                        
                        var argsCount = instruction.Argument;
                        var f = _program.GetFunction(functionPointer.FunctionIndex);

                        var fc = StartFunction(functionPointer.FunctionIndex, f.LocalsCount);

                        for (var j = f.Args.Count - 1; j >= 0; --j)
                        {
                            var arg = f.Args[j];
                            _locals[fc.LocalsOffset + j] = 
                                j < argsCount ? _stack.Pop() : arg.DefaultValue;
                        }

                        jump = true;
                        
                    }
                    else
                    {
                        throw new LjsRuntimeError("not a function");
                    }
                    
                    break;
                
                case LjsInstructionCode.Return:
                    StopFunction();
                    jump = true;
                    break;
                    
                
                case LjsInstructionCode.Halt:
                    execute = false;
                    break;
                
                case LjsInstructionCode.ConstInt:
                    _stack.Push(new LjsValue<int>(instruction.Argument));
                    break;
                case LjsInstructionCode.ConstDouble:
                    _stack.Push(new LjsValue<double>(_constants.GetDoubleConstant(instruction.Argument)));
                    break;
                case LjsInstructionCode.ConstString:
                    _stack.Push(new LjsValue<string>(_constants.GetStringConstant(instruction.Argument)));
                    break;
                case LjsInstructionCode.ConstTrue:
                    _stack.Push(LjsValue.True);
                    break;
                case LjsInstructionCode.ConstFalse:
                    _stack.Push(LjsValue.False);
                    break;
                
                case LjsInstructionCode.ConstNull:
                    _stack.Push(LjsObject.Null);
                    break;
                
                case LjsInstructionCode.ConstUndef:
                    _stack.Push(LjsObject.Undefined);
                    break;
                // simple arithmetic
                case LjsInstructionCode.Add:
                case LjsInstructionCode.Sub:
                case LjsInstructionCode.Mul:
                case LjsInstructionCode.Div:
                case LjsInstructionCode.Mod:
                    var right = _stack.Pop();
                    var left = _stack.Pop();
                    _stack.Push(LjsRuntimeUtils.ExecuteArithmeticOperation(left, right, instructionCode));
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
                        LjsRuntimeUtils.ExecuteBitwiseOperation(bitsOperandLeft, bitsOperandRight, instructionCode));
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
                        LjsRuntimeUtils.ExecuteComparisonOperation(compareLeft, compareRight, instructionCode));
                    break;
                // compare ops
                case LjsInstructionCode.And:
                case LjsInstructionCode.Or:
                    var flagRight = _stack.Pop();
                    var flagLeft = _stack.Pop();
                    _stack.Push(
                        LjsRuntimeUtils.ExecuteLogicalOperation(flagLeft, flagRight, instructionCode));
                    break;
                // unary ops
                case LjsInstructionCode.Minus:
                case LjsInstructionCode.BitNot:
                case LjsInstructionCode.Not:
                    var unaryOperand = _stack.Pop();
                    _stack.Push(LjsRuntimeUtils.ExecuteUnaryOperation(unaryOperand, instructionCode));
                    break;
                
                // vars 
                
                case LjsInstructionCode.VarInit:
                    
                    v = _stack.Pop();
                    varIndex = instruction.Argument;
                    
                    _locals[fCtx.LocalsOffset + varIndex] = v;
                    break;
                
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
                
                case LjsInstructionCode.ParentVarInit:
                    v = _stack.Pop();
                    
                    varIndex = LjsRuntimeUtils.GetLocalIndex(instruction.Argument);
                    
                    var funcIndex = LjsRuntimeUtils.GetFunctionIndex(instruction.Argument);
                    var parentFc = GetParentFunctionContext(funcIndex);
                    
                    _locals[parentFc.LocalsOffset + varIndex] = v;

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
                    
                default:
                    throw new LjsInternalError($"unsupported op code {instructionCode}");
                    
            }

            if (!jump)
            {
                _functionCallStack[^1] = fCtx.NextInstruction;
            }
        }

        return (_stack.Count > 0) ? _stack.Pop() : LjsObject.Undefined;
    }
}