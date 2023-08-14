using LightJS.Ast;

namespace LightJS.Test;

public static class TestUtils
{

    public static string LoadJsFile(string fileName)
    {
       return File.ReadAllText(Path.Combine(
            TestContext.CurrentContext.TestDirectory, "js", fileName));
    }

    public static ILjsAstNode BuildAstNode(string sourceCode)
    {
        var builder = new LjsAstBuilder2(sourceCode);
        var model = builder.Build();
        return model.RootNode;
    }
    
}