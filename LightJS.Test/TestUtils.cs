namespace LightJS.Test;

public static class TestUtils
{

    public static string LoadJsFile(string fileName)
    {
       return File.ReadAllText(Path.Combine(
            TestContext.CurrentContext.TestDirectory, "js", fileName));
    }
    
}