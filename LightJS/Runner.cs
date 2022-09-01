namespace LightJS;

public static class Runner
{

    public static void TestTokenizer()
    {
        const string jsFilesDir = "/Users/nikitak/Proj/LightJS/LightJS/js";

        var jsString = File.ReadAllText($"{jsFilesDir}/simpleTest.js");

        var ljsReader = new LjsReader(jsString);

        var ljsTokenizer = new LjsTokenizer(ljsReader);

        var tokens = ljsTokenizer.ReadTokens();
        
        Console.WriteLine("DONE");
    }
    
    public static void PrintJsSourceCodeWithLineNumbers()
    {
        const string jsFilesDir = "/Users/nikitak/Proj/LightJS/LightJS/js";

        var jsString = File.ReadAllText($"{jsFilesDir}/simpleTest.js");

        var ljsReader = new LjsReader(jsString);


        Console.Write("line 0:");

        while (ljsReader.HasNextChar)
        {
            var c = ljsReader.CurrentChar;

            if (c == '\r') continue;
            
            Console.Write(c);
            
            if (c == '\n')
            {
                Console.Write($"line {ljsReader.CurrentLine}:");
            }
            
            ljsReader.MoveForward();
        }
    }

    public static void PrintCharCodes()
    {
        var chars = new char[128];

        for (var i = 0; i < chars.Length; i++)
        {
            var c = (char)i;
            Console.WriteLine($"char {c} code {i}");
        }
    }
    
}