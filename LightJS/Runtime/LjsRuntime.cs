using LightJS.Errors;
using LightJS.Program;

namespace LightJS.Runtime;

public sealed class LjsRuntime
{
    private readonly LjsProgram _program;
    private readonly Stack<LjsObject> _stack = new();
    private readonly List<Context> _contextsStack = new();

    public LjsRuntime(LjsProgram program)
    {
        _program = program;
    }
    
    private sealed class Context
    {
        public readonly Dictionary<string, LjsObject> Vars = new();
        public int InstructionIndex = 0;
        public readonly LjsInstructionsList InstructionsList;

        public Context(LjsInstructionsList instructionsList)
        {
            InstructionsList = instructionsList;
        }
    }

    private LjsObject GetVarValue(string varName)
    {
        for (var i = _contextsStack.Count - 1; i >= 0; i++)
        {
            var ctx = _contextsStack[i];
            if (ctx.Vars.ContainsKey(varName))
            {
                return ctx.Vars[varName];
            }
        }
        
        throw new LjsRuntimeError($"variable not declared {varName}");
        
    }
    
    public LjsObject Execute()
    {
        var prg = _program;

        var ctx = new Context(prg.InstructionsList);

        foreach (var (funcName, func) in prg.Functions)
        {
            ctx.Vars[funcName] = func;
        }
        
        _contextsStack.Add(ctx);
        

        var varName = string.Empty;
        var v = LjsObject.Undefined;

        var execute = true;

        while (execute)
        {
            ctx = _contextsStack[^1];

            var vars = ctx.Vars;
            
            var i = ctx.InstructionIndex;
            
            var jump = false;
            var instruction = ctx.InstructionsList.Instructions[i];
            var instructionCode = instruction.Code;

            switch (instructionCode)
            {
                case LjsInstructionCode.Jump:
                    ctx.InstructionIndex = instruction.Index;
                    jump = true;
                    break;
                
                case LjsInstructionCode.JumpIfFalse:
                    var jumpConditionObj = _stack.Pop();
                    var jumpCondition = LjsRuntimeUtils.ToBool(jumpConditionObj);
                    if (!jumpCondition)
                    {
                        ctx.InstructionIndex = instruction.Index;
                        jump = true;
                    }
                    break;
                
                case LjsInstructionCode.FuncCall:

                    var funcRef = _stack.Pop();

                    if (funcRef is not LjsFunction f)
                    {
                        throw new LjsRuntimeError("not a function");
                    }
                    
                    throw new NotImplementedException();
                    break;
                    
                
                case LjsInstructionCode.Halt:
                    execute = false;
                    break;
                
                case LjsInstructionCode.ConstInt:
                    _stack.Push(new LjsValue<int>(prg.GetIntegerConstant(instruction.Index)));
                    break;
                case LjsInstructionCode.ConstDouble:
                    _stack.Push(new LjsValue<double>(prg.GetDoubleConstant(instruction.Index)));
                    break;
                case LjsInstructionCode.ConstString:
                    _stack.Push(new LjsValue<string>(prg.GetStringConstant(instruction.Index)));
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
                    varName = prg.GetStringConstant(instruction.Index);
                    if (vars.ContainsKey(varName))
                    {
                        throw new LjsRuntimeError($"variable already declared {varName}");
                    }
                    vars[varName] = LjsObject.Undefined;
                    break;
                
                case LjsInstructionCode.VarInit:
                    
                    varName = prg.GetStringConstant(instruction.Index);
                    v = _stack.Pop();
                    
                    vars[varName] = v;
                    break;
                
                case LjsInstructionCode.VarStore:
                    varName = prg.GetStringConstant(instruction.Index);
                    
                    v = _stack.Peek();
                    
                    vars[varName] = v;
                    break;
                
                case LjsInstructionCode.VarLoad:
                    
                    varName = prg.GetStringConstant(instruction.Index);
                    
                    if (!vars.ContainsKey(varName))
                    {
                        throw new LjsRuntimeError($"variable not declared {varName}");
                    }
                    
                    _stack.Push(vars[varName]);
                    
                    break;
                    
                default:
                    throw new LjsInternalError($"unsupported op code {instructionCode}");
                    
            }
            
            if (!jump) ++ctx.InstructionIndex;
        }

        return (_stack.Count > 0) ? _stack.Pop() : LjsObject.Undefined;
    }
}