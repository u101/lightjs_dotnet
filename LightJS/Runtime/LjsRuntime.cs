using LightJS.Errors;
using LightJS.Program;

namespace LightJS.Runtime;

public sealed class LjsRuntime
{
    private readonly LjsProgram _program;
    private readonly Stack<LjsObject> _stack = new();
    private readonly Stack<LjsInstructionsList> _instructionsStack = new();
    private readonly Stack<int> _instructionsIndexStack = new();
    private readonly Dictionary<string, LjsObject> _vars = new();

    public LjsRuntime(LjsProgram program)
    {
        _program = program;
    }
    
    public LjsObject Execute()
    {
        var prg = _program;

        foreach (var (funcName, func) in prg.Functions)
        {
            _vars[funcName] = func;
        }
        
        var instructions = 
            prg.InstructionsList;
        

        var varName = string.Empty;
        var v = LjsObject.Undefined;

        var i = 0;
        var execute = true;

        while (execute && i < instructions.InstructionsCount)
        {
            var jump = false;
            var instruction = instructions.Instructions[i];
            var instructionCode = instruction.Code;

            switch (instructionCode)
            {
                case LjsInstructionCodes.Jump:
                    i = instruction.Index;
                    jump = true;
                    break;
                
                case LjsInstructionCodes.JumpIfFalse:
                    var jumpConditionObj = _stack.Pop();
                    var jumpCondition = LjsRuntimeUtils.ToBool(jumpConditionObj);
                    if (!jumpCondition)
                    {
                        i = instruction.Index;
                        jump = true;
                    }
                    break;
                
                case LjsInstructionCodes.FuncCall:

                    var funcRef = _stack.Pop();

                    if (funcRef is not LjsFunction f)
                    {
                        throw new LjsRuntimeError("not a function");
                    }

                    _instructionsIndexStack.Push(i);
                    _instructionsStack.Push(instructions);
                    throw new NotImplementedException();
                    break;
                    
                
                case LjsInstructionCodes.Halt:
                    execute = false;
                    break;
                
                case LjsInstructionCodes.ConstInt:
                    _stack.Push(new LjsValue<int>(prg.GetIntegerConstant(instruction.Index)));
                    break;
                case LjsInstructionCodes.ConstDouble:
                    _stack.Push(new LjsValue<double>(prg.GetDoubleConstant(instruction.Index)));
                    break;
                case LjsInstructionCodes.ConstString:
                    _stack.Push(new LjsValue<string>(prg.GetStringConstant(instruction.Index)));
                    break;
                case LjsInstructionCodes.ConstTrue:
                    _stack.Push(LjsValue.True);
                    break;
                case LjsInstructionCodes.ConstFalse:
                    _stack.Push(LjsValue.False);
                    break;
                
                case LjsInstructionCodes.ConstNull:
                    _stack.Push(LjsObject.Null);
                    break;
                
                case LjsInstructionCodes.ConstUndef:
                    _stack.Push(LjsObject.Undefined);
                    break;
                // simple arithmetic
                case LjsInstructionCodes.Add:
                case LjsInstructionCodes.Sub:
                case LjsInstructionCodes.Mul:
                case LjsInstructionCodes.Div:
                case LjsInstructionCodes.Mod:
                    var right = _stack.Pop();
                    var left = _stack.Pop();
                    _stack.Push(LjsRuntimeUtils.ExecuteArithmeticOperation(left, right, instructionCode));
                    break;
                // bitwise ops
                case LjsInstructionCodes.BitAnd:
                case LjsInstructionCodes.BitOr:
                case LjsInstructionCodes.BitShiftLeft:
                case LjsInstructionCodes.BitSShiftRight:
                case LjsInstructionCodes.BitUShiftRight:
                    var bitsOperandRight = _stack.Pop();
                    var bitsOperandLeft = _stack.Pop();
                    _stack.Push(
                        LjsRuntimeUtils.ExecuteBitwiseOperation(bitsOperandLeft, bitsOperandRight, instructionCode));
                    break;
                // compare ops
                case LjsInstructionCodes.Gt:
                case LjsInstructionCodes.Gte:
                case LjsInstructionCodes.Lt:
                case LjsInstructionCodes.Lte:
                case LjsInstructionCodes.Eq:
                case LjsInstructionCodes.Eqs:
                case LjsInstructionCodes.Neq:
                case LjsInstructionCodes.Neqs:
                    var compareRight = _stack.Pop();
                    var compareLeft = _stack.Pop();
                    _stack.Push(
                        LjsRuntimeUtils.ExecuteComparisonOperation(compareLeft, compareRight, instructionCode));
                    break;
                // compare ops
                case LjsInstructionCodes.And:
                case LjsInstructionCodes.Or:
                    var flagRight = _stack.Pop();
                    var flagLeft = _stack.Pop();
                    _stack.Push(
                        LjsRuntimeUtils.ExecuteLogicalOperation(flagLeft, flagRight, instructionCode));
                    break;
                // unary ops
                case LjsInstructionCodes.Minus:
                case LjsInstructionCodes.BitNot:
                case LjsInstructionCodes.Not:
                    var unaryOperand = _stack.Pop();
                    _stack.Push(LjsRuntimeUtils.ExecuteUnaryOperation(unaryOperand, instructionCode));
                    break;
                
                // vars 
                case LjsInstructionCodes.VarDef:
                    varName = prg.GetStringConstant(instruction.Index);
                    if (_vars.ContainsKey(varName))
                    {
                        throw new LjsRuntimeError($"variable already declared {varName}");
                    }
                    _vars[varName] = LjsObject.Undefined;
                    break;
                
                case LjsInstructionCodes.VarInit:
                    
                    varName = prg.GetStringConstant(instruction.Index);
                    v = _stack.Pop();
                    
                    _vars[varName] = v;
                    break;
                
                case LjsInstructionCodes.VarStore:
                    varName = prg.GetStringConstant(instruction.Index);
                    
                    v = _stack.Peek();
                    
                    _vars[varName] = v;
                    break;
                
                case LjsInstructionCodes.VarLoad:
                    
                    varName = prg.GetStringConstant(instruction.Index);
                    
                    if (!_vars.ContainsKey(varName))
                    {
                        throw new LjsRuntimeError($"variable not declared {varName}");
                    }
                    
                    _stack.Push(_vars[varName]);
                    
                    break;
                    
                default:
                    throw new LjsInternalError($"unsupported op code {instructionCode}");
                    
            }
            
            if (!jump) ++i;
        }

        return (_stack.Count > 0) ? _stack.Pop() : LjsObject.Undefined;
    }
}