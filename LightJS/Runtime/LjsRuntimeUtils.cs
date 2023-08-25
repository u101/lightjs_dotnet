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

    public static int GetIntValue(LjsObject obj) => obj switch
    {
        LjsValue<int> i => i.Value,
        LjsValue<double> i => (int)i.Value,
        LjsValue<bool> i => i.Value ? 1 : 0,
        _ => obj == LjsObject.Null || obj == LjsObject.Undefined ? 0 : 1
    };
    
    public static double GetDoubleValue(LjsObject obj) => obj switch
    {
        LjsValue<int> i => i.Value,
        LjsValue<double> i => i.Value,
        LjsValue<bool> i => i.Value ? 1 : 0,
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
            default:
                throw new LjsInternalError($"unsupported arithmetic op code {opCode}");
        }
    }

    public static bool IsNumber(LjsObject o) => o is LjsValue<int> or LjsValue<double>;



}