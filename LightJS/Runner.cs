namespace LightJS;

public static class Runner
{

    public static void PrintJsSourceCodeWithLineNumbers()
    {
        const string jsFilesDir = "/Users/nikitak/Proj/LightJS/LightJS/js";

        var jsString = File.ReadAllText($"{jsFilesDir}/simpleTest.js");

        var ljsReader = new LjsReader(jsString);

        var line = 0;
        var col = 0;

        Console.Write("line 0:");

        while (ljsReader.HasNextChar())
        {
            var c = ljsReader.ReadNextChar();

            if (c == '\r') continue;
            
            Console.Write(c);
            
            if (c == '\n')
            {
                ++line;
                col = 0;
                Console.Write($"line {line}:");
            }
            else
            {
                // Console.Write($"[{col}]");
                ++col;
            }
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