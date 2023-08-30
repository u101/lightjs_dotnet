### Runtime/Compiler features

* LjsRuntime variables access

    > LjsRuntime.GetVar(string varName)
   
    > LjsRuntime.SetVar(string varName, LjsObject value)

* LjsRuntime function invokation

    > LjsRuntime.Invoke(string functionName) with overloads for different arguments count

* LjsString methods and operations

  > substring(..)

  > str[] character access

* Global functions

  > int(..)
  
  > Number(..)

  > String(..)

  > parseInt(..)

  > parseFloat(..)

* LjsObject toString()

* LjsArray with methods and length property

* LjsDict

* 'this' reference support

* Math package

* Named functions inside functions

### Compiler features
* Save token positions in compiled code

### Project-wide
* Helper class(es) for raising exceptions
* Add extended info in exception (pointer to the token that caused an Exception) 

### Ast features
* Try catch block
* 'new' operator
* for (x in y) loop
* switch(x) {case y:...} branching
* Exponentiation operator **
* 'let' variable definition

