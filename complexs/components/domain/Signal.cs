using complexs.components;
using complexs.components.events;

namespace complexs.components.domain;

public class Signal : Component
{
    private readonly MouseSignalEvent mouseSignalEvent = new(MouseSignalEvent.MouseEventTypes.SingleClick);

    public Signal(Coordinates coordinates, int width, int height = 1) : base(coordinates, width, height) {  }

    public override void Draw(int L = 0, int T = 0, string placeholder = "~~ph~~")
    {
        int finalW = _width / 2 - placeholder.Length;
        string component_image = $"[{new string(' ', finalW)}{placeholder}{new string(' ', finalW)}]";

        Console.SetCursorPosition(L, T);
        Console.Write(component_image);
    }

    public bool handleSingleClickEvent(Coordinates _clickCoordinates) 
    {
        throw new NotImplementedException();
    }
}