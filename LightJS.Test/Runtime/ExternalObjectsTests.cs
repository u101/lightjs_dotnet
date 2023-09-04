using LightJS.ExternalApi;
using LightJS.Runtime;

namespace LightJS.Test.Runtime;

[TestFixture]
public class ExternalObjectsTests
{

    [Test]
    public void SimpleTest()
    {
        const string code = """
        point.x = 123;
        point.y = 456;
        point.xy = point.x + point.y;
        """;

        var p = new Point();
        
        var runtime = CreateRuntime(code);
        
        runtime.AddExternal("point", 
            ExternalObjectAdapterFactory.CreateObjectAdapter(p));
        
        runtime.Execute();
        
        Assert.That(p.x, Is.EqualTo(123));
        Assert.That(p.y, Is.EqualTo(456));
        Assert.That(p.xy, Is.EqualTo(123+456));
        
    }
    
    public class Point
    {
        [LjsField] public int x = 0;
        [LjsField] public int y = 0;
        [LjsField] public int xy = 0;

        public int z = 0;
    }
    
}