using LightJS.Runtime;

namespace LightJS.Test.Runtime;

[TestFixture]
public class ExternalObjectsTests
{

    [Test]
    public void AddExternalApiTest()
    {
        const string code = """
        function foo() {
            someApi.Foo();
            
            if (someApi.Bar()) {
                return someApi.ApiVersion;
            }
            return -1;
        }
        """;
        
        var runtime = CreateRuntime(code);
        
        var someApi = new SomeApi();
        
        runtime.AddExternal("someApi", LjsTypesConverter.ToLjsObject(someApi));

        runtime.Execute();
        
        var result = runtime.Invoke("foo");

        CheckResult(result, 123);
    }
    
    public class SomeApi
    {
        [LjsField] public int ApiVersion => 123;
        
        [LjsMethod] public void Foo() {/* */}

        [LjsMethod] public bool Bar() => true;
    }

    [Test]
    public void PassObjectToFunctionTest()
    {
        const string code = """
        function getPersonInfo(p) {
            let fullName = p.Name + " " + p.Surname;
            let address = p.GetAddress();
            
            return fullName + "," + address.City + "," + address.PostalCode;
        }
        """;
        
        var runtime = CreateRuntime(code);
        runtime.Execute();

        var person = new Person("John", "Doe");
        
        var result = runtime.Invoke(
            "getPersonInfo", LjsTypesConverter.ToLjsObject(person));
        
        CheckResult(result, "John Doe,London,123456");
    }
    
    public class Person
    {
        [LjsField] public string Name;
        [LjsField] public string Surname;

        public Person(string name, string surname)
        {
            Name = name;
            Surname = surname;
        }

        [LjsMethod]
        public Address GetAddress() => new("London", 123456);
    }
    
    public class Address
    {
        [LjsField] public string City;
        [LjsField] public int PostalCode;

        public Address(string city, int postalCode)
        {
            City = city;
            PostalCode = postalCode;
        }
    }
    
    
    

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