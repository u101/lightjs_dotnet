namespace LightJS.Test.Ast;

[TestFixture]
public class SwitchBlockTests
{
    [Test]
    public void EmptySwitchBlockTest()
    {
        const string code = """
        switch (x) {}
        """;
        
        var node = BuildAstNode(code);

        var expected = Switch("x".ToVar());
            
        Match(node, expected);
    }
    
}