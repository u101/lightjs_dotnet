using static LightJS.Test.Runtime.RuntimeTestUtils;
using LightJS.Runtime;

namespace LightJS.Test.Runtime;

[TestFixture]
public class ArithmeticTests
{
    
    [Test]
    public void PostfixDecrementTest()
    {
        var code = """
        var a = 4;
        a--
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 4);
    }
    

    [Test]
    public void PrefixDecrementTest()
    {
        var code = """
        var a = 4;
        --a
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 3);
    }
    
    
    [Test]
    public void PostfixIncrementTest()
    {
        var code = """
        var a = 4;
        a++
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 4);
    }
    

    [Test]
    public void PrefixIncrementTest()
    {
        var code = """
        var a = 4;
        ++a
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();
        
        CheckResult(result, 5);
    }

    [Test]
    public void UnaryPlusIntegerTest()
    {
        var runtime = CreateRuntime("+123456");
        var result = runtime.Execute();
        
        CheckResult(result, 123456);
    }
    
    [Test]
    public void UnaryMinusIntegerTest()
    {
        const int expected = -(1+2+3+4+5);
        var runtime = CreateRuntime("-(1+2+3+4+5)");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
    }
    
    [Test]
    public void UnaryMinusDoubleTest()
    {
        const double expected = -(1.1+2+3+4+5);
        var runtime = CreateRuntime("-(1.1+2+3+4+5)");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
    }
    
    [Test]
    public void LogicalNotTest()
    {
        const bool expected = !(true || false);
        var runtime = CreateRuntime("!(true || false)");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
    }
    
    [Test]
    public void BitNotTest()
    {
        const int expected = ~0b01010101;
        var runtime = CreateRuntime("~0b01010101");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
    }
    
    [Test]
    public void SimpleDivIntegers()
    {
        var runtime = CreateRuntime("4/2");
        var result = runtime.Execute();
        
        CheckResult(result, 2);
    }

    [Test]
    public void ExpressionTest()
    {
        const double expected = 3.14 * (1 / 2.2 + 1e-9) - 1;
        var runtime = CreateRuntime("3.14 * (1 / 2.2 + 1e-9) - 1");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
    }

    [Test]
    public void BitOrTest()
    {
        const int expected = 0b01010101 | 0b10011001;
        var runtime = CreateRuntime("0b01010101 | 0b10011001");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
        
    }
    
    [Test]
    public void BitAndTest()
    {
        const int expected = 0b01010101 & 0b10011001;
        var runtime = CreateRuntime("0b01010101 & 0b10011001");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
        
    }
    
    
    [Test]
    public void BitShiftLeftTest()
    {
        const int expected = 0b01010101 << 3;
        var runtime = CreateRuntime("0b01010101 << 3");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
        
    }
    
    [Test]
    public void BitShiftRightTest()
    {
        const int expected = 0b01010101 >> 3;
        var runtime = CreateRuntime("0b01010101 >> 3");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
        
    }
    
    [Test]
    public void BitUnsignedShiftRightTest()
    {
        const int expected = 0b01010101 >>> 3;
        var runtime = CreateRuntime("0b01010101 >>> 3");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
        
    }
    
    [Test]
    public void GreaterThenTest()
    {
        const bool expected = 123 > 100;
        var runtime = CreateRuntime("123 > 100");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
        
    }
    
    [Test]
    public void GreaterThenOrEqualTest()
    {
        const bool expected = 123 >= 100;
        var runtime = CreateRuntime("123 >= 100");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
        
    }
    
    [Test]
    public void LessThenTest()
    {
        const bool expected = 123 < 456;
        var runtime = CreateRuntime("123 < 456");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
        
    }
    
    [Test]
    public void LessThenOrEqualTest()
    {
        const bool expected = 123 <= 456;
        var runtime = CreateRuntime("123 <= 456");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
        
    }
    
    [Test]
    public void IntegersEqualTest()
    {
        const bool expected = 123 == 456;
        var runtime = CreateRuntime("123 == 456");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
        
    }
    
    [Test]
    public void IntegersNotEqualTest()
    {
        const bool expected = 123 != 456;
        var runtime = CreateRuntime("123 != 456");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
        
    }
    
    [Test]
    public void StringsEqualTest()
    {
        const bool expected = "hello" == "hello";
        var runtime = CreateRuntime("'hello' == 'hello'");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
        
    }
    
    [Test]
    public void StringsNotEqualTest()
    {
        const bool expected = "hello" != "world";
        var runtime = CreateRuntime("'hello' != 'world'");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
        
    }
    
    [Test]
    public void LogicalAndTest()
    {
        const bool expected = true && false;
        var runtime = CreateRuntime("true && false");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
        
    }
    
    [Test]
    public void LogicalOrTest()
    {
        const bool expected = true || false;
        var runtime = CreateRuntime("true || false");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
        
    }
    
    [Test]
    public void LogicalExpressionAndTest()
    {
        const bool expected = (1 > 2) && (2 > 1);
        var runtime = CreateRuntime("(1 > 2) && (2 > 1)");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
        
    }
    
    [Test]
    public void LogicalExpressionOrTest()
    {
        const bool expected = (1 > 2) || (2 > 1);
        var runtime = CreateRuntime("(1 > 2) || (2 > 1)");
        var result = runtime.Execute();
        
        CheckResult(result, expected);
        
    }
    
    
    

}