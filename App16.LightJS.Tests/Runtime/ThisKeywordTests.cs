using static App16.LightJS.Tests.Utils.RuntimeTestUtils;

namespace App16.LightJS.Tests.Runtime;

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
            fullname:function() {
                return this.name + " " + this.surname;
            }
        };

        person.fullname()
        """;

        var runtime = CreateRuntime(code);

        var result = runtime.Execute();
        
        CheckResult(result, "Bob Smith");
        
    }
    
    [Test]
    public void InnerFunctionsCallTest()
    {

        const string code = """
        var point = {

            x:101,
            y:202,

            ln:function() {
                return Math.sqrt(this.getX()*this.getX() + this.getY()*this.getY());
            },
            getX:function() { return this.x; },
            getY:function() { return this.y; },
        };

        point.ln()
        """;

        var runtime = CreateRuntime(code);

        var result = runtime.Execute();

        CheckResult(result, Math.Sqrt(101.0 * 101 + 202.0 * 202));
        
    }
    
    
}