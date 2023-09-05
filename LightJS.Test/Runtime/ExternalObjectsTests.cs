using LightJS.Runtime;

namespace LightJS.Test.Runtime;

[TestFixture]
public class ExternalObjectsTests
{

    [Test]
    public void FieldsTest()
    {
        const string code = """
        point.x = 123;
        point.y = 456;
        point.xy = point.x + point.y;
        """;

        var p = new Point();
        
        var runtime = CreateRuntime(code);
        
        runtime.AddExternal("point", LjsTypesConverter.ToLjsObject(p));
        
        runtime.Execute();
        
        Assert.That(p.x, Is.EqualTo(123));
        Assert.That(p.y, Is.EqualTo(456));
        Assert.That(p.xy, Is.EqualTo(123+456));
        
    }
    
    [Test]
    public void PropertiesTest()
    {
        const string code = """
        point.x = 123;
        point.y = 456;
        point.xy = point.x + point.y;
        """;

        var p = new Point2();
        
        var runtime = CreateRuntime(code);
        
        runtime.AddExternal("point", LjsTypesConverter.ToLjsObject(p));
        
        runtime.Execute();
        
        Assert.That(p.x, Is.EqualTo(123));
        Assert.That(p.y, Is.EqualTo(456));
        Assert.That(p.xy, Is.EqualTo(123+456));
        
    }
    
    [Test]
    public void MethodsTest()
    {
        const string code = """
        point.SetXY(123,456)
        """;

        var p = new Point3();
        
        var runtime = CreateRuntime(code);
        
        runtime.AddExternal("point", LjsTypesConverter.ToLjsObject(p));
        
        runtime.Execute();
        
        Assert.That(p.x, Is.EqualTo(123));
        Assert.That(p.y, Is.EqualTo(456));
        
    }
    
    public class Point
    {
        [LjsField] public int x = 0;
        [LjsField] public int y = 0;
        [LjsField] public int xy = 0;

        public int z = 0;
    }
    
    public class Point2
    {
        [LjsField] public int x { get; set; } = 0;
        [LjsField] public int y { get; set; } = 0;
        [LjsField] public int xy { get; set; } = 0;
    }

    public class Point3
    {
        public int x { get; private set; } = 0;
        public int y { get; private set; } = 0;

        [LjsMethod]
        public void SetXY(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

}