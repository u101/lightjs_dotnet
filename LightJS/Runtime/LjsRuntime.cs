using LightJS.Errors;
using LightJS.Program;

namespace LightJS.Runtime;

public sealed class LjsRuntime
{
    private readonly LjsProgram _program;
    private readonly Stack<LjsObject> _executionStack = new();

    public LjsRuntime(LjsProgram program)
    {
        _program = program;
    }
    
    public LjsObject Execute()
    {
        var prg = _program;
        var instructions = prg.Instructions;
        var ln = instructions.Count;

        for (var i = 0; i < ln; i++)
        {
            var instruction = instructions[i];

            var instructionCode = instruction.Code;
            switch (instructionCode)
            {
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
                default:
                    throw new LjsInternalError($"unsupported op code {instructionCode}");
                    
            }
            
        }

        return (_executionStack.Count > 0) ? _executionStack.Pop() : LjsObject.Undefined;
    }
}