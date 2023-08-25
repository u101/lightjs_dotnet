using LightJS.Ast;
using LightJS.Program;

namespace LightJS.Compiler;

public static class LjsCompileUtils
{
    
    
    public static byte GetBinaryOpCode(LjsAstBinaryOperationType binaryOperationType)
    {
        switch (binaryOperationType)
        {
            case LjsAstBinaryOperationType.Plus:
                return LjsInstructionCodes.Add;

            case LjsAstBinaryOperationType.Minus:
                return LjsInstructionCodes.Sub;

            case LjsAstBinaryOperationType.Multiply:
                return LjsInstructionCodes.Mul;

            case LjsAstBinaryOperationType.Div:
                return LjsInstructionCodes.Div;

            case LjsAstBinaryOperationType.Modulo:
                return LjsInstructionCodes.Mod;

            case LjsAstBinaryOperationType.BitAnd:
                return LjsInstructionCodes.BitAnd;

            case LjsAstBinaryOperationType.BitOr:
                return LjsInstructionCodes.BitOr;

            case LjsAstBinaryOperationType.BitLeftShift:
                return LjsInstructionCodes.BitShiftLeft;

            case LjsAstBinaryOperationType.BitRightShift:
                return LjsInstructionCodes.BitSShiftRight;

            case LjsAstBinaryOperationType.BitUnsignedRightShift:
                return LjsInstructionCodes.BitUShiftRight;

            case LjsAstBinaryOperationType.Greater:
                return LjsInstructionCodes.Gt;

            case LjsAstBinaryOperationType.GreaterOrEqual:
                return LjsInstructionCodes.Gte;

            case LjsAstBinaryOperationType.Less:
                return LjsInstructionCodes.Lt;

            case LjsAstBinaryOperationType.LessOrEqual:
                return LjsInstructionCodes.Lte;

            case LjsAstBinaryOperationType.Equals:
                return LjsInstructionCodes.Eq;

            case LjsAstBinaryOperationType.EqualsStrict:
                return LjsInstructionCodes.Eqs;

            case LjsAstBinaryOperationType.NotEqual:
                return LjsInstructionCodes.Neq;

            case LjsAstBinaryOperationType.NotEqualStrict:
                return LjsInstructionCodes.Neqs;

            case LjsAstBinaryOperationType.LogicalAnd:
                return LjsInstructionCodes.And;

            case LjsAstBinaryOperationType.LogicalOr:
                return LjsInstructionCodes.Or;

            default:
                throw new ArgumentOutOfRangeException(binaryOperationType.ToString());
        }
    }
}