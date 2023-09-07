# Light JS
LightJS is a javascript-like language compiler and runtime written in C#

All LightJS code is valid javascript code, but not all javascript features are supported

LightJS has no external dependencies 

This is the Preview version, not everything well tested, some features are not implemented yet

[LightJS lexical grammar page](LightJS/Docs/lexical_grammar.md)

## Getting started

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

