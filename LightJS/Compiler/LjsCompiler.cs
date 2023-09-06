using LightJS.Ast;
using LightJS.Errors;
using LightJS.Program;
using LightJS.Runtime;
using LightJS.Tokenizer;

namespace LightJS.Compiler;

public class LjsCompiler
{
    private readonly LjsAstModel _astModel;
    private readonly LjsProgramConstants _constants = new();
    private readonly List<LjsCompilerFunctionData> _functionsList = new();

    public LjsCompiler(string sourceCodeString)
    {
        if (string.IsNullOrEmpty(sourceCodeString))
        {
            throw new ArgumentException("input string is null or empty");
        }

        var astModelBuilder = new LjsAstBuilder(sourceCodeString);
        
        _astModel = astModelBuilder.Build();
    }

    public LjsCompiler(string sourceCodeString, List<LjsToken> tokens)
    {
        if (string.IsNullOrEmpty(sourceCodeString))
        {
            throw new ArgumentException("input string is null or empty");
        }
        
        if (tokens == null)
            throw new ArgumentNullException(nameof(tokens));

        if (tokens.Count == 0)
            throw new ArgumentException("empty tokens list");
        
        var astModelBuilder = new LjsAstBuilder(sourceCodeString, tokens);
        
        _astModel = astModelBuilder.Build();
    }
    
    public LjsCompiler(LjsAstModel astModel)
    {
        _astModel = astModel;
    }

    public LjsProgram Compile()
    {
        var context = new LjsCompilerFunctionData(0, _functionsList);
        
        _functionsList.Add(context);
        
        ProcessNode(_astModel.RootNode, context);

        context.Instructions.Add(
            new LjsInstruction(LjsInstructionCode.Halt));

        var functions = _functionsList.Select(fd => new LjsFunctionData(
            fd.FunctionIndex,
            fd.Instructions.Instructions.ToArray(), 
            fd.Args.ToArray(), 
            fd.LocalVars.ToArray()
        )).ToArray();
        
        return new LjsProgram(
            _constants, functions, context.NamedFunctionsMap);
    }
    private void ProcessFunction(
        LjsAstFunctionDeclaration functionDeclaration, 
        LjsCompilerFunctionData functionData)
    {

        var parameters = functionDeclaration.Parameters;

        for (var i = 0; i < parameters.Length; i++)
        {
            var p = parameters[i];
            var defaultValue = GetFunctionParameterDefaultValue(p.DefaultValue);
            
            functionData.Args.Add(new LjsFunctionArgument(p.Name, defaultValue));

            functionData.AddLocal(p.Name);
        }
        
        ProcessNode(functionDeclaration.FunctionBody, functionData);

        if (functionData.Instructions.Count == 0 ||
            functionData.Instructions.LastInstruction.Code != LjsInstructionCode.Return)
        {
            functionData.Instructions.Add(new LjsInstruction(LjsInstructionCode.ConstUndef));
            functionData.Instructions.Add(new LjsInstruction(LjsInstructionCode.Return));
        }
    }

    private static LjsObject GetFunctionParameterDefaultValue(ILjsAstNode node) => node switch
    {
        LjsAstNull _ => LjsObject.Null,
        LjsAstUndefined _ => LjsObject.Undefined,
        LjsAstLiteral<int> i => new LjsInteger(i.Value),
        LjsAstLiteral<double> i => new LjsDouble(i.Value),
        LjsAstLiteral<string> i => new LjsString(i.Value),
        LjsAstLiteral<bool> i => i.Value ? LjsBoolean.True : LjsBoolean.False,
        _ => LjsObject.Undefined
    };

    
    
    private LjsCompilerFunctionData CreateAnonFunction(LjsCompilerFunctionData parentFunction)
    {
        var functionIndex = _functionsList.Count;
        var func = parentFunction.CreateChild(functionIndex);

        _functionsList.Add(func);
        return func;
    }

    

    private void ProcessNode(
        ILjsAstNode node, 
        LjsCompilerFunctionData functionData,
        ICollection<int>? jumpToTheStartPlaceholdersIndices = null, 
        ICollection<int>? jumpToTheEndPlaceholdersIndices = null)
    {
        var instructions = functionData.Instructions;

        switch (node)
        {
            case LjsAstAnonymousFunctionDeclaration funcDeclaration:
                
                var anonFunc = CreateAnonFunction(functionData);
                
                ProcessFunction(funcDeclaration, anonFunc);
                
                instructions.Add(new LjsInstruction(LjsInstructionCode.FuncRef, anonFunc.FunctionIndex));
                
                break;
            
            case LjsAstNamedFunctionDeclaration namedFunctionDeclaration:
                
                var (namedFunc, namedFuncIndex) = functionData.
                    GetOrCreateNamedFunctionData(namedFunctionDeclaration);
                
                ProcessFunction(namedFunctionDeclaration, namedFunc);
                break;
            
            case LjsAstFunctionCall functionCall:

                var specifiedArgumentsCount = functionCall.Arguments.Count;
                
                foreach (var n in functionCall.Arguments.ChildNodes)
                {
                    ProcessNode(n, functionData);
                }
                
                ProcessNode(functionCall.FunctionGetter, functionData);
                
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.FuncCall, specifiedArgumentsCount));
                
                break;
            
            case LjsAstReturn astReturn:

                if (astReturn.ReturnValue != LjsAstEmptyNode.Instance)
                {
                    ProcessNode(astReturn.ReturnValue, functionData);
                }
                else
                {
                    instructions.Add(new LjsInstruction(LjsInstructionCode.ConstUndef));
                }
                
                instructions.Add(new LjsInstruction(LjsInstructionCode.Return));
                break;
            
            case LjsAstEmptyNode _:
                // do nothing
                break;
            
            case LjsAstBreak _:
                
                if (jumpToTheEndPlaceholdersIndices == null)
                    throw new LjsCompilerError("unexpected break statement");
                
                jumpToTheEndPlaceholdersIndices.Add(instructions.Count);
                instructions.Add(default);
                
                break;
            
            case LjsAstContinue _:
                if (jumpToTheStartPlaceholdersIndices == null)
                    throw new LjsCompilerError("unexpected continue statement");
                jumpToTheStartPlaceholdersIndices.Add(instructions.Count);
                instructions.Add(default);
                break;
            
            case LjsAstLiteral<int> lit:
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.ConstInt, lit.Value));
                break;
            
            case LjsAstLiteral<double> lit:
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.ConstDouble, 
                    _constants.AddDoubleConstant(lit.Value)));
                break;
            
            case LjsAstLiteral<string> lit:
                instructions.Add(GetStringConstInstruction(lit.Value));
                break;
            
            case LjsAstNull _:
                instructions.Add(
                    new LjsInstruction(LjsInstructionCode.ConstNull));
                break;
            case LjsAstUndefined _:
                instructions.Add(
                    new LjsInstruction(LjsInstructionCode.ConstUndef));
                break;
            
            case LjsAstLiteral<bool> lit:
                instructions.Add(new LjsInstruction(
                    lit.Value ? LjsInstructionCode.ConstTrue : LjsInstructionCode.ConstFalse));
                break;
            
            case LjsAstBinaryOperation binaryOperation:
                
                ProcessNode(binaryOperation.LeftOperand, functionData);
                ProcessNode(binaryOperation.RightOperand, functionData);

                instructions.Add(new LjsInstruction(LjsCompileUtils.GetBinaryOpCode(binaryOperation.OperatorType)));
                
                break;
            
            case LjsAstUnaryOperation unaryOperation:

                switch (unaryOperation.OperatorType)
                {
                    case LjsAstUnaryOperationType.Plus:
                        // just skip, because unary plus does nothing
                        ProcessNode(unaryOperation.Operand, functionData);
                        break;
                    
                    case LjsAstUnaryOperationType.Minus:
                        ProcessNode(unaryOperation.Operand, functionData);
                        instructions.Add(new LjsInstruction(LjsInstructionCode.Minus));
                        break;
                    case LjsAstUnaryOperationType.LogicalNot:
                        ProcessNode(unaryOperation.Operand, functionData);
                        instructions.Add(new LjsInstruction(LjsInstructionCode.Not));
                        break;
                    
                    case LjsAstUnaryOperationType.BitNot:
                        ProcessNode(unaryOperation.Operand, functionData);
                        instructions.Add(new LjsInstruction(LjsInstructionCode.BitNot));
                        break;
                    
                    default:
                        throw new LjsCompilerError(
                            $"unsupported unary operator type {unaryOperation.OperatorType}");
                }
                
                break;
            
            case LjsAstVariableDeclaration variableDeclaration:

                var varIndex = functionData.AddLocal(variableDeclaration.Name);

                if (variableDeclaration.Value != LjsAstEmptyNode.Instance)
                {
                    ProcessNode(variableDeclaration.Value, functionData);
                    
                    instructions.Add(new LjsInstruction(LjsInstructionCode.VarInit, varIndex));
                }
                
                break;
            
            case LjsAstGetVar getVar:
                AddVarLoadInstruction(functionData, getVar);
                break;
            
            case LjsAstSetVar setVar:
                AddVarStoreInstructions(functionData, setVar);
                break;
            
            case LjsAstIncrementVar incrementVar:
                AddVarIncrementInstruction(functionData, incrementVar);
                break;
            
            case LjsAstIfBlock ifBlock:
                
                ProcessNode(ifBlock.MainBlock.Condition, functionData);
                
                // indices of empty placeholder instructions to be replaced with actual jump instructions 
                var ifEndIndices = LjsCompileUtils.GetTemporaryIntList();

                var ifConditionalJumpIndex = instructions.Count;
                
                // if false jump to next condition or to the else block or to the end
                instructions.Add(default);
                
                ProcessNode(ifBlock.MainBlock.Expression, functionData, 
                    jumpToTheStartPlaceholdersIndices, jumpToTheEndPlaceholdersIndices);
                
                ifEndIndices.Add(instructions.Count);
                instructions.Add(default);

                if (ifBlock.ConditionalAlternatives.Count != 0)
                {
                    foreach (var alternative in ifBlock.ConditionalAlternatives)
                    {
                        // set previous jump instruction
                        instructions.SetAt(new LjsInstruction(
                            LjsInstructionCode.JumpIfFalse, instructions.Count), 
                            ifConditionalJumpIndex);
                        
                        ProcessNode(alternative.Condition, functionData);
                        
                        ifConditionalJumpIndex = instructions.Count;
                        instructions.Add(default);
                        
                        ProcessNode(alternative.Expression, functionData,
                            jumpToTheStartPlaceholdersIndices, jumpToTheEndPlaceholdersIndices);
                        
                        ifEndIndices.Add(instructions.Count);
                        instructions.Add(default);
                    }
                }

                if (ifBlock.ElseBlock != null)
                {
                    instructions.SetAt(new LjsInstruction(
                        LjsInstructionCode.JumpIfFalse, instructions.Count), 
                        ifConditionalJumpIndex);

                    ifConditionalJumpIndex = -1;
                    
                    ProcessNode(ifBlock.ElseBlock, functionData,
                        jumpToTheStartPlaceholdersIndices, jumpToTheEndPlaceholdersIndices);
                }
                
                var ifBlockEndIndex = instructions.Count;

                if (ifConditionalJumpIndex != -1)
                {
                    instructions.SetAt(new LjsInstruction(
                        LjsInstructionCode.JumpIfFalse, ifBlockEndIndex), 
                        ifConditionalJumpIndex);
                }

                foreach (var i in ifEndIndices)
                {
                    instructions.SetAt(new LjsInstruction(
                        LjsInstructionCode.Jump, ifBlockEndIndex), i);
                }
                
                LjsCompileUtils.ReleaseTemporaryIntList(ifEndIndices);
                
                break;
            
            case LjsAstForLoop forLoop:
                
                ProcessNode(forLoop.InitExpression, functionData);
                
                var loopStartIndex = instructions.Count;
                var loopConditionalJumpIndex = -1;
                
                if (forLoop.Condition != LjsAstEmptyNode.Instance)
                {
                    ProcessNode(forLoop.Condition, functionData);
                
                    loopConditionalJumpIndex = instructions.Count;
                    instructions.Add(default);
                }
                
                // for break statements inside
                var loopEndIndices = LjsCompileUtils.GetTemporaryIntList();
                var loopContinueIndices = LjsCompileUtils.GetTemporaryIntList();

                ProcessNode(forLoop.Body, functionData,
                    loopContinueIndices, loopEndIndices);

                var loopIteratorIndex = instructions.Count;
                
                ProcessNode(forLoop.IterationExpression, functionData);
                
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.Jump, loopStartIndex));

                var loopEndIndex = instructions.Count;

                if (loopConditionalJumpIndex != -1)
                {
                    instructions.SetAt(new LjsInstruction(
                            LjsInstructionCode.JumpIfFalse, loopEndIndex), 
                        loopConditionalJumpIndex);
                }
                
                foreach (var i in loopContinueIndices)
                {
                    instructions.SetAt(new LjsInstruction(
                        LjsInstructionCode.Jump, loopIteratorIndex), i);
                }
                
                foreach (var i in loopEndIndices)
                {
                    instructions.SetAt(new LjsInstruction(
                        LjsInstructionCode.Jump, loopEndIndex), i);
                }
                
                LjsCompileUtils.ReleaseTemporaryIntList(loopEndIndices);
                LjsCompileUtils.ReleaseTemporaryIntList(loopContinueIndices);
                break;
            
            case LjsAstWhileLoop whileLoop:
                
                var whileStartIndex = instructions.Count;
                
                ProcessNode(whileLoop.Condition, functionData);

                var whileConditionalJumpIndex = instructions.Count;
                instructions.Add(default);
                
                // for break statements inside
                var whileEndIndices = LjsCompileUtils.GetTemporaryIntList();
                var whileContinueIndices = LjsCompileUtils.GetTemporaryIntList();
                
                ProcessNode(whileLoop.Body, functionData, 
                    whileContinueIndices, whileEndIndices);
                
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.Jump, whileStartIndex));

                var whileEndIndex = instructions.Count;

                instructions.SetAt(new LjsInstruction(
                    LjsInstructionCode.JumpIfFalse, whileEndIndex), 
                    whileConditionalJumpIndex);

                foreach (var i in whileContinueIndices)
                {
                    instructions.SetAt(new LjsInstruction(
                        LjsInstructionCode.Jump, whileStartIndex), i);
                }
                
                foreach (var i in whileEndIndices)
                {
                    instructions.SetAt(new LjsInstruction(
                        LjsInstructionCode.Jump, whileEndIndex), i);
                }
                
                LjsCompileUtils.ReleaseTemporaryIntList(whileEndIndices);
                LjsCompileUtils.ReleaseTemporaryIntList(whileContinueIndices);
                break;
            
            case LjsAstSequence sequence:

                foreach (var n in 
                         sequence.ChildNodes.OfType<LjsAstNamedFunctionDeclaration>())
                {
                    functionData.CreateNamedFunction(n);
                }
                
                foreach (var n in sequence.ChildNodes)
                {
                    ProcessNode(n, functionData, 
                        jumpToTheStartPlaceholdersIndices, jumpToTheEndPlaceholdersIndices);
                }
                
                break;
            
            case LjsAstGetNamedProperty astGetNamedProperty:

                ProcessNode(astGetNamedProperty.PropertySource, functionData);
                
                instructions.Add(GetStringConstInstruction(astGetNamedProperty.PropertyName));

                instructions.Add(new LjsInstruction(LjsInstructionCode.GetProp));
                break;
            
            case LjsAstGetProperty astGetProperty:

                ProcessNode(astGetProperty.PropertySource, functionData);
                ProcessNode(astGetProperty.PropertyName, functionData);

                instructions.Add(new LjsInstruction(LjsInstructionCode.GetProp));
                break;
            
            case LjsAstSetNamedProperty astSetNamedProperty:
                
                if (astSetNamedProperty.AssignMode == LjsAstAssignMode.Normal)
                {
                    ProcessNode(astSetNamedProperty.AssignmentExpression, functionData);
                }
                else
                {
                    ProcessNode(astSetNamedProperty.PropertySource, functionData);
                    instructions.Add(GetStringConstInstruction(astSetNamedProperty.PropertyName));
                    instructions.Add(new LjsInstruction(LjsInstructionCode.GetProp));
                    
                    ProcessNode(astSetNamedProperty.AssignmentExpression, functionData);
                    
                    instructions.Add(new LjsInstruction(
                        LjsCompileUtils.GetComplexAssignmentOpCode(astSetNamedProperty.AssignMode)));
                }
                
                ProcessNode(astSetNamedProperty.PropertySource, functionData);
                instructions.Add(GetStringConstInstruction(astSetNamedProperty.PropertyName));
                
                instructions.Add(new LjsInstruction(LjsInstructionCode.SetProp));
                
                break;
            
            case LjsAstSetProperty astSetProperty:

                if (astSetProperty.AssignMode == LjsAstAssignMode.Normal)
                {
                    ProcessNode(astSetProperty.AssignmentExpression, functionData);
                }
                else
                {
                    ProcessNode(astSetProperty.PropertySource, functionData);
                    ProcessNode(astSetProperty.PropertyName, functionData);
                    instructions.Add(new LjsInstruction(LjsInstructionCode.GetProp));
                    
                    ProcessNode(astSetProperty.AssignmentExpression, functionData);
                    
                    instructions.Add(new LjsInstruction(
                        LjsCompileUtils.GetComplexAssignmentOpCode(astSetProperty.AssignMode)));
                }
                
                ProcessNode(astSetProperty.PropertySource, functionData);
                ProcessNode(astSetProperty.PropertyName, functionData);
                
                instructions.Add(new LjsInstruction(LjsInstructionCode.SetProp));
                
                break;
            
            case LjsAstObjectLiteral objectLiteral:
                
                foreach (var prop in objectLiteral.ChildNodes)
                {
                    ProcessNode(prop.Value, functionData);
                    instructions.Add(GetStringConstInstruction(prop.Name));
                }
                
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.NewDictionary, objectLiteral.Count));
                
                break;
            
            case LjsAstArrayLiteral arrayLiteral:
                
                foreach (var e in arrayLiteral.ChildNodes)
                {
                    ProcessNode(e, functionData);
                }
                
                instructions.Add(new LjsInstruction(LjsInstructionCode.NewArray, arrayLiteral.Count));
                
                break;
            
            case LjsAstGetThis _:
                
                instructions.Add(new LjsInstruction(LjsInstructionCode.GetThis));
                
                break;
            
            
            default:
                throw new LjsCompilerError($"unsupported ast node {node.GetType().Name}");
        }
    }

    private LjsInstruction GetStringConstInstruction(string s) => new(
        LjsInstructionCode.ConstString,
        _constants.AddStringConstant(s));
    
    
    /////////// vars init/load/store
    
    private void AddVarLoadInstruction(LjsCompilerFunctionData data, LjsAstGetVar getVar)
    {
        var instructions = data.Instructions;

        if (data.HasLocal(getVar.VarName))
        {
            instructions.Add(new LjsInstruction(
                LjsInstructionCode.VarLoad, data.GetLocal(getVar.VarName)));
        }
        else if (data.HasFunctionWithName(getVar.VarName))
        {
            instructions.Add(new LjsInstruction(LjsInstructionCode.FuncRef, data.GetFunctionIndex(getVar.VarName)));
        }
        else if (data.HasLocalInHierarchy(getVar.VarName))
        {
            var (varIndex, functionIndex) = data.GetLocalInHierarchy(getVar.VarName);
            var instructionArg = LjsRuntimeUtils.CombineTwoShorts(varIndex, functionIndex);
            
            instructions.Add(new LjsInstruction(LjsInstructionCode.ParentVarLoad, instructionArg));
            
        }
        else
        {
            instructions.Add(new LjsInstruction(
                LjsInstructionCode.ExtLoad, _constants.AddStringConstant(getVar.VarName)));
        }
    }

    private LjsInstruction CreateVarLoadInstruction(string varName, LjsCompilerFunctionData data)
    {
        var localVarIndex = data.GetLocal(varName);
        var isLocal = localVarIndex != -1;

        if (isLocal) 
            return new LjsInstruction(LjsInstructionCode.VarLoad, localVarIndex);

        if (data.HasLocalInHierarchy(varName))
        {
            var (varIndex, functionIndex) = data.GetLocalInHierarchy(varName);
            var instructionArg = LjsRuntimeUtils.CombineTwoShorts(varIndex, functionIndex);
            
            return new LjsInstruction(LjsInstructionCode.ParentVarLoad, instructionArg);
        }
        
        return new LjsInstruction(
                LjsInstructionCode.ExtLoad, _constants.AddStringConstant(varName));
    }
    
    private LjsInstruction CreateVarStoreInstruction(string varName, LjsCompilerFunctionData data)
    {
        var localVarIndex = data.GetLocal(varName);
        var isLocal = localVarIndex != -1;
        
        if (isLocal) 
            return new LjsInstruction(LjsInstructionCode.VarStore, localVarIndex);
        
        if (data.HasLocalInHierarchy(varName))
        {
            var (varIndex, functionIndex) = data.GetLocalInHierarchy(varName);
            var instructionArg = LjsRuntimeUtils.CombineTwoShorts(varIndex, functionIndex);
            
            return new LjsInstruction(LjsInstructionCode.ParentVarStore, instructionArg);
        }
        
        return new LjsInstruction(
                LjsInstructionCode.ExtStore, _constants.AddStringConstant(varName));
    }
    
    private LjsInstruction CreateVarInitInstruction(string varName, LjsCompilerFunctionData data)
    {
        var localVarIndex = data.GetLocal(varName);
        var isLocal = localVarIndex != -1;
        
        if (isLocal) 
            return new LjsInstruction(LjsInstructionCode.VarInit, localVarIndex);
        
        if (data.HasLocalInHierarchy(varName))
        {
            var (varIndex, functionIndex) = data.GetLocalInHierarchy(varName);
            var instructionArg = LjsRuntimeUtils.CombineTwoShorts(varIndex, functionIndex);
            
            return new LjsInstruction(LjsInstructionCode.ParentVarInit, instructionArg);
        }
        
        return new LjsInstruction(
                LjsInstructionCode.ExtStore, _constants.AddStringConstant(varName));
    }
    
    private void AddVarIncrementInstruction(LjsCompilerFunctionData data, LjsAstIncrementVar incrementVar)
    {
        var instructions = data.Instructions;

        var varLoadInstruction = CreateVarLoadInstruction(incrementVar.VarName, data);
                
        if (incrementVar.Order == LjsAstIncrementOrder.Postfix)
        {
            // we leave old var value on stack
            instructions.Add(varLoadInstruction);
        }
        
        instructions.Add(varLoadInstruction);
        instructions.Add(new LjsInstruction(LjsInstructionCode.ConstInt, 1));
        instructions.Add(new LjsInstruction(LjsCompileUtils.GetIncrementOpCode(incrementVar.Sign)));
                
        switch (incrementVar.Order)
        {
            case LjsAstIncrementOrder.Prefix:
                instructions.Add(CreateVarStoreInstruction(incrementVar.VarName, data));
                break;
                    
            case LjsAstIncrementOrder.Postfix:
                instructions.Add(CreateVarInitInstruction(incrementVar.VarName, data));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void AddVarStoreInstructions(LjsCompilerFunctionData data, LjsAstSetVar setVar)
    {
        var instructions = data.Instructions;
        
        var localVarIndex = data.GetLocal(setVar.VarName);
        var isLocal = localVarIndex != -1;
                
        if (!isLocal && data.HasFunctionWithName(setVar.VarName))
        {
            throw new LjsCompilerError($"named function assign {setVar.VarName}");
        }
                
        if (setVar.AssignMode == LjsAstAssignMode.Normal)
        {
            ProcessNode(setVar.Expression, data);
        }
        else
        {
            instructions.Add(CreateVarLoadInstruction(setVar.VarName, data));
                    
            ProcessNode(setVar.Expression, data);
                    
            instructions.Add(new LjsInstruction(
                LjsCompileUtils.GetComplexAssignmentOpCode(setVar.AssignMode)));
        }
        
        instructions.Add(CreateVarStoreInstruction(setVar.VarName, data));
    }
    
    
}