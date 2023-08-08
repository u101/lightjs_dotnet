namespace LightJS.Tokenizer;

[Flags]
public enum LjsTokenClass
{
    None = 0,
    Word = 1 << 0,
    Literal = 1 << 1,
    Operator = 1 << 2,
    
    BinaryOperator = 1 << 3,
    UnaryOperator = 1 << 4,
    // operator that can be unary or binary depending on the context
    PolymorphicOperator = 1 << 5
    
    
}