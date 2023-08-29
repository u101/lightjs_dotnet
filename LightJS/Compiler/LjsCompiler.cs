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
            new LjsInstruction(LjsInstructionCode.Halt));
        
        return _program;
    }
    
    private class Context
    {
        public LjsInstructionsList Instructions { get; }
        public Context? ParentContext { get; }

        public Context(LjsInstructionsList instructions)
        {
            Instructions = instructions;
        }
        
        public Context(LjsInstructionsList instructions, Context parentContext)
        {
            Instructions = instructions;
            ParentContext = parentContext;
        }
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
            f.InstructionsList.LastInstruction.Code != LjsInstructionCode.Return)
        {
            f.InstructionsList.AddInstruction(new LjsInstruction(LjsInstructionCode.ConstUndef));
            f.InstructionsList.AddInstruction(new LjsInstruction(LjsInstructionCode.Return));
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
        LjsInstructionsList instructions,
        int startIndex = -1, 
        ICollection<int>? jumpToTheEndPlaceholdersIndices = null)
    {
        
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
                    ProcessNode(n, instructions);
                }
                
                instructions.AddInstruction(new LjsInstruction(
                    LjsInstructionCode.FuncCall, (short) specifiedArgumentsCount));
                
                break;
            
            case LjsAstReturn astReturn:

                if (astReturn.ReturnValue != LjsAstEmptyNode.Instance)
                {
                    ProcessNode(astReturn.ReturnValue, instructions);
                }
                else
                {
                    instructions.AddInstruction(new LjsInstruction(LjsInstructionCode.ConstUndef));
                }
                
                instructions.AddInstruction(new LjsInstruction(LjsInstructionCode.Return));
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
                instructions.AddInstruction(new LjsInstruction(LjsInstructionCode.Jump, (short) startIndex));
                break;
            
            case LjsAstLiteral<int> lit:
                instructions.AddInstruction(new LjsInstruction(
                    LjsInstructionCode.ConstInt, 
                    _program.AddIntegerConstant(lit.Value)));
                break;
            
            case LjsAstLiteral<double> lit:
                instructions.AddInstruction(new LjsInstruction(
                    LjsInstructionCode.ConstDouble, 
                    _program.AddDoubleConstant(lit.Value)));
                break;
            
            case LjsAstLiteral<string> lit:
                instructions.AddInstruction(new LjsInstruction(
                    LjsInstructionCode.ConstString, 
                    _program.AddStringConstant(lit.Value)));
                break;
            
            case LjsAstNull _:
                instructions.AddInstruction(
                    new LjsInstruction(LjsInstructionCode.ConstNull));
                break;
            case LjsAstUndefined _:
                instructions.AddInstruction(
                    new LjsInstruction(LjsInstructionCode.ConstUndef));
                break;
            
            case LjsAstLiteral<bool> lit:
                instructions.AddInstruction(new LjsInstruction(
                    lit.Value ? LjsInstructionCode.ConstTrue : LjsInstructionCode.ConstFalse));
                break;
            
            case LjsAstBinaryOperation binaryOperation:
                
                ProcessNode(binaryOperation.LeftOperand, instructions);
                ProcessNode(binaryOperation.RightOperand, instructions);

                instructions.AddInstruction(new LjsInstruction(LjsCompileUtils.GetBinaryOpCode(binaryOperation.OperatorType)));
                
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
                        instructions.AddInstruction(new LjsInstruction(LjsInstructionCode.Minus));
                        break;
                    case LjsAstUnaryOperationType.LogicalNot:
                        ProcessNode(unaryOperation.Operand, instructions);
                        instructions.AddInstruction(new LjsInstruction(LjsInstructionCode.Not));
                        break;
                    
                    case LjsAstUnaryOperationType.BitNot:
                        ProcessNode(unaryOperation.Operand, instructions);
                        instructions.AddInstruction(new LjsInstruction(LjsInstructionCode.BitNot));
                        break;
                    
                    default:
                        throw new LjsCompilerError(
                            $"unsupported unary operator type {unaryOperation.OperatorType}");
                }
                
                break;
            
            case LjsAstVariableDeclaration variableDeclaration:

                var varNameIndex = _program.AddStringConstant(variableDeclaration.Name);

                instructions.AddInstruction(new LjsInstruction(LjsInstructionCode.VarDef, varNameIndex));

                if (variableDeclaration.Value != LjsAstEmptyNode.Instance)
                {
                    ProcessNode(variableDeclaration.Value, instructions);
                    
                    instructions.AddInstruction(new LjsInstruction(LjsInstructionCode.VarInit, varNameIndex));
                }
                
                break;
            
            case LjsAstGetVar getVar:
                instructions.AddInstruction(new LjsInstruction(
                    LjsInstructionCode.VarLoad, _program.AddStringConstant(getVar.VarName)));
                break;
            
            case LjsAstSetVar setVar:

                if (setVar.AssignMode == LjsAstAssignMode.Normal)
                {
                    ProcessNode(setVar.Expression, instructions);
                }
                else
                {
                    instructions.AddInstruction(new LjsInstruction(
                        LjsInstructionCode.VarLoad, _program.AddStringConstant(setVar.VarName)));
                    
                    ProcessNode(setVar.Expression, instructions);
                    
                    instructions.AddInstruction(new LjsInstruction(
                        LjsCompileUtils.GetComplexAssignmentOpCode(setVar.AssignMode)));
                }
                
                instructions.AddInstruction(new LjsInstruction(
                    LjsInstructionCode.VarStore, _program.AddStringConstant(setVar.VarName)));
                
                break;
            
            case LjsAstIncrementVar incrementVar:

                if (incrementVar.Order == LjsAstIncrementOrder.Postfix)
                {
                    // we leave old var value on stack
                    instructions.AddInstruction(new LjsInstruction(
                        LjsInstructionCode.VarLoad, _program.AddStringConstant(incrementVar.VarName)));
                }
                
                instructions.AddInstruction(new LjsInstruction(
                    LjsInstructionCode.VarLoad, _program.AddStringConstant(incrementVar.VarName)));
                instructions.AddInstruction(new LjsInstruction(
                    LjsInstructionCode.ConstInt, _program.AddIntegerConstant(1)));
                instructions.AddInstruction(new LjsInstruction(LjsCompileUtils.GetIncrementOpCode(incrementVar.Sign)));
                
                switch (incrementVar.Order)
                {
                    case LjsAstIncrementOrder.Prefix:
                        instructions.AddInstruction(new LjsInstruction(
                            LjsInstructionCode.VarStore, _program.AddStringConstant(incrementVar.VarName)));
                        break;
                    
                    case LjsAstIncrementOrder.Postfix:
                        instructions.AddInstruction(new LjsInstruction(
                            LjsInstructionCode.VarInit, _program.AddStringConstant(incrementVar.VarName)));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                break;
            
            case LjsAstIfBlock ifBlock:
                
                ProcessNode(ifBlock.MainBlock.Condition, instructions);
                
                // indices of empty placeholder instructions to be replaced with actual jump instructions 
                var ifEndIndices = LjsCompileUtils.GetTemporaryIntList();

                var ifConditionalJumpIndex = instructions.InstructionsCount;
                
                // if false jump to next condition or to the else block or to the end
                instructions.AddInstruction(default);
                
                ProcessNode(ifBlock.MainBlock.Expression, instructions, startIndex, jumpToTheEndPlaceholdersIndices);
                
                ifEndIndices.Add(instructions.InstructionsCount);
                instructions.AddInstruction(default);

                if (ifBlock.ConditionalAlternatives.Count != 0)
                {
                    foreach (var alternative in ifBlock.ConditionalAlternatives)
                    {
                        // set previous jump instruction
                        // TODO instructions.InstructionsCount can be more then short.MaxValue
                        instructions.SetInstructionAt(new LjsInstruction(
                            LjsInstructionCode.JumpIfFalse, (short) instructions.InstructionsCount), 
                            ifConditionalJumpIndex);
                        
                        ProcessNode(alternative.Condition, instructions);
                        
                        ifConditionalJumpIndex = instructions.InstructionsCount;
                        instructions.AddInstruction(default);
                        
                        ProcessNode(alternative.Expression, instructions, startIndex, jumpToTheEndPlaceholdersIndices);
                        
                        ifEndIndices.Add(instructions.InstructionsCount);
                        instructions.AddInstruction(default);
                    }
                }

                if (ifBlock.ElseBlock != null)
                {
                    // TODO instructions.InstructionsCount can be more then short.MaxValue
                    instructions.SetInstructionAt(new LjsInstruction(
                        LjsInstructionCode.JumpIfFalse, (short) instructions.InstructionsCount), 
                        ifConditionalJumpIndex);

                    ifConditionalJumpIndex = -1;
                    
                    ProcessNode(ifBlock.ElseBlock, instructions, startIndex, jumpToTheEndPlaceholdersIndices);
                }

                // TODO instructions.InstructionsCount can be more then short.MaxValue
                var ifBlockEndIndex = (short) instructions.InstructionsCount;

                if (ifConditionalJumpIndex != -1)
                {
                    instructions.SetInstructionAt(new LjsInstruction(
                        LjsInstructionCode.JumpIfFalse, ifBlockEndIndex), 
                        ifConditionalJumpIndex);
                }

                foreach (var i in ifEndIndices)
                {
                    instructions.SetInstructionAt(new LjsInstruction(
                        LjsInstructionCode.Jump, ifBlockEndIndex), i);
                }
                
                LjsCompileUtils.ReleaseTemporaryIntList(ifEndIndices);
                
                break;
            
            case LjsAstWhileLoop whileLoop:
                
                var whileStartIndex = instructions.InstructionsCount;
                
                ProcessNode(whileLoop.Condition, instructions);

                var whileConditionalJumpIndex = instructions.InstructionsCount;
                instructions.AddInstruction(default);
                
                // for break statements inside
                var whileEndIndices = LjsCompileUtils.GetTemporaryIntList();
                
                ProcessNode(whileLoop.Body, instructions, whileStartIndex, whileEndIndices);
                
                instructions.AddInstruction(new LjsInstruction(
                    LjsInstructionCode.Jump, (short) whileStartIndex));

                var whileEndIndex = instructions.InstructionsCount;

                instructions.SetInstructionAt(new LjsInstruction(
                    LjsInstructionCode.JumpIfFalse, (short) whileEndIndex), 
                    whileConditionalJumpIndex);
                
                foreach (var i in whileEndIndices)
                {
                    instructions.SetInstructionAt(new LjsInstruction(
                        LjsInstructionCode.Jump, (short) whileEndIndex), i);
                }
                
                LjsCompileUtils.ReleaseTemporaryIntList(whileEndIndices);
                break;
            
            case LjsAstSequence sequence:

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