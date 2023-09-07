# Light JS
LightJS is a javascript-like language compiler and runtime written in C#

All LightJS code is valid javascript code, but not all javascript features are supported

LightJS has no external dependencies 

This is the Preview version, not everything well tested, some features are not implemented yet

[LightJS lexical grammar page](LightJS/Docs/lexical_grammar.md)

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

### Usage

With LjsRuntime you can execute your script once and get computation result.
(runtime returns the last value on stack)

For example:

```csharp
const string code = """
fact(8)

function fact(n) {
    if (n <= 0) return 0;
    if (n == 1) return 1;
    return n * fact(n - 1); 
}
""";

var compiler = new LjsCompiler(code);
var program = compiler.Compile();
var runtime = new LjsRuntime(program);

var result = runtime.Execute(); // will be LjsInteger(40320)
```
You can also call script functions with LjsRuntime.Invoke()

For example:

```csharp
const string code = """
function fact(n) {
    if (n <= 0) return 0;
    if (n == 1) return 1;
    return n * fact(n - 1); 
}
""";

var compiler = new LjsCompiler(code);
var program = compiler.Compile();
var runtime = new LjsRuntime(program);
// we need to call execute to initailize all things
runtime.Execute();

// and then
for (var i = 1; i <= 6; i++)
{
    var result = runtime.Invoke("fact", i); // 1, 2, 6, 24, 120, 720
}
```
You can also pass regular objects and interact with them in script

You can either pass reference to your object via LjsRuntime.Invoke()

For example:

```csharp

public class Person
{
    [LjsField] public string Name;
    [LjsField] public string Surname;

    public Person(string name, string surname)
    {
        Name = name;
        Surname = surname;
    }

    [LjsMethod]
    public Address GetAddress() => new("London", 123456);
}

public class Address
{
    [LjsField] public string City;
    [LjsField] public int PostalCode;

    public Address(string city, int postalCode)
    {
        City = city;
        PostalCode = postalCode;
    }
}
// -------------------------------------------------------------------

const string code = """
function getPersonInfo(p) {
    let fullName = p.Name + " " + p.Surname;
    let address = p.GetAddress();
    
    return fullName + "," + address.City + "," + address.PostalCode;
}
""";

var compiler = new LjsCompiler(code);
var program = compiler.Compile();
var runtime = new LjsRuntime(program);
runtime.Execute();

var person = new Person("John", "Doe");

var result = runtime.Invoke(
    "getPersonInfo", LjsTypesConverter.ToLjsObject(person));
    
// result will be LjsString("John Doe,London,123456")

```

Or you can pass reference to your object via LjsRuntime.AddExternal

For example:

```csharp
public class SomeApi
{
    [LjsField] public int ApiVersion => 123;
    
    [LjsMethod] public void Foo() {/* */}

    [LjsMethod] public bool Bar() => true;
}
// -------------------------------------------------------------------

const string code = """
function foo() {
    someApi.Foo();
    
    if (someApi.Bar()) {
        return someApi.ApiVersion;
    }
    return -1;
}
""";

var compiler = new LjsCompiler(code);
var program = compiler.Compile();
var runtime = new LjsRuntime(program);

var someApi = new SomeApi();

runtime.AddExternal("someApi", LjsTypesConverter.ToLjsObject(someApi));

runtime.Execute();

var result = runtime.Invoke("foo"); // will be LjsInteger(123)

```


### Overview

Everything in LightJS is LjsObject

All primitive types are boxed

Main types are 
* LjsInteger
* LjsDouble
* LjsBoolean
* LjsString
* LjsArray // [a,b,c,..]
* LjsDictionary // {a:1, b:2, ..}







