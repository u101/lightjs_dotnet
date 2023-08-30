## Version 0.1

#### Project structure
- **Tokenizer** ( splits source code string into tokens )
- **Abstract Syntax Tree** (Ast package) with LjsAstBuilder builder 
- - ( produces abstract syntax tree model using tokenizer and source code string )
- **LjsCompiler** : compiles LjsAstModel into LjsProgram
- **LjsRuntime** : executes LjsProgram 

#### Supported features
- Basic binary and unary operations
- - > (+ - * ++ -- / > >= < <= == != etc ..)
- Variables and const declarations
- - > var x
- - > var x = 0, y = 1
- - > const x = 0, y = 1 // consts are mutable and treated as vars
- If statements
- - > if (a) ..
- - > if (a) {..} else if (b) {..} else {..}
- Ternary if operations
- - > x = a ? b : c
- - > x = a ? b ? c : d : e  
- Loops
- - > while (..) {}
- - > for(;;) {}
- Basic runtime types int,double,bool,string, null, undefined
- Function declarations
- - named func
- - - > function foo() {..}
- - anonymous func
- - - > var x = function() {..}
- Function calls
- Able to compile and run scripts and get result 
- - > var result = runtime.Execute()