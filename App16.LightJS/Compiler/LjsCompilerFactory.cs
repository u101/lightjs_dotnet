using App16.ALang.Ast;
using App16.ALang.Js.Ast;
using App16.ALang.Js.Ast.Builders;
using App16.ALang.Tokenizers;

namespace App16.LightJS.Compiler;

public static class LjsCompilerFactory
{

    private static readonly ILjsCompilerNodeProcessor _nodeProcessor = CreateNodeProcessor();
    
    public static LjsCompiler CreateCompiler(IAstNode astModel) => new(astModel, _nodeProcessor);

    public static LjsCompiler CreateCompiler(string sourceCodeString)
    {
        if (string.IsNullOrEmpty(sourceCodeString))
        {
            throw new ArgumentException("input string is null or empty");
        }

        var astModelBuilder = JsAstBuilderFactory.CreateBuilder(sourceCodeString);
        
        var astModel = astModelBuilder.Build();

        return CreateCompiler(astModel);
    }
    
    public static LjsCompiler CreateCompiler(string sourceCodeString, List<Token> tokens)
    {
        if (string.IsNullOrEmpty(sourceCodeString))
        {
            throw new ArgumentException("input string is null or empty");
        }
        
        if (tokens == null)
            throw new ArgumentNullException(nameof(tokens));

        if (tokens.Count == 0)
            throw new ArgumentException("empty tokens list");
        
        var astModelBuilder = JsAstBuilderFactory.CreateBuilder(sourceCodeString, tokens);
        
        var astModel = astModelBuilder.Build();
        
        return CreateCompiler(astModel);
    }

    private static ILjsCompilerNodeProcessor CreateNodeProcessor()
    {
        var nodesProcessorDefault = new LjsCompilerNodeSelectorDefault();
        var nodesProcessorByType = new LjsCompilerNodeSelectorByType(nodesProcessorDefault);
        var nodesProcessor = new LjsCompilerNodeSelectorWithLookup(nodesProcessorByType);
        
        nodesProcessor.Register(
            new LjsLiteralNodeLookup(),
            new LjsLiteralNodeProcessor());
        
        nodesProcessor.Register(new LjsDotPropertyAssignLookup(), new LjsDotPropertyAssignProcessor(nodesProcessor));
        
        nodesProcessor.Register(new LjsSqbPropertyAssignLookup(), new LjsSqbPropertyAssignProcessor(nodesProcessor));
        
        nodesProcessor.Register(new LjsVarAssignLookup(), new LjsVarAssignProcessor(nodesProcessor));
        
        nodesProcessorByType.Register(
            typeof(AstBinaryOperation), new LjsBinaryOperationProcessor(nodesProcessor));
        
        nodesProcessor.Register(new LjsUnaryOperationLookup(), new LjsUnaryOperationProcessor(nodesProcessor));
        
        nodesProcessor.Register(
            new LjsVarIncrementLookup(),
            new LjsVarIncrementProcessor());
        
        nodesProcessor.Register(new LjsDotPropertyIncrementLookup(), new LjsDotPropertyIncrementProcessor(nodesProcessor));
        
        nodesProcessor.Register(new LjsSqbPropertyIncrementLookup(), new LjsSqbPropertyIncrementProcessor(nodesProcessor));
        
        nodesProcessorByType.Register(
            typeof(AstGetDotProperty), new LjsDotPropertyGetProcessor(nodesProcessor));
        
        nodesProcessorByType.Register(
            typeof(AstGetSquareBracketsProp), new LjsSqbPropertyGetProcessor(nodesProcessor));
        
        nodesProcessorByType.Register(
            typeof(AstGetId), new LjsVarGetProcessor());

        var controlTransferNodesProcessor = new LjsControlTransferNodesProcessor();
        nodesProcessorByType.Register(typeof(AstContinue), controlTransferNodesProcessor);
        nodesProcessorByType.Register(typeof(AstBreak), controlTransferNodesProcessor);
        
        nodesProcessorByType.Register(
            typeof(AstEmptyNode), new LjsEmptyNodeProcessor());
        
        nodesProcessorByType.Register(
            typeof(JsFunctionCall), new LjsFunctionCallProcessor(nodesProcessor));
        
        nodesProcessorByType.Register(
            typeof(JsVariableDeclaration),  new LjsVarDeclarationProcessor(nodesProcessor));
        
        nodesProcessorByType.Register(
            typeof(JsObjectLiteral),  new LjsObjectLiteralProcessor(nodesProcessor));
        
        nodesProcessorByType.Register(
            typeof(JsArrayLiteral),  new LjsArrayLiteralProcessor(nodesProcessor));
        
        nodesProcessorByType.Register(
            typeof(AstReturn),  new LjsReturnProcessor(nodesProcessor));
        
        nodesProcessorByType.Register(
            typeof(AstIfBlock),  new LjsIfBlockProcessor(nodesProcessor));
        
        nodesProcessorByType.Register(
            typeof(JsSwitchBlock),  new LjsSwitchBlockProcessor(nodesProcessor));
        
        nodesProcessorByType.Register(
            typeof(AstWhileLoop),  new LjsWhileLoopProcessor(nodesProcessor));
        
        nodesProcessorByType.Register(
            typeof(AstForLoop),  new LjsForLoopProcessor(nodesProcessor));
        
        nodesProcessorByType.Register(
            typeof(AstSequence),  new LjsSequenceProcessor(nodesProcessor));
        
        nodesProcessorByType.Register(
            typeof(JsAnonymousFunctionDeclaration),  new LjsAnonymousFunctionDeclarationProcessor(nodesProcessor));
        
        nodesProcessorByType.Register(
            typeof(JsNamedFunctionDeclaration),  new LjsNamedFunctionDeclarationProcessor(nodesProcessor));


        return nodesProcessor;

    }
}