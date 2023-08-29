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
    private readonly Dictionary<string, int> _namedFunctionsMap = new();
    private readonly List<LjsFunctionData> _functionsList = new();

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
        var mainFunc = new LjsFunctionData();
        
        _functionsList.Add(mainFunc);
        
        ProcessNode(_astModel.RootNode, mainFunc.InstructionsList);
        
        mainFunc.InstructionsList.Add(
            new LjsInstruction(LjsInstructionCode.Halt));
        
        return new LjsProgram(
            _constants, _functionsList, _namedFunctionsMap);
    }

    private void ProcessFunction(LjsFunctionData f, LjsAstFunctionDeclaration functionDeclaration)
    {
        
        foreach (var p in functionDeclaration.Parameters)
        {
            var defaultValue = GetFunctionParameterDefaultValue(p.DefaultValue);
            f.Args.Add(new LjsFunctionArg(p.Name, defaultValue));
        }
        
        ProcessNode(functionDeclaration.FunctionBody, f.InstructionsList);

        if (f.InstructionsList.Count == 0 ||
            f.InstructionsList.LastInstruction.Code != LjsInstructionCode.Return)
        {
            f.InstructionsList.Add(new LjsInstruction(LjsInstructionCode.ConstUndef));
            f.InstructionsList.Add(new LjsInstruction(LjsInstructionCode.Return));
        }
    }

    private static LjsObject GetFunctionParameterDefaultValue(ILjsAstNode node) => node switch
    {
        LjsAstNull _ => LjsObject.Null,
        LjsAstUndefined _ => LjsObject.Undefined,
        LjsAstLiteral<int> i => new LjsValue<int>(i.Value),
        LjsAstLiteral<double> i => new LjsValue<double>(i.Value),
        LjsAstLiteral<string> i => new LjsValue<string>(i.Value),
        LjsAstLiteral<bool> i => i.Value ? LjsValue.True : LjsValue.False,
        _ => LjsObject.Undefined
    };

    private LjsFunctionData CreateNamedFunction(LjsAstNamedFunctionDeclaration namedFunctionDeclaration)
    {
        var namedFunc = new LjsFunctionData();
        var namedFunctionIndex = _functionsList.Count;
                
        _functionsList.Add(namedFunc);
        _namedFunctionsMap[namedFunctionDeclaration.Name] = namedFunctionIndex;
        return namedFunc;
    }

    private LjsFunctionData GetNamedFunction(LjsAstNamedFunctionDeclaration namedFunctionDeclaration) =>
        _functionsList[_namedFunctionsMap[namedFunctionDeclaration.Name]];
    

    private void ProcessNode(
        ILjsAstNode node, 
        LjsInstructionsList instructions,
        int startIndex = -1, 
        ICollection<int>? jumpToTheEndPlaceholdersIndices = null)
    {
        
        switch (node)
        {
            case LjsAstAnonymousFunctionDeclaration funcDeclaration:
                
                var anonFunc = new LjsFunctionData();
                var anonFunctionIndex = _functionsList.Count;
                
                _functionsList.Add(anonFunc);
                
                ProcessFunction(anonFunc, funcDeclaration);
                
                instructions.Add(new LjsInstruction(LjsInstructionCode.FuncRef, anonFunctionIndex));
                
                break;
            
            case LjsAstNamedFunctionDeclaration namedFunctionDeclaration:
                var namedFunc = _namedFunctionsMap.ContainsKey(namedFunctionDeclaration.Name)
                        ? GetNamedFunction(namedFunctionDeclaration) : CreateNamedFunction(namedFunctionDeclaration);

                if (namedFunc != GetNamedFunction(namedFunctionDeclaration))
                {
                    throw new LjsCompilerError($"duplicate function names {namedFunctionDeclaration.Name}");
                }
                
                ProcessFunction(namedFunc, namedFunctionDeclaration);
                break;
            
            case LjsAstFunctionCall functionCall:

                var specifiedArgumentsCount = functionCall.Arguments.Count;
                
                foreach (var n in functionCall.Arguments)
                {
                    ProcessNode(n, instructions);
                }
                
                ProcessNode(functionCall.FunctionGetter, instructions);
                
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.FuncCall, specifiedArgumentsCount));
                
                break;
            
            case LjsAstReturn astReturn:

                if (astReturn.ReturnValue != LjsAstEmptyNode.Instance)
                {
                    ProcessNode(astReturn.ReturnValue, instructions);
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
                if (startIndex == -1)
                    throw new LjsCompilerError("unexpected continue statement");
                instructions.Add(new LjsInstruction(LjsInstructionCode.Jump, startIndex));
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
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.ConstString, 
                    _constants.AddStringConstant(lit.Value)));
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
                
                ProcessNode(binaryOperation.LeftOperand, instructions);
                ProcessNode(binaryOperation.RightOperand, instructions);

                instructions.Add(new LjsInstruction(LjsCompileUtils.GetBinaryOpCode(binaryOperation.OperatorType)));
                
                break;
            
            case LjsAstUnaryOperation unaryOperation:

                switch (unaryOperation.OperatorType)
                {
                    case LjsAstUnaryOperationType.Plus:
                        // just skip, because unary plus does nothing
                        ProcessNode(unaryOperation.Operand, instructions);
                        break;
                    
                    case LjsAstUnaryOperationType.Minus:
                        ProcessNode(unaryOperation.Operand, instructions);
                        instructions.Add(new LjsInstruction(LjsInstructionCode.Minus));
                        break;
                    case LjsAstUnaryOperationType.LogicalNot:
                        ProcessNode(unaryOperation.Operand, instructions);
                        instructions.Add(new LjsInstruction(LjsInstructionCode.Not));
                        break;
                    
                    case LjsAstUnaryOperationType.BitNot:
                        ProcessNode(unaryOperation.Operand, instructions);
                        instructions.Add(new LjsInstruction(LjsInstructionCode.BitNot));
                        break;
                    
                    default:
                        throw new LjsCompilerError(
                            $"unsupported unary operator type {unaryOperation.OperatorType}");
                }
                
                break;
            
            case LjsAstVariableDeclaration variableDeclaration:

                var varNameIndex = _constants.AddStringConstant(variableDeclaration.Name);

                instructions.Add(new LjsInstruction(LjsInstructionCode.VarDef, varNameIndex));

                if (variableDeclaration.Value != LjsAstEmptyNode.Instance)
                {
                    ProcessNode(variableDeclaration.Value, instructions);
                    
                    instructions.Add(new LjsInstruction(LjsInstructionCode.VarInit, varNameIndex));
                }
                
                break;
            
            case LjsAstGetVar getVar:

                if (_namedFunctionsMap.ContainsKey(getVar.VarName))
                {
                    instructions.Add(new LjsInstruction(LjsInstructionCode.FuncRef, _namedFunctionsMap[getVar.VarName]));
                }
                else
                {
                    instructions.Add(new LjsInstruction(
                        LjsInstructionCode.VarLoad, _constants.AddStringConstant(getVar.VarName)));
                }
                break;
            
            case LjsAstSetVar setVar:

                if (_namedFunctionsMap.ContainsKey(setVar.VarName))
                {
                    throw new LjsCompilerError($"named function assign {setVar.VarName}");
                }
                
                if (setVar.AssignMode == LjsAstAssignMode.Normal)
                {
                    ProcessNode(setVar.Expression, instructions);
                }
                else
                {
                    instructions.Add(new LjsInstruction(
                        LjsInstructionCode.VarLoad, _constants.AddStringConstant(setVar.VarName)));
                    
                    ProcessNode(setVar.Expression, instructions);
                    
                    instructions.Add(new LjsInstruction(
                        LjsCompileUtils.GetComplexAssignmentOpCode(setVar.AssignMode)));
                }
                
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.VarStore, _constants.AddStringConstant(setVar.VarName)));
                
                break;
            
            case LjsAstIncrementVar incrementVar:

                if (incrementVar.Order == LjsAstIncrementOrder.Postfix)
                {
                    // we leave old var value on stack
                    instructions.Add(new LjsInstruction(
                        LjsInstructionCode.VarLoad, _constants.AddStringConstant(incrementVar.VarName)));
                }
                
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.VarLoad, _constants.AddStringConstant(incrementVar.VarName)));
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.ConstInt, 1));
                instructions.Add(new LjsInstruction(LjsCompileUtils.GetIncrementOpCode(incrementVar.Sign)));
                
                switch (incrementVar.Order)
                {
                    case LjsAstIncrementOrder.Prefix:
                        instructions.Add(new LjsInstruction(
                            LjsInstructionCode.VarStore, _constants.AddStringConstant(incrementVar.VarName)));
                        break;
                    
                    case LjsAstIncrementOrder.Postfix:
                        instructions.Add(new LjsInstruction(
                            LjsInstructionCode.VarInit, _constants.AddStringConstant(incrementVar.VarName)));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                break;
            
            case LjsAstIfBlock ifBlock:
                
                ProcessNode(ifBlock.MainBlock.Condition, instructions);
                
                // indices of empty placeholder instructions to be replaced with actual jump instructions 
                var ifEndIndices = LjsCompileUtils.GetTemporaryIntList();

                var ifConditionalJumpIndex = instructions.Count;
                
                // if false jump to next condition or to the else block or to the end
                instructions.Add(default);
                
                ProcessNode(ifBlock.MainBlock.Expression, instructions, startIndex, jumpToTheEndPlaceholdersIndices);
                
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
                        
                        ProcessNode(alternative.Condition, instructions);
                        
                        ifConditionalJumpIndex = instructions.Count;
                        instructions.Add(default);
                        
                        ProcessNode(alternative.Expression, instructions, startIndex, jumpToTheEndPlaceholdersIndices);
                        
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
                    
                    ProcessNode(ifBlock.ElseBlock, instructions, startIndex, jumpToTheEndPlaceholdersIndices);
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
            
            case LjsAstWhileLoop whileLoop:
                
                var whileStartIndex = instructions.Count;
                
                ProcessNode(whileLoop.Condition, instructions);

                var whileConditionalJumpIndex = instructions.Count;
                instructions.Add(default);
                
                // for break statements inside
                var whileEndIndices = LjsCompileUtils.GetTemporaryIntList();
                
                ProcessNode(whileLoop.Body, instructions, whileStartIndex, whileEndIndices);
                
                instructions.Add(new LjsInstruction(
                    LjsInstructionCode.Jump, whileStartIndex));

                var whileEndIndex = instructions.Count;

                instructions.SetAt(new LjsInstruction(
                    LjsInstructionCode.JumpIfFalse, whileEndIndex), 
                    whileConditionalJumpIndex);
                
                foreach (var i in whileEndIndices)
                {
                    instructions.SetAt(new LjsInstruction(
                        LjsInstructionCode.Jump, whileEndIndex), i);
                }
                
                LjsCompileUtils.ReleaseTemporaryIntList(whileEndIndices);
                break;
            
            case LjsAstSequence sequence:

                foreach (var n in 
                         sequence.ChildNodes.OfType<LjsAstNamedFunctionDeclaration>())
                {
                    CreateNamedFunction(n);
                }
                
                foreach (var n in sequence.ChildNodes)
                {
                    ProcessNode(n, instructions, startIndex, jumpToTheEndPlaceholdersIndices);
                }
                
                break;
            
            
            default:
                throw new LjsCompilerError("unsupported ast node");
        }
        
        
        
    }
    
}