using LightJS.Errors;
using LightJS.Program;

namespace LightJS.Runtime;

public sealed class LjsRuntime
{
    private readonly LjsProgram _program;
    private readonly Stack<LjsObject> _executionStack = new();
    private readonly Dictionary<string, LjsObject> _vars = new();

    public LjsRuntime(LjsProgram program)
    {
        _program = program;
    }
    
    public LjsObject Execute()
    {
        var prg = _program;
        var instructions = prg.Instructions;
        var ln = instructions.Count;

        var varName = string.Empty;
        var v = LjsObject.Undefined;

        var i = 0;
        var execute = true;

        while (execute && i < ln)
        {
            var jump = false;
            var instruction = instructions[i];
            var instructionCode = instruction.Code;

            switch (instructionCode)
            {
                case LjsInstructionCodes.Jump:
                    i = instruction.Index;
                    jump = true;
                    break;
                
                case LjsInstructionCodes.JumpIfFalse:
                    var jumpConditionObj = _executionStack.Pop();
                    var jumpCondition = LjsRuntimeUtils.ToBool(jumpConditionObj);
                    if (!jumpCondition)
                    {
                        i = instruction.Index;
                        jump = true;
                    }
                    break;
                
                case LjsInstructionCodes.Halt:
                    execute = false;
                    break;
                
                case LjsInstructionCodes.ConstInt:
                    _executionStack.Push(new LjsValue<int>(prg.GetIntegerConstant(instruction.Index)));
                    break;
                case LjsInstructionCodes.ConstDouble:
                    _executionStack.Push(new LjsValue<double>(prg.GetDoubleConstant(instruction.Index)));
                    break;
                case LjsInstructionCodes.ConstString:
                    _executionStack.Push(new LjsValue<string>(prg.GetStringConstant(instruction.Index)));
                    break;
                case LjsInstructionCodes.ConstTrue:
                    _executionStack.Push(LjsValue.True);
                    break;
                case LjsInstructionCodes.ConstFalse:
                    _executionStack.Push(LjsValue.False);
                    break;
                
                case LjsInstructionCodes.ConstNull:
                    _executionStack.Push(LjsObject.Null);
                    break;
                
                case LjsInstructionCodes.ConstUndef:
                    _executionStack.Push(LjsObject.Undefined);
                    break;
                // simple arithmetic
                case LjsInstructionCodes.Add:
                case LjsInstructionCodes.Sub:
                case LjsInstructionCodes.Mul:
                case LjsInstructionCodes.Div:
                case LjsInstructionCodes.Mod:
                    var right = _executionStack.Pop();
                    var left = _executionStack.Pop();
                    _executionStack.Push(LjsRuntimeUtils.ExecuteArithmeticOperation(left, right, instructionCode));
                    break;
                // bitwise ops
                case LjsInstructionCodes.BitAnd:
                case LjsInstructionCodes.BitOr:
                case LjsInstructionCodes.BitShiftLeft:
                case LjsInstructionCodes.BitSShiftRight:
                case LjsInstructionCodes.BitUShiftRight:
                    var bitsOperandRight = _executionStack.Pop();
                    var bitsOperandLeft = _executionStack.Pop();
                    _executionStack.Push(
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
                    var compareRight = _executionStack.Pop();
                    var compareLeft = _executionStack.Pop();
                    _executionStack.Push(
                        LjsRuntimeUtils.ExecuteComparisonOperation(compareLeft, compareRight, instructionCode));
                    break;
                // compare ops
                case LjsInstructionCodes.And:
                case LjsInstructionCodes.Or:
                    var flagRight = _executionStack.Pop();
                    var flagLeft = _executionStack.Pop();
                    _executionStack.Push(
                        LjsRuntimeUtils.ExecuteLogicalOperation(flagLeft, flagRight, instructionCode));
                    break;
                // unary ops
                case LjsInstructionCodes.Minus:
                case LjsInstructionCodes.BitNot:
                case LjsInstructionCodes.Not:
                    var unaryOperand = _executionStack.Pop();
                    _executionStack.Push(LjsRuntimeUtils.ExecuteUnaryOperation(unaryOperand, instructionCode));
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
                    v = _executionStack.Pop();
                    
                    _vars[varName] = v;
                    break;
                
                case LjsInstructionCodes.VarStore:
                    varName = prg.GetStringConstant(instruction.Index);
                    
                    v = _executionStack.Peek();
                    
                    _vars[varName] = v;
                    break;
                
                case LjsInstructionCodes.VarLoad:
                    
                    varName = prg.GetStringConstant(instruction.Index);
                    
                    if (!_vars.ContainsKey(varName))
                    {
                        throw new LjsRuntimeError($"variable not declared {varName}");
                    }
                    
                    _executionStack.Push(_vars[varName]);
                    
                    break;
                    
                default:
                    throw new LjsInternalError($"unsupported op code {instructionCode}");
                    
            }
            
            if (!jump) ++i;
        }

        return (_executionStack.Count > 0) ? _executionStack.Pop() : LjsObject.Undefined;
    }
}