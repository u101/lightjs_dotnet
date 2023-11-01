using App16.LightJS.Runtime;

namespace App16.LightJS.Tests.Runtime;

[TestFixture]
public class RuntimeUtilsTests
{

    [Test]
    public void TestIndicesCombination()
    {

        var pairs = new (int, int)[]
        {
            (1,0),
            (0,0),
            (0,1),
            (123,456),
            (65500,65500),
            (1,65500),
            (0,65500),
            (555,65500),
            (65500,1),
            (65500,0),
            (65500,555),
        };

        for (var i = 0; i < pairs.Length; i++)
        {
            var (localIndex, funcIndex) = pairs[i];
            var j = LjsRuntimeUtils.CombineTwoShorts(localIndex, funcIndex);

            var lci = LjsRuntimeUtils.GetLocalIndex(j);
            var fci = LjsRuntimeUtils.GetFunctionIndex(j);
            
            Assert.That(lci, Is.EqualTo(localIndex));
            Assert.That(fci, Is.EqualTo(funcIndex));
        }

    }
}