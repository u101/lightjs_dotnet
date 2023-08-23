using LightJS.Ast;
using LightJS.Errors;
using LightJS.Program;
using LightJS.Tokenizer;

namespace LightJS.Compiler;

public class LjsCompiler
{
    private readonly LjsAstModel _astModel;

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
        throw new NotImplementedException();
    }

    private void ProcessNode(ILjsAstNode node, List<byte> instructions)
    {
        switch (node)
        {
            case LjsAstLiteral<int> lit:
                
                break;
            case LjsAstLiteral<double> lit:
                
                break;
            case LjsAstLiteral<string> lit:
                
                break;
            
            case LjsAstNull _:
                instructions.Add(LjsInstructionCodes.ConstNull);
                break;
            case LjsAstUndefined _:
                instructions.Add(LjsInstructionCodes.ConstUndef);
                break;
            
            case LjsAstLiteral<bool> lit:
                instructions.Add(lit.Value ? LjsInstructionCodes.ConstTrue : LjsInstructionCodes.ConstFalse);
                break;
            
            
            case LjsAstGetVar getVar:
                //
                break;
            
            
            default:
                throw new LjsCompilerError();
        }
        
        throw new NotImplementedException();
    }
    
}