using LightJS.Runtime;

namespace LightJS.Test.Runtime;

[TestFixture]
public class AlgorithmsTests
{

    [Test]
    public void BubbleSortTest()
    {
        var input = new[]
        {
            234, 43, 55, 63, 5, 6, 235, 547
        };
        
        var code = """
        // Bubble sort Implementation using Javascript
          
        // Creating the bblSort function
        function bblSort(arr) {
          
            for (var i = 0; i < arr.length; i++) {
          
                // Last i elements are already in place  
                for (var j = 0; j < (arr.length - i - 1); j++) {
          
                    // Checking if the item at present iteration 
                    // is greater than the next iteration
                    if (arr[j] > arr[j + 1]) {
          
                        // If the condition is true
                        // then swap them
                        var temp = arr[j]
                        arr[j] = arr[j + 1]
                        arr[j + 1] = temp
                    }
                }
            }
          
            // Print the sorted array
            //console.log(arr);
        }
          
        // This is our unsorted array
        var arr = [$];
          
        // Now pass this array to the bblSort() function
        bblSort(arr);
        
        arr
        """;
        
        code = code.Replace(
            "$", string.Join(',', input));
        
        var expected = input.ToArray();
        
        Array.Sort(expected);
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();

        CheckResult(result, expected);
    }

    [Test]
    public void ArrayMergeTest()
    {
        var code = """
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
        
        merge([1, 4], [2, 6, 9])
        """;
        
        var runtime = CreateRuntime(code);
        var result = runtime.Execute();

        CheckResult(result, Arr(1,2,4,6,9));
    }

    [Test]
    public void MergeSortTest()
    {
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

        CheckResult(result, Arr(1, 3, 5, 5, 8, 99));
    }
    
}