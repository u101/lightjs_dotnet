using LightJS.Errors;
using LightJS.Program;

namespace LightJS.Runtime;

public static class LjsRuntimeUtils
{

    public static LjsObject ExecuteArithmeticOperation(LjsObject left, LjsObject right, byte opCode)
    {
        if (opCode == LjsInstructionCodes.Add && (left is LjsValue<string> || right is LjsValue<string>))
        {
            return new LjsValue<string>(left.ToString() + right.ToString());
        }

        var isDouble = left is LjsValue<double> || right is LjsValue<double>;

        return isDouble ?
            ExecuteArithmeticOperation(GetDoubleValue(left), GetDoubleValue(right), opCode):
            ExecuteArithmeticOperation(GetIntValue(left), GetIntValue(right), opCode);
    }

    public static LjsObject ExecuteBitwiseOperation(LjsObject left, LjsObject right, byte opCode)
    {
        var a = GetIntValue(left);
        var b = GetIntValue(right);
        
        switch (opCode)
        {
            case LjsInstructionCodes.BitAnd:
                return new LjsValue<int>(a & b);
            case LjsInstructionCodes.BitOr:
                return new LjsValue<int>(a | b);
            case LjsInstructionCodes.BitShiftLeft:
                return new LjsValue<int>(a << b);
            case LjsInstructionCodes.BitSShiftRight:
                return new LjsValue<int>(a >> b);
            case LjsInstructionCodes.BitUShiftRight:
                return new LjsValue<int>(a >>> b);
            default:
                throw new LjsInternalError($"unsupported bitwise op code {opCode}");
        }
    }

    public static int GetIntValue(LjsObject obj) => obj switch
    {
        LjsValue<int> i => i.Value,
        LjsValue<double> i => (int)i.Value,
        LjsValue<bool> i => i.Value ? 1 : 0,
        LjsValue<string> i => i.Value.Length,
        _ => obj == LjsObject.Null || obj == LjsObject.Undefined ? 0 : 1
    };
    
    public static double GetDoubleValue(LjsObject obj) => obj switch
    {
        LjsValue<int> i => i.Value,
        LjsValue<double> i => i.Value,
        LjsValue<bool> i => i.Value ? 1 : 0,
        LjsValue<string> i => i.Value.Length,
        _ => obj == LjsObject.Null || obj == LjsObject.Undefined ? 0 : 1
    };

    public static LjsObject ExecuteArithmeticOperation(int left, int right, byte opCode)
    {
        switch (opCode)
        {
            case LjsInstructionCodes.Add:
                return new LjsValue<int>(left + right);
            case LjsInstructionCodes.Sub:
                return new LjsValue<int>(left - right);
            case LjsInstructionCodes.Mul:
                return new LjsValue<int>(left * right);
            case LjsInstructionCodes.Div:
                return new LjsValue<int>(left / right);
            case LjsInstructionCodes.Mod:
                return new LjsValue<int>(left % right);
            default:
                throw new LjsInternalError($"unsupported arithmetic op code {opCode}");
        }
    }
    
    public static LjsObject ExecuteArithmeticOperation(double left, double right, byte opCode)
    {
        switch (opCode)
        {
            case LjsInstructionCodes.Add:
                return new LjsValue<double>(left + right);
            case LjsInstructionCodes.Sub:
                return new LjsValue<double>(left - right);
            case LjsInstructionCodes.Mul:
                return new LjsValue<double>(left * right);
            case LjsInstructionCodes.Div:
                return new LjsValue<double>(left / right);
            case LjsInstructionCodes.Mod:
                return new LjsValue<double>(left % right);
            default:
                throw new LjsInternalError($"unsupported arithmetic op code {opCode}");
        }
    }

    public static bool IsNumber(LjsObject o) => o is LjsValue<int> or LjsValue<double>;

    public static LjsObject ExecuteComparisonOperation(LjsObject left, LjsObject right, byte opCode)
    {
        switch (opCode)
        {
            case LjsInstructionCodes.Gt:
                return GetDoubleValue(left) > GetDoubleValue(right) ? LjsValue.True : LjsValue.False;
                
            case LjsInstructionCodes.Gte:
                return GetDoubleValue(left) >= GetDoubleValue(right) ? LjsValue.True : LjsValue.False;
                
            case LjsInstructionCodes.Lt:
                return GetDoubleValue(left) < GetDoubleValue(right) ? LjsValue.True : LjsValue.False;
                
            case LjsInstructionCodes.Lte:
                return GetDoubleValue(left) <= GetDoubleValue(right) ? LjsValue.True : LjsValue.False;
                
            case LjsInstructionCodes.Eq:
            case LjsInstructionCodes.Eqs:
                return left.Equals(right) ? LjsValue.True : LjsValue.False;
                
            case LjsInstructionCodes.Neq:
            case LjsInstructionCodes.Neqs:
                return left.Equals(right) ? LjsValue.False : LjsValue.True;
                
            default:
                throw new LjsInternalError($"unsupported comparison op code {opCode}");
        }
    }



}