# Light JS
LightJS is javascript-like language compiler and runtime written in C# ***just for fun***

LightJS has no external dependencies and is not using any external libraries 

This is the Preview version, not everything works well

### Hello world program

```csharp
var compiler = new LjsCompiler("console.log('hello world')");
var program = compiler.Compile();
var runtime = new LjsRuntime(program);

runtime.Execute();
```

Output:

> hello world
>
> Process finished with exit code 0.
> 
***TODO***
