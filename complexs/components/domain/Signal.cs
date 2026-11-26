using complexs.components;
using complexs.components.events;
using complexs.components.structure;

namespace complexs.components.domain;

public class Signal : TerminalComponent
{
    private readonly MouseSignalEvent mouseSignalEvent = new(MouseSignalEvent.MouseEventTypes.SingleClick);

    public Signal(Coordinates coordinates, int width, int height = 1) : base(coordinates, width, height) {  }

    public override void Draw(ref Thread thread, string placeholder = "~~ph~~")
    {
        int finalW = _width / 2 - placeholder.Length;
        string component_image = $"[{new string(' ', finalW)}{placeholder}{new string(' ', finalW)}]";

        Console.SetCursorPosition(_position.Left, _position.Top);
        Console.Write(component_image);
    }

    public bool handleSingleClickEvent(Coordinates _clickCoordinates) 
    {
        throw new NotImplementedException();
    }
}