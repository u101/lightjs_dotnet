namespace LightJS.Test.Runtime;

[TestFixture]
public class ThisKeywordTests
{

    [Test]
    public void SimpleObjectTest()
    {

        const string code = """
        var person = {
            name:"Bob",
            surname:"Smith",
            fullName:function() {
                return this.name + " " + this.surname;
            }
        };

        person.fullname()
        """;

        var runtime = CreateRuntime(code);

        var result = runtime.Execute();
        
        CheckResult(result, "Bob Smith");
        
    }
    
    
}