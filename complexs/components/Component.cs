namespace complexs.components;

public abstract class Component
{
    protected Coordinates _position;
    protected int _width;
    protected int _height;
    protected string _placeholder = string.Empty;

    protected Component(Coordinates position, int width, int height, string placeholder = "~~placeholder~~")
    {
        _position = position;
        _width = width;
        _height = height;
        _placeholder = placeholder ?? throw new ArgumentNullException(nameof(placeholder));
    }
}