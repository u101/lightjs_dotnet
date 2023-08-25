using LightJS.Runtime;

namespace LightJS.Test.Runtime;

[TestFixture]
public class ArithmeticTests
{

    [Test]
    public void SimpleDivIntegers()
    {
        var runtime = RuntimeTestUtils.CreateRuntime("4/2");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<int>(2)));
    }

    [Test]
    public void ExpressionTest()
    {
        const double expected = 3.14 * (1 / 2.2 + 1e-9) - 1;
        var runtime = RuntimeTestUtils.CreateRuntime("3.14 * (1 / 2.2 + 1e-9) - 1");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<double>(expected)));
    }

    [Test]
    public void BitOrTest()
    {
        const int expected = 0b01010101 | 0b10011001;
        var runtime = RuntimeTestUtils.CreateRuntime("0b01010101 | 0b10011001");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<int>(expected)));
        
    }
    
    [Test]
    public void BitAndTest()
    {
        const int expected = 0b01010101 & 0b10011001;
        var runtime = RuntimeTestUtils.CreateRuntime("0b01010101 & 0b10011001");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<int>(expected)));
        
    }
    
    
    [Test]
    public void BitShiftLeftTest()
    {
        const int expected = 0b01010101 << 3;
        var runtime = RuntimeTestUtils.CreateRuntime("0b01010101 << 3");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<int>(expected)));
        
    }
    
    [Test]
    public void BitShiftRightTest()
    {
        const int expected = 0b01010101 >> 3;
        var runtime = RuntimeTestUtils.CreateRuntime("0b01010101 >> 3");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<int>(expected)));
        
    }
    
    [Test]
    public void BitUnsignedShiftRightTest()
    {
        const int expected = 0b01010101 >>> 3;
        var runtime = RuntimeTestUtils.CreateRuntime("0b01010101 >>> 3");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<int>(expected)));
        
    }
    
    [Test]
    public void GreaterThenTest()
    {
        const bool expected = 123 > 100;
        var runtime = RuntimeTestUtils.CreateRuntime("123 > 100");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<bool>(expected)));
        
    }
    
    [Test]
    public void GreaterThenOrEqualTest()
    {
        const bool expected = 123 >= 100;
        var runtime = RuntimeTestUtils.CreateRuntime("123 >= 100");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<bool>(expected)));
        
    }
    
    [Test]
    public void LessThenTest()
    {
        const bool expected = 123 < 456;
        var runtime = RuntimeTestUtils.CreateRuntime("123 < 456");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<bool>(expected)));
        
    }
    
    [Test]
    public void LessThenOrEqualTest()
    {
        const bool expected = 123 <= 456;
        var runtime = RuntimeTestUtils.CreateRuntime("123 <= 456");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<bool>(expected)));
        
    }
    
    [Test]
    public void IntegersEqualTest()
    {
        const bool expected = 123 == 456;
        var runtime = RuntimeTestUtils.CreateRuntime("123 == 456");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<bool>(expected)));
        
    }
    
    [Test]
    public void IntegersNotEqualTest()
    {
        const bool expected = 123 != 456;
        var runtime = RuntimeTestUtils.CreateRuntime("123 != 456");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<bool>(expected)));
        
    }
    
    [Test]
    public void StringsEqualTest()
    {
        const bool expected = "hello" == "hello";
        var runtime = RuntimeTestUtils.CreateRuntime("'hello' == 'hello'");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<bool>(expected)));
        
    }
    
    [Test]
    public void StringsNotEqualTest()
    {
        const bool expected = "hello" != "world";
        var runtime = RuntimeTestUtils.CreateRuntime("'hello' != 'world'");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<bool>(expected)));
        
    }
    
    [Test]
    public void LogicalAndTest()
    {
        const bool expected = true && false;
        var runtime = RuntimeTestUtils.CreateRuntime("true && false");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<bool>(expected)));
        
    }
    
    [Test]
    public void LogicalOrTest()
    {
        const bool expected = true || false;
        var runtime = RuntimeTestUtils.CreateRuntime("true || false");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<bool>(expected)));
        
    }
    
    [Test]
    public void LogicalExpressionAndTest()
    {
        const bool expected = (1 > 2) && (2 > 1);
        var runtime = RuntimeTestUtils.CreateRuntime("(1 > 2) && (2 > 1)");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<bool>(expected)));
        
    }
    
    [Test]
    public void LogicalExpressionOrTest()
    {
        const bool expected = (1 > 2) || (2 > 1);
        var runtime = RuntimeTestUtils.CreateRuntime("(1 > 2) || (2 > 1)");
        var result = runtime.Execute();
        
        Assert.That(result, Is.EqualTo(new LjsValue<bool>(expected)));
        
    }
    
    
    

}