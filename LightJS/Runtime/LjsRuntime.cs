using LightJS.Errors;
using LightJS.Program;

namespace LightJS.Runtime;

public sealed class LjsRuntime
{
    private readonly LjsProgram _program;
    private readonly LjsProgramConstants _constants;
    private readonly Stack<LjsObject> _stack = new();
    
    private readonly List<FunctionContext> _functionCallStack = new();
    
    private readonly VarSpace _varSpace;

    public LjsRuntime(LjsProgram program)
    {
        _program = program;
        _constants = program.Constants;
        _varSpace = new VarSpace();
    }
    
    private sealed class VarSpace
    {
        private readonly Dictionary<string, LjsObject> _vars = new();
        
        private readonly VarSpace? _parent;
        
        public VarSpace() {}

        private VarSpace(VarSpace parentSpace)
        {
            _parent = parentSpace;
        }
        
        public LjsObject Get(string varName)
        {
            if (_vars.ContainsKey(varName))
            {
                return _vars[varName];
            }

            if (_parent != null) return _parent.Get(varName);
        
            throw new LjsRuntimeError($"{varName} is not defined");
        
        }

        public void Declare(string varName)
        {
            if (_vars.ContainsKey(varName))
            {
                throw new LjsRuntimeError($"variable already declared {varName}");
            }
            _vars[varName] = LjsObject.Undefined;
        }

        public void Store(string varName, LjsObject value)
        {
            _vars[varName] = value;
        }

        public VarSpace CreateChild()
        {
            return new VarSpace(this);
        }

        public VarSpace GetParent()
        {
            return _parent ?? this;
        }
    }
    
    private readonly struct FunctionContext
    {
        public int FunctionIndex { get; }
        public int InstructionIndex { get; }

        public FunctionContext(int functionIndex, int instructionIndex)
        {
            FunctionIndex = functionIndex;
            InstructionIndex = instructionIndex;
        }
    }
    
    public LjsObject Execute()
    {
        // start main function at instruction 0
        _functionCallStack.Add(new FunctionContext(0,0));

        var varName = string.Empty;
        var v = LjsObject.Undefined;

        var execute = true;
        var varSpace = _varSpace;

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
                    _functionCallStack[^1] = 
                        new FunctionContext(fCtx.FunctionIndex, instruction.Argument);
                    jump = true;
                    break;
                
                case LjsInstructionCode.JumpIfFalse:
                    var jumpConditionObj = _stack.Pop();
                    var jumpCondition = LjsRuntimeUtils.ToBool(jumpConditionObj);
                    if (!jumpCondition)
                    {
                        _functionCallStack[^1] = 
                            new FunctionContext(fCtx.FunctionIndex, instruction.Argument);
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
                        var argsCount = instruction.Argument;
                        var f = _program.GetFunction(functionPointer.FunctionIndex);
                    
                        varSpace = varSpace.CreateChild();

                        for (var j = f.Args.Count - 1; j >= 0; --j)
                        {
                            var arg = f.Args[j];
                        
                            varSpace.Store(arg.Name, j < argsCount ? _stack.Pop() : arg.DefaultValue);
                        }

                        _functionCallStack[^1] = new FunctionContext(fCtx.FunctionIndex, fCtx.InstructionIndex + 1);
                        _functionCallStack.Add(new FunctionContext(functionPointer.FunctionIndex, 0));

                        jump = true;
                        
                    }
                    else
                    {
                        throw new LjsRuntimeError("not a function");
                    }
                    
                    break;
                
                case LjsInstructionCode.Return:
                    varSpace = varSpace.GetParent();
                    _functionCallStack.RemoveAt(_functionCallStack.Count - 1);
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
                case LjsInstructionCode.VarDef:
                    varName = _constants.GetStringConstant(instruction.Argument);
                    
                    varSpace.Declare(varName);
                    
                    break;
                
                case LjsInstructionCode.VarInit:
                    
                    varName = _constants.GetStringConstant(instruction.Argument);
                    v = _stack.Pop();
                    
                    varSpace.Store(varName, v);
                    break;
                
                case LjsInstructionCode.VarStore:
                    varName = _constants.GetStringConstant(instruction.Argument);
                    
                    v = _stack.Peek();
                    
                    varSpace.Store(varName, v);
                    break;
                
                case LjsInstructionCode.VarLoad:
                    
                    varName = _constants.GetStringConstant(instruction.Argument);

                    v = varSpace.Get(varName);
                    
                    _stack.Push(v);
                    
                    break;
                    
                default:
                    throw new LjsInternalError($"unsupported op code {instructionCode}");
                    
            }

            if (!jump)
            {
                _functionCallStack[^1] = 
                    new FunctionContext(fCtx.FunctionIndex, fCtx.InstructionIndex + 1);
            }
        }

        return (_stack.Count > 0) ? _stack.Pop() : LjsObject.Undefined;
    }
}