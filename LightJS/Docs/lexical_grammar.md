# Lexical grammar

## Comments

LightJS has two ways to add comments to code: line comments and block comments

### Line comments

The first way is the // comment; this makes all text following it on the same line into a comment. For example:

```js
function comment() {
  // This is a one line comment
  console.log("Hello world!");
}
comment();
```

### Block comments

The second way is the /* */ style, which is much more flexible.

For example, you can use it on a single line:

```js
function comment() {
  /* This is a one line comment */
  console.log("Hello world!");
}
comment();
```

You can also make multiple-line comments, like this:

```js
function comment() {
    /* This comment spans multiple lines. Notice
      that we don't need to end the comment until we're done. */
  console.log("Hello world!");
}
comment();
```

## Identifiers

An identifier is used to link a value with a name. Identifiers can be used in various places:

```js
const c = 123; // Variable declaration (may also be `let` or `var`)
function fn() {} // Function declaration
const obj = { key: "value" }; // Object keys
```

In LightJS, identifiers are made of alphanumeric characters, dollar sign ($)  and
underscores (_). [0-9a-bA-B$_]
Identifiers are not allowed to start with numbers. 
Identifiers are only limited to ASCII.

## Keywords

Keywords are tokens that look like identifiers but have special meanings in LightJS.

All keywords are reserved, meaning that they cannot be used as an identifier for variable declarations, function declarations, etc.

### Reserved words

* break
* case
* catch
* class ( not supported yet )
* const
* continue
* default
* do ( not supported yet )
* else
* extends ( not supported yet )
* false
* finally ( not supported yet )
* for
* function
* if
* import ( not supported yet )
* in ( not supported yet )
* instanceof ( not supported yet )
* let
* NaN
* new ( not supported yet )
* null
* of ( not supported yet )
* return
* super ( not supported yet )
* switch
* this
* throw ( not supported yet )
* true
* try ( not supported yet )
* typeof ( not supported yet )
* undefined
* var
* while

## Literals

This section discusses literals that are atomic tokens.

### Null literal

```js
null
```

### Boolean literal


```js
true
false
```

### Numeric literals

The LjsInteger and LjsDouble types use numeric literals

```js
/* DECIMAL */
1234567890 // decimal LjsInteger
3.14159265359 // decimal LjsDouble

/* EXPONENTIAL (LjsDouble) */
1e-5
1e+5
1.4e+8
0.5e-9

/* BINARY */
0b0
0b1010101010101
0b01

/* HEX */
0x0eF45ab
0x00000000
0xffffffff
```

### String literals

A string literal is zero or more Unicode code points enclosed in single or double quotes.

```js
'foo'
"bar"
```

TODO : escape sequences table

# Automatic semicolon insertion

it works, but not in every case