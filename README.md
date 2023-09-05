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

### More complex example : merge sort function

```csharp
const string code = """
function mergeSort(arr) {

    // Base case
    if (arr.length <= 1) return arr
    var mid = Math.floor(arr.length / 2)

    // Recursive calls
    var left = mergeSort(arr.slice(0, mid))
    var right = mergeSort(arr.slice(mid))

    return merge(left, right)

    function merge(left, right) {
        var sortedArr = [] // the sorted items will go here

        while (left.length && right.length) {
            // Insert the smallest item into sortedArr
            if (left[0] < right[0]) {
                sortedArr.push(left.shift())
            } else {
                sortedArr.push(right.shift())
            }
        }
        // Use spread operators to create a new array, combining the three arrays
        return sortedArr.concat(left,right);
    }
}

mergeSort([3, 5, 8, 5, 99, 1])
""";

var runtime = CreateRuntime(code);
var result = runtime.Execute();
// result will be LjsArray(1, 3, 5, 5, 8, 99)
```
