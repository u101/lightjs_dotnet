using LightJS.Ast;
using LightJS.Errors;
using LightJS.Program;
using LightJS.Runtime;
using LightJS.Tokenizer;

namespace LightJS.Compiler;

public class LjsCompiler
{
    private readonly LjsAstModel _astModel;
    private readonly LjsProgram _program = new();

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
        ProcessNode(_astModel.RootNode, _program.InstructionsList);
        
        _program.InstructionsList.AddInstruction(
            new LjsInstruction(LjsInstructionCodes.Halt));
        
        return _program;
    }

    private LjsFunction CreateFunction(LjsAstFunctionDeclaration functionDeclaration)
    {
        var f = new LjsFunction();
        
        foreach (var p in functionDeclaration.Parameters)
        {
            var defaultValue = GetFunctionParameterDefaultValue(p.DefaultValue);
            f.Args.Add(new LjsFunctionArg(p.Name, defaultValue));
        }
        
        ProcessNode(functionDeclaration.FunctionBody, f.InstructionsList);

        if (f.InstructionsList.InstructionsCount == 0 ||
            f.InstructionsList.LastInstruction.Code != LjsInstructionCodes.Return)
        {
            f.InstructionsList.AddInstruction(new LjsInstruction(LjsInstructionCodes.ConstUndef));
            f.InstructionsList.AddInstruction(new LjsInstruction(LjsInstructionCodes.Return));
        }

        return f;
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

    private void ProcessNode(
        ILjsAstNode node, 
        LjsInstructionsList instructionsList,
        int startIndex = -1, 
        ICollection<int>? jumpToTheEndPlaceholdersIndices = null)
    {
        var instructions = instructionsList;
        
        switch (node)
        {
            case LjsAstNamedFunctionDeclaration namedFunctionDeclaration:
                var f = CreateFunction(namedFunctionDeclaration);
                _program.AddFunction(namedFunctionDeclaration.Name, f);
                break;
            
            case LjsAstFunctionCall functionCall:

                var specifiedArgumentsCount = functionCall.Arguments.Count;
                
                foreach (var n in functionCall.Arguments)
                {
                    ProcessNode(n, instructionsList);
                }
                
                instructions.AddInstruction(new LjsInstruction(
                    LjsInstructionCodes.FuncCall, (short) specifiedArgumentsCount));
                
                break;
            
            case LjsAstEmptyNode _:
                // do nothing
                break;
            
            case LjsAstBreak _:
                
                if (jumpToTheEndPlaceholdersIndices == null)
                    throw new LjsCompilerError("unexpected break statement");
                
                jumpToTheEndPlaceholdersIndices.Add(instructions.InstructionsCount);
                instructions.AddInstruction(default);
                
                break;
            
            case LjsAstContinue _:
                if (startIndex == -1)
                    throw new LjsCompilerError("unexpected continue statement");
                instructions.AddInstruction(new LjsInstruction(LjsInstructionCodes.Jump, (short) startIndex));
                break;
            
            case LjsAstLiteral<int> lit:
                instructions.AddInstruction(new LjsInstruction(
                    LjsInstructionCodes.ConstInt, 
                    _program.AddIntegerConstant(lit.Value)));
                break;
            
            case LjsAstLiteral<double> lit:
                instructions.AddInstruction(new LjsInstruction(
                    LjsInstructionCodes.ConstDouble, 
                    _program.AddDoubleConstant(lit.Value)));
                break;
            
            case LjsAstLiteral<string> lit:
                instructions.AddInstruction(new LjsInstruction(
                    LjsInstructionCodes.ConstString, 
                    _program.AddStringConstant(lit.Value)));
                break;
            
            case LjsAstNull _:
                instructions.AddInstruction(
                    new LjsInstruction(LjsInstructionCodes.ConstNull));
                break;
            case LjsAstUndefined _:
                instructions.AddInstruction(
                    new LjsInstruction(LjsInstructionCodes.ConstUndef));
                break;
            
            case LjsAstLiteral<bool> lit:
                instructions.AddInstruction(new LjsInstruction(
                    lit.Value ? LjsInstructionCodes.ConstTrue : LjsInstructionCodes.ConstFalse));
                break;
            
            case LjsAstBinaryOperation binaryOperation:
                
                ProcessNode(binaryOperation.LeftOperand, instructionsList);
                ProcessNode(binaryOperation.RightOperand, instructionsList);

                instructions.AddInstruction(new LjsInstruction(LjsCompileUtils.GetBinaryOpCode(binaryOperation.OperatorType)));
                
                break;
            
            case LjsAstUnaryOperation unaryOperation:

                switch (unaryOperation.OperatorType)
                {
                    case LjsAstUnaryOperationType.Plus:
                        // just skip, because unary plus does nothing
                        ProcessNode(unaryOperation.Operand, instructionsList);
                        break;
                    
                    case LjsAstUnaryOperationType.Minus:
                        ProcessNode(unaryOperation.Operand, instructionsList);
                        instructions.AddInstruction(new LjsInstruction(LjsInstructionCodes.Minus));
                        break;
                    case LjsAstUnaryOperationType.LogicalNot:
                        ProcessNode(unaryOperation.Operand, instructionsList);
                        instructions.AddInstruction(new LjsInstruction(LjsInstructionCodes.Not));
                        break;
                    
                    case LjsAstUnaryOperationType.BitNot:
                        ProcessNode(unaryOperation.Operand, instructionsList);
                        instructions.AddInstruction(new LjsInstruction(LjsInstructionCodes.BitNot));
                        break;
                    
                    default:
                        throw new LjsCompilerError(
                            $"unsupported unary operator type {unaryOperation.OperatorType}");
                }
                
                break;
            
            case LjsAstVariableDeclaration variableDeclaration:

                var varNameIndex = _program.AddStringConstant(variableDeclaration.Name);

                instructions.AddInstruction(new LjsInstruction(LjsInstructionCodes.VarDef, varNameIndex));

                if (variableDeclaration.Value != LjsAstEmptyNode.Instance)
                {
                    ProcessNode(variableDeclaration.Value, instructionsList);
                    
                    instructions.AddInstruction(new LjsInstruction(LjsInstructionCodes.VarInit, varNameIndex));
                }
                
                break;
            
            case LjsAstGetVar getVar:
                instructions.AddInstruction(new LjsInstruction(
                    LjsInstructionCodes.VarLoad, _program.AddStringConstant(getVar.VarName)));
                break;
            
            case LjsAstSetVar setVar:

                if (setVar.AssignMode == LjsAstAssignMode.Normal)
                {
                    ProcessNode(setVar.Expression, instructionsList);
                }
                else
                {
                    instructions.AddInstruction(new LjsInstruction(
                        LjsInstructionCodes.VarLoad, _program.AddStringConstant(setVar.VarName)));
                    
                    ProcessNode(setVar.Expression, instructionsList);
                    
                    instructions.AddInstruction(new LjsInstruction(
                        LjsCompileUtils.GetComplexAssignmentOpCode(setVar.AssignMode)));
                }
                
                instructions.AddInstruction(new LjsInstruction(
                    LjsInstructionCodes.VarStore, _program.AddStringConstant(setVar.VarName)));
                
                break;
            
            case LjsAstIncrementVar incrementVar:

                if (incrementVar.Order == LjsAstIncrementOrder.Postfix)
                {
                    // we leave old var value on stack
                    instructions.AddInstruction(new LjsInstruction(
                        LjsInstructionCodes.VarLoad, _program.AddStringConstant(incrementVar.VarName)));
                }
                
                instructions.AddInstruction(new LjsInstruction(
                    LjsInstructionCodes.VarLoad, _program.AddStringConstant(incrementVar.VarName)));
                instructions.AddInstruction(new LjsInstruction(
                    LjsInstructionCodes.ConstInt, _program.AddIntegerConstant(1)));
                instructions.AddInstruction(new LjsInstruction(LjsCompileUtils.GetIncrementOpCode(incrementVar.Sign)));
                
                switch (incrementVar.Order)
                {
                    case LjsAstIncrementOrder.Prefix:
                        instructions.AddInstruction(new LjsInstruction(
                            LjsInstructionCodes.VarStore, _program.AddStringConstant(incrementVar.VarName)));
                        break;
                    
                    case LjsAstIncrementOrder.Postfix:
                        instructions.AddInstruction(new LjsInstruction(
                            LjsInstructionCodes.VarInit, _program.AddStringConstant(incrementVar.VarName)));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                break;
            
            case LjsAstIfBlock ifBlock:
                
                ProcessNode(ifBlock.MainBlock.Condition, instructionsList);
                
                // indices of empty placeholder instructions to be replaced with actual jump instructions 
                var ifEndIndices = LjsCompileUtils.GetTemporaryIntList();

                var ifConditionalJumpIndex = instructions.InstructionsCount;
                
                // if false jump to next condition or to the else block or to the end
                instructions.AddInstruction(default);
                
                ProcessNode(ifBlock.MainBlock.Expression, instructionsList, startIndex, jumpToTheEndPlaceholdersIndices);
                
                ifEndIndices.Add(instructions.InstructionsCount);
                instructions.AddInstruction(default);

                if (ifBlock.ConditionalAlternatives.Count != 0)
                {
                    foreach (var alternative in ifBlock.ConditionalAlternatives)
                    {
                        // set previous jump instruction
                        // TODO instructions.InstructionsCount can be more then short.MaxValue
                        instructions.SetInstructionAt(new LjsInstruction(
                            LjsInstructionCodes.JumpIfFalse, (short) instructions.InstructionsCount), 
                            ifConditionalJumpIndex);
                        
                        ProcessNode(alternative.Condition, instructionsList);
                        
                        ifConditionalJumpIndex = instructions.InstructionsCount;
                        instructions.AddInstruction(default);
                        
                        ProcessNode(alternative.Expression, instructionsList, startIndex, jumpToTheEndPlaceholdersIndices);
                        
                        ifEndIndices.Add(instructions.InstructionsCount);
                        instructions.AddInstruction(default);
                    }
                }

                if (ifBlock.ElseBlock != null)
                {
                    // TODO instructions.InstructionsCount can be more then short.MaxValue
                    instructions.SetInstructionAt(new LjsInstruction(
                        LjsInstructionCodes.JumpIfFalse, (short) instructions.InstructionsCount), 
                        ifConditionalJumpIndex);

                    ifConditionalJumpIndex = -1;
                    
                    ProcessNode(ifBlock.ElseBlock, instructionsList, startIndex, jumpToTheEndPlaceholdersIndices);
                }

                // TODO instructions.InstructionsCount can be more then short.MaxValue
                var ifBlockEndIndex = (short) instructions.InstructionsCount;

                if (ifConditionalJumpIndex != -1)
                {
                    instructions.SetInstructionAt(new LjsInstruction(
                        LjsInstructionCodes.JumpIfFalse, ifBlockEndIndex), 
                        ifConditionalJumpIndex);
                }

                foreach (var i in ifEndIndices)
                {
                    instructions.SetInstructionAt(new LjsInstruction(
                        LjsInstructionCodes.Jump, ifBlockEndIndex), i);
                }
                
                LjsCompileUtils.ReleaseTemporaryIntList(ifEndIndices);
                
                break;
            
            case LjsAstWhileLoop whileLoop:
                
                var whileStartIndex = instructions.InstructionsCount;
                
                ProcessNode(whileLoop.Condition, instructionsList);

                var whileConditionalJumpIndex = instructions.InstructionsCount;
                instructions.AddInstruction(default);
                
                // for break statements inside
                var whileEndIndices = LjsCompileUtils.GetTemporaryIntList();
                
                ProcessNode(whileLoop.Body, instructionsList, whileStartIndex, whileEndIndices);
                
                instructions.AddInstruction(new LjsInstruction(
                    LjsInstructionCodes.Jump, (short) whileStartIndex));

                var whileEndIndex = instructions.InstructionsCount;

                instructions.SetInstructionAt(new LjsInstruction(
                    LjsInstructionCodes.JumpIfFalse, (short) whileEndIndex), 
                    whileConditionalJumpIndex);
                
                foreach (var i in whileEndIndices)
                {
                    instructions.SetInstructionAt(new LjsInstruction(
                        LjsInstructionCodes.Jump, (short) whileEndIndex), i);
                }
                
                LjsCompileUtils.ReleaseTemporaryIntList(whileEndIndices);
                break;
            
            case LjsAstSequence sequence:

                foreach (var n in sequence.ChildNodes)
                {
                    ProcessNode(n, instructionsList, startIndex, jumpToTheEndPlaceholdersIndices);
                }
                
                break;
            
            
            default:
                throw new LjsCompilerError("unsupported ast node");
        }
        
        
        
    }
    
}