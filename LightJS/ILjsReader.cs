namespace LightJS;

public interface ILjsReader
{
    char ReadNextChar();

    bool HasNextChar();
}