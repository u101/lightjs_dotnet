using LightJS.Outsource;

namespace LightJS.Test;

[TestFixture]
public class MatherTest
{


    [Test]
    public void SimpleTest()
    {
        const string expression = 
            "15/(7-(1+1))*3-(2+(1+1))*15/(7-(200+1))*3-(2+(1+1))*(15/(7-(1+1))*3-(2+(1+1))+15/(7-(1+1))*3-(2+(1+1)))";

        const double expectedResult =
            15.0 / (7.0 - (1.0 + 1.0)) * 3.0 - (2.0 + (1.0 + 1.0)) * 15.0 / (7.0 - (200.0 + 1.0)) * 3.0 - (2.0 + (1.0 + 1.0)) *
            (15.0 / (7.0 - (1.0 + 1.0)) * 3.0 - (2.0 + (1.0 + 1.0)) + 15.0 / (7.0 - (1.0 + 1.0)) * 3.0 - (2.0 + (1.0 + 1.0)));
        
        var mather = new Mather(expression);
        
        TestContext.WriteLine("Постфиксная форма: " + mather.postfixExpr);

        var result = mather.Calc();
        
        Assert.That(result, Is.EqualTo(expectedResult));
    }
    
}