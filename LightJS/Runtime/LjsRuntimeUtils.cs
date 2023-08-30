using LightJS.Errors;
using LightJS.Program;

namespace LightJS.Runtime;

public static class LjsRuntimeUtils
{

    public static LjsObject ExecuteArithmeticOperation(LjsObject left, LjsObject right, LjsInstructionCode opCode)
    {
        if (opCode == LjsInstructionCode.Add && (left is LjsValue<string> || right is LjsValue<string>))
        {
            return new LjsValue<string>(left.ToString() + right.ToString());
        }

        var isDouble = left is LjsValue<double> || right is LjsValue<double>;

        return isDouble ?
            ExecuteArithmeticOperation(GetDoubleValue(left), GetDoubleValue(right), opCode):
            ExecuteArithmeticOperation(GetIntValue(left), GetIntValue(right), opCode);
    }

    public static LjsObject ExecuteBitwiseOperation(LjsObject left, LjsObject right, LjsInstructionCode opCode)
    {
        var a = GetIntValue(left);
        var b = GetIntValue(right);
        
        switch (opCode)
        {
            case LjsInstructionCode.BitAnd:
                return new LjsValue<int>(a & b);
            case LjsInstructionCode.BitOr:
                return new LjsValue<int>(a | b);
            case LjsInstructionCode.BitShiftLeft:
                return new LjsValue<int>(a << b);
            case LjsInstructionCode.BitSShiftRight:
                return new LjsValue<int>(a >> b);
            case LjsInstructionCode.BitUShiftRight:
                return new LjsValue<int>(a >>> b);
            default:
                throw new LjsInternalError($"unsupported bitwise op code {opCode}");
        }
    }
    
    public static bool ToBool(LjsObject obj) => obj switch
    {
        LjsValue<int> i => i.Value == 0,
        LjsValue<double> i => i.Value == 0,
        LjsValue<bool> i => i.Value,
        LjsValue<string> i => !string.IsNullOrEmpty(i.Value),
        _ => obj != LjsObject.Null && obj != LjsObject.Undefined
    };

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

    public static LjsObject ExecuteArithmeticOperation(int left, int right, LjsInstructionCode opCode)
    {
        switch (opCode)
        {
            case LjsInstructionCode.Add:
                return new LjsValue<int>(left + right);
            case LjsInstructionCode.Sub:
                return new LjsValue<int>(left - right);
            case LjsInstructionCode.Mul:
                return new LjsValue<int>(left * right);
            case LjsInstructionCode.Div:
                return new LjsValue<int>(left / right);
            case LjsInstructionCode.Mod:
                return new LjsValue<int>(left % right);
            default:
                throw new LjsInternalError($"unsupported arithmetic op code {opCode}");
        }
    }
    
    public static LjsObject ExecuteArithmeticOperation(double left, double right, LjsInstructionCode opCode)
    {
        switch (opCode)
        {
            case LjsInstructionCode.Add:
                return new LjsValue<double>(left + right);
            case LjsInstructionCode.Sub:
                return new LjsValue<double>(left - right);
            case LjsInstructionCode.Mul:
                return new LjsValue<double>(left * right);
            case LjsInstructionCode.Div:
                return new LjsValue<double>(left / right);
            case LjsInstructionCode.Mod:
                return new LjsValue<double>(left % right);
            default:
                throw new LjsInternalError($"unsupported arithmetic op code {opCode}");
        }
    }

    public static bool IsNumber(LjsObject o) => o is LjsValue<int> or LjsValue<double>;

    public static LjsObject ExecuteComparisonOperation(LjsObject left, LjsObject right, LjsInstructionCode opCode)
    {
        switch (opCode)
        {
            case LjsInstructionCode.Gt:
                return GetDoubleValue(left) > GetDoubleValue(right) ? LjsValue.True : LjsValue.False;
                
            case LjsInstructionCode.Gte:
                return GetDoubleValue(left) >= GetDoubleValue(right) ? LjsValue.True : LjsValue.False;
                
            case LjsInstructionCode.Lt:
                return GetDoubleValue(left) < GetDoubleValue(right) ? LjsValue.True : LjsValue.False;
                
            case LjsInstructionCode.Lte:
                return GetDoubleValue(left) <= GetDoubleValue(right) ? LjsValue.True : LjsValue.False;
                
            case LjsInstructionCode.Eq:
            case LjsInstructionCode.Eqs:
                return left.Equals(right) ? LjsValue.True : LjsValue.False;
                
            case LjsInstructionCode.Neq:
            case LjsInstructionCode.Neqs:
                return left.Equals(right) ? LjsValue.False : LjsValue.True;
                
            default:
                throw new LjsInternalError($"unsupported comparison op code {opCode}");
        }
    }

    public static LjsObject ExecuteLogicalOperation(LjsObject left, LjsObject right, LjsInstructionCode opCode)
    {
        switch (opCode)
        {
            case LjsInstructionCode.And:
                return ToBool(left) && ToBool(right) ? LjsValue.True : LjsValue.False;
            case LjsInstructionCode.Or:
                return ToBool(left) || ToBool(right) ? LjsValue.True : LjsValue.False;
            default:
                throw new LjsInternalError($"unsupported logical op code {opCode}");
        }
    }

    public static LjsObject ExecuteUnaryOperation(LjsObject operand, LjsInstructionCode opCode)
    {
        switch (opCode)
        {
            case LjsInstructionCode.Minus:
                if (operand is LjsValue<double> d)
                {
                    return new LjsValue<double>(-d.Value);
                }

                return new LjsValue<int>(-GetIntValue(operand));
                
            case LjsInstructionCode.BitNot:
                return new LjsValue<int>(~GetIntValue(operand));
                
            case LjsInstructionCode.Not:
                return ToBool(operand) ? LjsValue.False : LjsValue.True;
                
            default:
                throw new LjsInternalError($"unsupported unary op code {opCode}");
        }
    }

    public static int CombineLocalIndexAndFunctionIndex(int localIndex, int funcIndex)
    {
        if (localIndex < 0 || localIndex >= short.MaxValue)
            throw new ArgumentException($"localIndex {localIndex} out of range");
        
        if (funcIndex < 0 || funcIndex >= short.MaxValue)
            throw new ArgumentException($"localIndex {localIndex} out of range");

        return (localIndex & 0x0000FFFF) | (funcIndex << 16);
    }

    public static int GetLocalIndex(int combinedValue) => combinedValue & 0x0000FFFF;
    public static int GetFunctionIndex(int combinedValue) => (combinedValue >>> 16) & 0x0000FFFF;


}