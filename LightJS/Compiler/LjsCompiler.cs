using LightJS.Ast;
using LightJS.Errors;
using LightJS.Program;
using LightJS.Tokenizer;

namespace LightJS.Compiler;

public class LjsCompiler
{
    private readonly LjsAstModel _astModel;
    private readonly LjsProgram _program;

    public LjsCompiler(string sourceCodeString)
    {
        if (string.IsNullOrEmpty(sourceCodeString))
        {
            throw new ArgumentException("input string is null or empty");
        }

        var astModelBuilder = new LjsAstBuilder(sourceCodeString);
        
        _astModel = astModelBuilder.Build();
        _program = new LjsProgram();
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
        _program = new LjsProgram();
    }
    
    public LjsCompiler(LjsAstModel astModel)
    {
        _astModel = astModel;
        _program = new LjsProgram();
    }

    public LjsProgram Compile()
    {
        ProcessNode(_astModel.RootNode);
        
        _program.AddInstruction(new LjsInstruction(LjsInstructionCodes.Halt));
        
        return _program;
    }

    private void ProcessNode(ILjsAstNode node)
    {
        switch (node)
        {
            case LjsAstEmptyNode emptyNode:
                // do nothing
                break;
            
            case LjsAstLiteral<int> lit:
                _program.AddInstruction(new LjsInstruction(
                    LjsInstructionCodes.ConstInt, 
                    _program.AddIntegerConstant(lit.Value)));
                break;
            
            case LjsAstLiteral<double> lit:
                _program.AddInstruction(new LjsInstruction(
                    LjsInstructionCodes.ConstDouble, 
                    _program.AddDoubleConstant(lit.Value)));
                break;
            
            case LjsAstLiteral<string> lit:
                _program.AddInstruction(new LjsInstruction(
                    LjsInstructionCodes.ConstString, 
                    _program.AddStringConstant(lit.Value)));
                break;
            
            case LjsAstNull _:
                _program.AddInstruction(
                    new LjsInstruction(LjsInstructionCodes.ConstNull));
                break;
            case LjsAstUndefined _:
                _program.AddInstruction(
                    new LjsInstruction(LjsInstructionCodes.ConstUndef));
                break;
            
            case LjsAstLiteral<bool> lit:
                _program.AddInstruction(new LjsInstruction(
                    lit.Value ? LjsInstructionCodes.ConstTrue : LjsInstructionCodes.ConstFalse));
                break;
            
            case LjsAstBinaryOperation binaryOperation:
                
                ProcessNode(binaryOperation.LeftOperand);
                ProcessNode(binaryOperation.RightOperand);

                _program.AddInstruction(new LjsInstruction(LjsCompileUtils.GetBinaryOpCode(binaryOperation.OperatorType)));
                
                break;
            
            case LjsAstUnaryOperation unaryOperation:

                switch (unaryOperation.OperatorType)
                {
                    case LjsAstUnaryOperationType.Plus:
                        // just skip, because unary plus does nothing
                        ProcessNode(unaryOperation.Operand);
                        break;
                    
                    case LjsAstUnaryOperationType.Minus:
                        ProcessNode(unaryOperation.Operand);
                        _program.AddInstruction(new LjsInstruction(LjsInstructionCodes.Minus));
                        break;
                    case LjsAstUnaryOperationType.LogicalNot:
                        ProcessNode(unaryOperation.Operand);
                        _program.AddInstruction(new LjsInstruction(LjsInstructionCodes.Not));
                        break;
                    
                    case LjsAstUnaryOperationType.BitNot:
                        ProcessNode(unaryOperation.Operand);
                        _program.AddInstruction(new LjsInstruction(LjsInstructionCodes.BitNot));
                        break;
                    
                    // TODO increment, decrement
                    
                    default:
                        throw new NotImplementedException();
                }
                
                break;
            
            case LjsAstVariableDeclaration variableDeclaration:

                var varNameIndex = _program.AddStringConstant(variableDeclaration.Name);

                _program.AddInstruction(new LjsInstruction(LjsInstructionCodes.VarDef, varNameIndex));

                if (variableDeclaration.Value != LjsAstEmptyNode.Instance)
                {
                    ProcessNode(variableDeclaration.Value);
                    
                    _program.AddInstruction(new LjsInstruction(LjsInstructionCodes.VarInit, varNameIndex));
                }
                
                break;
            
            case LjsAstGetVar getVar:
                _program.AddInstruction(new LjsInstruction(
                    LjsInstructionCodes.VarLoad, _program.AddStringConstant(getVar.VarName)));
                break;
            
            case LjsAstSetVar setVar:

                if (setVar.AssignMode == LjsAstAssignMode.Normal)
                {
                    ProcessNode(setVar.Expression);
                }
                else
                {
                    _program.AddInstruction(new LjsInstruction(
                        LjsInstructionCodes.VarLoad, _program.AddStringConstant(setVar.VarName)));
                    
                    ProcessNode(setVar.Expression);
                    
                    _program.AddInstruction(new LjsInstruction(
                        LjsCompileUtils.GetComplexAssignmentOpCode(setVar.AssignMode)));
                }
                
                _program.AddInstruction(new LjsInstruction(
                    LjsInstructionCodes.VarStore, _program.AddStringConstant(setVar.VarName)));
                
                break;
            
            case LjsAstIfBlock ifBlock:
                
                ProcessNode(ifBlock.MainBlock.Condition);
                
                var gotoEndIndices = LjsCompileUtils.GetTemporaryIntList();

                var conditionalJumpIndex = _program.InstructionsCount;
                
                // if false jump to next condition or to the else block or to the end
                _program.AddInstruction(default);
                
                ProcessNode(ifBlock.MainBlock.Expression);
                
                gotoEndIndices.Add(_program.InstructionsCount);
                _program.AddInstruction(default);

                if (ifBlock.ConditionalAlternatives.Count != 0)
                {
                    foreach (var alternative in ifBlock.ConditionalAlternatives)
                    {
                        // set previous jump instruction
                        // TODO _program.InstructionsCount can be more then short.MaxValue
                        _program.SetInstructionAt(new LjsInstruction(
                            LjsInstructionCodes.JumpIfFalse, (short) _program.InstructionsCount), conditionalJumpIndex);
                        
                        ProcessNode(alternative.Condition);
                        
                        conditionalJumpIndex = _program.InstructionsCount;
                        _program.AddInstruction(default);
                        
                        ProcessNode(alternative.Expression);
                        
                        gotoEndIndices.Add(_program.InstructionsCount);
                        _program.AddInstruction(default);
                    }
                }

                if (ifBlock.ElseBlock != null)
                {
                    // TODO _program.InstructionsCount can be more then short.MaxValue
                    _program.SetInstructionAt(new LjsInstruction(
                        LjsInstructionCodes.JumpIfFalse, (short) _program.InstructionsCount), conditionalJumpIndex);

                    conditionalJumpIndex = -1;
                    
                    ProcessNode(ifBlock.ElseBlock);
                }

                // TODO _program.InstructionsCount can be more then short.MaxValue
                var ifBlockEndIndex = (short) _program.InstructionsCount;

                if (conditionalJumpIndex != -1)
                {
                    _program.SetInstructionAt(new LjsInstruction(
                        LjsInstructionCodes.JumpIfFalse, ifBlockEndIndex), conditionalJumpIndex);
                }

                foreach (var i in gotoEndIndices)
                {
                    _program.SetInstructionAt(new LjsInstruction(
                        LjsInstructionCodes.Jump, ifBlockEndIndex), i);
                }
                
                LjsCompileUtils.ReleaseTemporaryIntList(gotoEndIndices);
                
                break;
            
            case LjsAstSequence sequence:

                foreach (var n in sequence.ChildNodes)
                {
                    ProcessNode(n);
                }
                
                break;
            
            
            default:
                throw new LjsCompilerError();
        }
        
        
        
    }
    
}