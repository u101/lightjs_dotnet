using static App16.LightJS.Runtime.LjsTypesCoercionUtil;

using App16.LightJS.Errors;
using App16.LightJS.Program;

namespace App16.LightJS.Runtime;

internal static class LjsBasicOperationsHelper
{
    internal static LjsObject ExecuteArithmeticOperation(LjsObject left, LjsObject right, LjsInstructionCode opCode)
    {
        if (opCode == LjsInstructionCode.Add && (left is LjsString || right is LjsString))
        {
            return new LjsString(left.ToString() + right.ToString());
        }

        var isDouble = left is LjsDouble || right is LjsDouble || opCode == LjsInstructionCode.Div;

        return isDouble ?
            ExecuteArithmeticOperation(ToDouble(left), ToDouble(right), opCode):
            ExecuteArithmeticOperation(ToInt(left), ToInt(right), opCode);
    }

    internal static LjsObject ExecuteBitwiseOperation(LjsObject left, LjsObject right, LjsInstructionCode opCode)
    {
        var a = ToInt(left);
        var b = ToInt(right);
        
        switch (opCode)
        {
            case LjsInstructionCode.BitAnd:
                return new LjsInteger(a & b);
            case LjsInstructionCode.BitOr:
                return new LjsInteger(a | b);
            case LjsInstructionCode.BitShiftLeft:
                return new LjsInteger(a << b);
            case LjsInstructionCode.BitSShiftRight:
                return new LjsInteger(a >> b);
            case LjsInstructionCode.BitUShiftRight:
                return new LjsInteger(a >>> b);
            default:
                throw new LjsInternalError($"unsupported bitwise op code {opCode}");
        }
    }
    
    internal static LjsObject ExecuteArithmeticOperation(int left, int right, LjsInstructionCode opCode)
    {
        switch (opCode)
        {
            case LjsInstructionCode.Add:
                return new LjsInteger(left + right);
            case LjsInstructionCode.Sub:
                return new LjsInteger(left - right);
            case LjsInstructionCode.Mul:
                return new LjsInteger(left * right);
            case LjsInstructionCode.Div:
                return new LjsInteger(left / right);
            case LjsInstructionCode.Mod:
                return new LjsInteger(left % right);
            case LjsInstructionCode.Pow:
                return new LjsDouble(Math.Pow(left, right));
            default:
                throw new LjsInternalError($"unsupported arithmetic op code {opCode}");
        }
    }
    
    internal static LjsObject ExecuteArithmeticOperation(double left, double right, LjsInstructionCode opCode)
    {
        switch (opCode)
        {
            case LjsInstructionCode.Add:
                return new LjsDouble(left + right);
            case LjsInstructionCode.Sub:
                return new LjsDouble(left - right);
            case LjsInstructionCode.Mul:
                return new LjsDouble(left * right);
            case LjsInstructionCode.Div:
                return new LjsDouble(left / right);
            case LjsInstructionCode.Mod:
                return new LjsDouble(left % right);
            case LjsInstructionCode.Pow:
                return new LjsDouble(Math.Pow(left, right));
            default:
                throw new LjsInternalError($"unsupported arithmetic op code {opCode}");
        }
    }
    
    internal static LjsObject ExecuteComparisonOperation(LjsObject left, LjsObject right, LjsInstructionCode opCode)
    {
        switch (opCode)
        {
            case LjsInstructionCode.Gt:
                return ToDouble(left) > ToDouble(right) ? LjsBoolean.True : LjsBoolean.False;
                
            case LjsInstructionCode.Gte:
                return ToDouble(left) >= ToDouble(right) ? LjsBoolean.True : LjsBoolean.False;
                
            case LjsInstructionCode.Lt:
                return ToDouble(left) < ToDouble(right) ? LjsBoolean.True : LjsBoolean.False;
                
            case LjsInstructionCode.Lte:
                return ToDouble(left) <= ToDouble(right) ? LjsBoolean.True : LjsBoolean.False;
                
            case LjsInstructionCode.Eq:
            case LjsInstructionCode.Eqs:
                return left.Equals(right) ? LjsBoolean.True : LjsBoolean.False;
                
            case LjsInstructionCode.Neq:
            case LjsInstructionCode.Neqs:
                return left.Equals(right) ? LjsBoolean.False : LjsBoolean.True;
                
            default:
                throw new LjsInternalError($"unsupported comparison op code {opCode}");
        }
    }
    
    internal static LjsObject ExecuteLogicalOperation(LjsObject left, LjsObject right, LjsInstructionCode opCode)
    {
        switch (opCode)
        {
            case LjsInstructionCode.And:
                return ToBool(left) && ToBool(right) ? LjsBoolean.True : LjsBoolean.False;
            case LjsInstructionCode.Or:
                return ToBool(left) || ToBool(right) ? LjsBoolean.True : LjsBoolean.False;
            default:
                throw new LjsInternalError($"unsupported logical op code {opCode}");
        }
    }

    internal static LjsObject ExecuteUnaryOperation(LjsObject operand, LjsInstructionCode opCode)
    {
        switch (opCode)
        {
            case LjsInstructionCode.Minus:
                if (operand is LjsDouble d)
                {
                    return new LjsDouble(-d.Value);
                }

                return new LjsInteger(-ToInt(operand));
                
            case LjsInstructionCode.BitNot:
                return new LjsInteger(~ToInt(operand));
                
            case LjsInstructionCode.Not:
                return ToBool(operand) ? LjsBoolean.False : LjsBoolean.True;
                
            default:
                throw new LjsInternalError($"unsupported unary op code {opCode}");
        }
    }
}