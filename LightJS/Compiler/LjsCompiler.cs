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
        return _program;
    }

    private void ProcessNode(ILjsAstNode node)
    {
        switch (node)
        {
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
                    
                    default:
                        throw new NotImplementedException();
                }
                
                break;
            
            
            // case LjsAstGetVar getVar:
            //     break;
            
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