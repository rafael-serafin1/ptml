
using System.Runtime.CompilerServices;
using complexs.components.structure;

namespace complexs.components;

public abstract class TerminalComponent
{
    protected Coordinates _position;
    protected Coordinates _previous_position;

    protected int _width;
    protected int _height;
    protected string _placeholder = string.Empty;

    protected TerminalComponent(Coordinates position, int width, int height, string placeholder = "~~placeholder~~")
    {
        _position = position;
        _width = width;
        _height = height;
        _placeholder = placeholder ?? throw new ArgumentNullException(nameof(placeholder));
    }

    public virtual void Draw(ref Thread thread, string placeholder = "~~ph~~")
    {
        throw new NotImplementedException("Faça 'override' desse método.");
    }

    protected void GotoCoordinates()
    {
        Console.SetCursorPosition(_position.Left, _position.Top);
    }
    
    protected void GotoPreviousCoordinates()
    {
        Console.SetCursorPosition(_previous_position.Left, _previous_position.Top);
    }

    protected void SaveCurrent()
    {
        (_previous_position.Left, _previous_position.Top) = Console.GetCursorPosition();
    }
}