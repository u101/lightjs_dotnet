using App16.ALang.Js.Ast;
using App16.ALang.Tokenizers;
using App16.LightJS.Errors;
using App16.LightJS.Program;
using App16.LightJS.Runtime;

namespace App16.LightJS.Compiler;

public static class LjsCompileUtils
{
    public static LjsInstruction GetIntLiteralInstruction(int value) => value switch
    {
        0 => new LjsInstruction(LjsInstructionCode.ConstIntZero),
        1 => new LjsInstruction(LjsInstructionCode.ConstIntOne),
        -1 => new LjsInstruction(LjsInstructionCode.ConstIntMinusOne),
        _ => new LjsInstruction(LjsInstructionCode.ConstInt, value)
    };
    
    public static LjsInstruction GetDoubleLiteralInstruction(double value, LjsProgramConstants constants)
    {
        return value switch
        {
            double.NaN => new LjsInstruction(LjsInstructionCode.ConstDoubleNaN),
            0 => new LjsInstruction(LjsInstructionCode.ConstDoubleZero),
            _ => new LjsInstruction(LjsInstructionCode.ConstDouble, 
                constants.AddDoubleConstant(value))
        };
    }
    
    public static LjsInstruction GetStringConstInstruction(string s, LjsProgramConstants constants) =>
        !string.IsNullOrEmpty(s)
            ? new LjsInstruction(LjsInstructionCode.ConstString, constants.AddStringConstant(s))
            : new LjsInstruction(LjsInstructionCode.ConstStringEmpty);
    
    public static LjsInstructionCode GetComplexAssignmentOpCode(LjsCompilerAssignMode assignMode)
    {
        switch (assignMode)
        {
            case LjsCompilerAssignMode.PlusAssign: return LjsInstructionCode.Add;
            case LjsCompilerAssignMode.MinusAssign: return LjsInstructionCode.Sub;
            case LjsCompilerAssignMode.MulAssign: return LjsInstructionCode.Mul;
            case LjsCompilerAssignMode.DivAssign: return LjsInstructionCode.Div;
            case LjsCompilerAssignMode.BitOrAssign: return LjsInstructionCode.BitOr;
            case LjsCompilerAssignMode.BitAndAssign: return LjsInstructionCode.BitAnd;
            case LjsCompilerAssignMode.LogicalOrAssign: return LjsInstructionCode.Or;
            case LjsCompilerAssignMode.LogicalAndAssign: return LjsInstructionCode.And;
            default:
                throw new LjsInternalError($"invalid complex assign mode {assignMode}");
        }
    }
    
    public static LjsCompilerAssignMode GetAssignMode(int binaryOperationType)
    {
        return binaryOperationType switch
        {
            JsBinaryOperationTypes.Assign => LjsCompilerAssignMode.Normal,
            JsBinaryOperationTypes.PlusAssign => LjsCompilerAssignMode.PlusAssign,
            JsBinaryOperationTypes.MinusAssign => LjsCompilerAssignMode.MinusAssign,
            JsBinaryOperationTypes.MulAssign => LjsCompilerAssignMode.MulAssign,
            JsBinaryOperationTypes.DivAssign => LjsCompilerAssignMode.DivAssign,
            JsBinaryOperationTypes.BitOrAssign => LjsCompilerAssignMode.BitOrAssign,
            JsBinaryOperationTypes.BitAndAssign => LjsCompilerAssignMode.BitAndAssign,
            JsBinaryOperationTypes.LogicalOrAssign => LjsCompilerAssignMode.LogicalOrAssign,
            JsBinaryOperationTypes.LogicalAndAssign => LjsCompilerAssignMode.LogicalAndAssign,
            _ => throw new ArgumentOutOfRangeException(binaryOperationType.ToString())
        };
    }
    
    public static LjsInstruction CreateVarLoadInstruction(string varName, LjsCompilerContext context)
    {

        if (context.HasLocal(varName)) 
            return new LjsInstruction(LjsInstructionCode.VarLoad, context.GetLocal(varName).Index);

        if (context.HasFunctionWithName(varName))
        {
            var functionIndex = context.GetFunctionIndex(varName);
            return new LjsInstruction(LjsInstructionCode.FuncRef, functionIndex);
        }

        if (context.HasLocalInHierarchy(varName))
        {
            var (localVarPointer, functionIndex) = context.GetLocalInHierarchy(varName);
            
            var instructionArg = LjsRuntimeUtils.CombineTwoShorts(
                localVarPointer.Index, functionIndex);
            
            return new LjsInstruction(LjsInstructionCode.ParentVarLoad, instructionArg);
        }
        
        return new LjsInstruction(
            LjsInstructionCode.ExtLoad, context.Constants.AddStringConstant(varName));
    }
    
    public static LjsInstruction CreateVarStoreInstruction(string varName, LjsCompilerContext context, Token varToken)
    {
        
        if (context.HasLocal(varName))
        {
            var localVarPointer = context.GetLocal(varName);
            
            AssertPointerIsWritable(localVarPointer, varToken);

            return new LjsInstruction(LjsInstructionCode.VarStore, localVarPointer.Index);
        } 
            
        
        if (context.HasLocalInHierarchy(varName))
        {
            var (localPointer, functionIndex) = context.GetLocalInHierarchy(varName);

            AssertPointerIsWritable(localPointer, varToken);

            var instructionArg = 
                LjsRuntimeUtils.CombineTwoShorts(localPointer.Index, functionIndex);
            
            return new LjsInstruction(LjsInstructionCode.ParentVarStore, instructionArg);
        }
        
        return new LjsInstruction(
            LjsInstructionCode.ExtStore, context.Constants.AddStringConstant(varName));
    }
    
    private static void AssertPointerIsWritable(LjsLocalVarPointer varPointer, Token varToken)
    {
        if (varPointer.VarKind == LjsLocalVarKind.Const)
        {
            throw new LjsCompilerError($"invalid const {varPointer.Name} assign", varToken);
        }
    }
    
    public static bool IsAssignOperation(int binaryOperationType) =>
        binaryOperationType == JsBinaryOperationTypes.Assign ||
        binaryOperationType == JsBinaryOperationTypes.PlusAssign ||
        binaryOperationType == JsBinaryOperationTypes.MinusAssign ||
        binaryOperationType == JsBinaryOperationTypes.MulAssign ||
        binaryOperationType == JsBinaryOperationTypes.DivAssign ||
        binaryOperationType == JsBinaryOperationTypes.BitOrAssign ||
        binaryOperationType == JsBinaryOperationTypes.BitAndAssign ||
        binaryOperationType == JsBinaryOperationTypes.LogicalOrAssign ||
        binaryOperationType == JsBinaryOperationTypes.LogicalAndAssign;
    
    public static LjsInstructionCode GetBinaryOpCode(int binaryOperationType)
    {
        switch (binaryOperationType)
        {
            case JsBinaryOperationTypes.Plus:
                return LjsInstructionCode.Add;

            case JsBinaryOperationTypes.Minus:
                return LjsInstructionCode.Sub;

            case JsBinaryOperationTypes.Multiply:
                return LjsInstructionCode.Mul; 
            
            case JsBinaryOperationTypes.Pow:
                return LjsInstructionCode.Pow;

            case JsBinaryOperationTypes.Div:
                return LjsInstructionCode.Div;

            case JsBinaryOperationTypes.Modulo:
                return LjsInstructionCode.Mod;

            case JsBinaryOperationTypes.BitAnd:
                return LjsInstructionCode.BitAnd;

            case JsBinaryOperationTypes.BitOr:
                return LjsInstructionCode.BitOr;

            case JsBinaryOperationTypes.BitLeftShift:
                return LjsInstructionCode.BitShiftLeft;

            case JsBinaryOperationTypes.BitRightShift:
                return LjsInstructionCode.BitSShiftRight;

            case JsBinaryOperationTypes.BitUnsignedRightShift:
                return LjsInstructionCode.BitUShiftRight;

            case JsBinaryOperationTypes.Greater:
                return LjsInstructionCode.Gt;

            case JsBinaryOperationTypes.GreaterOrEqual:
                return LjsInstructionCode.Gte;

            case JsBinaryOperationTypes.Less:
                return LjsInstructionCode.Lt;

            case JsBinaryOperationTypes.LessOrEqual:
                return LjsInstructionCode.Lte;

            case JsBinaryOperationTypes.Equals:
                return LjsInstructionCode.Eq;

            case JsBinaryOperationTypes.EqualsStrict:
                return LjsInstructionCode.Eqs;

            case JsBinaryOperationTypes.NotEqual:
                return LjsInstructionCode.Neq;

            case JsBinaryOperationTypes.NotEqualStrict:
                return LjsInstructionCode.Neqs;

            case JsBinaryOperationTypes.LogicalAnd:
                return LjsInstructionCode.And;

            case JsBinaryOperationTypes.LogicalOr:
                return LjsInstructionCode.Or;

            default:
                throw new ArgumentOutOfRangeException(binaryOperationType.ToString());
        }
        
        
    }

    internal static LjsLocalVarKind GetVarKind(JsVariableKind varKind) => varKind switch
    {
        JsVariableKind.Var => LjsLocalVarKind.Var,
        JsVariableKind.Const => LjsLocalVarKind.Const,
        JsVariableKind.Let => LjsLocalVarKind.Let,
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