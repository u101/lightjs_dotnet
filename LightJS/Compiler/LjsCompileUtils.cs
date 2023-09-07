using LightJS.Ast;
using LightJS.Errors;
using LightJS.Program;

namespace LightJS.Compiler;

internal static class LjsCompileUtils
{
    
    
    public static LjsInstructionCode GetBinaryOpCode(LjsAstBinaryOperationType binaryOperationType)
    {
        switch (binaryOperationType)
        {
            case LjsAstBinaryOperationType.Plus:
                return LjsInstructionCode.Add;

            case LjsAstBinaryOperationType.Minus:
                return LjsInstructionCode.Sub;

            case LjsAstBinaryOperationType.Multiply:
                return LjsInstructionCode.Mul; 
            
            case LjsAstBinaryOperationType.Pow:
                return LjsInstructionCode.Pow;

            case LjsAstBinaryOperationType.Div:
                return LjsInstructionCode.Div;

            case LjsAstBinaryOperationType.Modulo:
                return LjsInstructionCode.Mod;

            case LjsAstBinaryOperationType.BitAnd:
                return LjsInstructionCode.BitAnd;

            case LjsAstBinaryOperationType.BitOr:
                return LjsInstructionCode.BitOr;

            case LjsAstBinaryOperationType.BitLeftShift:
                return LjsInstructionCode.BitShiftLeft;

            case LjsAstBinaryOperationType.BitRightShift:
                return LjsInstructionCode.BitSShiftRight;

            case LjsAstBinaryOperationType.BitUnsignedRightShift:
                return LjsInstructionCode.BitUShiftRight;

            case LjsAstBinaryOperationType.Greater:
                return LjsInstructionCode.Gt;

            case LjsAstBinaryOperationType.GreaterOrEqual:
                return LjsInstructionCode.Gte;

            case LjsAstBinaryOperationType.Less:
                return LjsInstructionCode.Lt;

            case LjsAstBinaryOperationType.LessOrEqual:
                return LjsInstructionCode.Lte;

            case LjsAstBinaryOperationType.Equals:
                return LjsInstructionCode.Eq;

            case LjsAstBinaryOperationType.EqualsStrict:
                return LjsInstructionCode.Eqs;

            case LjsAstBinaryOperationType.NotEqual:
                return LjsInstructionCode.Neq;

            case LjsAstBinaryOperationType.NotEqualStrict:
                return LjsInstructionCode.Neqs;

            case LjsAstBinaryOperationType.LogicalAnd:
                return LjsInstructionCode.And;

            case LjsAstBinaryOperationType.LogicalOr:
                return LjsInstructionCode.Or;

            default:
                throw new ArgumentOutOfRangeException(binaryOperationType.ToString());
        }
        
        
    }

    public static LjsInstructionCode GetComplexAssignmentOpCode(LjsAstAssignMode assignMode)
    {
        switch (assignMode)
        {
            case LjsAstAssignMode.PlusAssign: return LjsInstructionCode.Add;
            case LjsAstAssignMode.MinusAssign: return LjsInstructionCode.Sub;
            case LjsAstAssignMode.MulAssign: return LjsInstructionCode.Mul;
            case LjsAstAssignMode.DivAssign: return LjsInstructionCode.Div;
            case LjsAstAssignMode.BitOrAssign: return LjsInstructionCode.BitOr;
            case LjsAstAssignMode.BitAndAssign: return LjsInstructionCode.BitAnd;
            case LjsAstAssignMode.LogicalOrAssign: return LjsInstructionCode.Or;
            case LjsAstAssignMode.LogicalAndAssign: return LjsInstructionCode.And;
            default:
                throw new LjsInternalError($"invalid complex assign mode {assignMode}");
        }
    }

    public static LjsInstructionCode GetIncrementOpCode(LjsAstIncrementSign incrementSign) => incrementSign switch
    {
        LjsAstIncrementSign.Plus => LjsInstructionCode.Add,
        LjsAstIncrementSign.Minus => LjsInstructionCode.Sub,
        _ => throw new LjsInternalError($"invalid increment sign {incrementSign}")
    };

    internal static LjsLocalVarKind GetVarKind(LjsAstVariableKind varKind) => varKind switch
    {
        LjsAstVariableKind.Var => LjsLocalVarKind.Var,
        LjsAstVariableKind.Const => LjsLocalVarKind.Const,
        LjsAstVariableKind.Let => LjsLocalVarKind.Let,
        _ => throw new Exception($"unknown var kind {varKind}")
    };

    private static readonly List<List<int>> IntListsPool = new();

    public static List<int> GetTemporaryIntList()
    {
        if (IntListsPool.Count > 0)
        {
            var list = IntListsPool[^1];
            IntListsPool.RemoveAt(IntListsPool.Count - 1);
            list.Clear();
            return list;
        }

        return new List<int>(8);
    }

    public static void ReleaseTemporaryIntList(List<int> list)
    {
        list.Clear();
        IntListsPool.Add(list);
    }
}