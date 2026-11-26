
using complexs.components;
using complexs.components.events;
using complexs.components.structure;

namespace complexs.components.domain;

public class Button : TerminalComponent
{
    private const string Escape = "\x1b";
    private readonly string _label;
    private bool _hovered;
    private bool _pressed;

    public Button(Coordinates position, string label, int? width = null)
        : base(position, width ?? ValidateLabel(label).Length + 4, 1, ValidateLabel(label))
    {
        if (width.HasValue && width.Value < label.Length + 2)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must fit the label and its borders.");

        _label = label;
    }

    private static string ValidateLabel(string label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        return label;
    }

    public bool IsHovered => _hovered;
    public bool IsPressed => _pressed;
    public bool IsClicked { get; private set; }

    public bool Contains(int x, int y) =>
        x >= _position.Left &&
        y >= _position.Top &&
        x < _position.Left + _width &&
        y < _position.Top + _height;

    public bool Contains(Coordinates position) =>
        Contains(position.Left, position.Top);

    public bool HandleMouseEvent(string sgrSequence)
    {
        Thread? t = null;
        IsClicked = false;

        if (!MouseSignalEvent.TryParseSgr(sgrSequence, out var eventType, out var position))
            return false;

        _hovered = Contains(position);

        if (eventType == MouseSignalEvent.MouseEventTypes.SingleClick)
        {
            _pressed = Contains(position);
            if (_pressed)
                IsClicked = true;
        }
        else
        {
            _pressed = false;
        }

        Draw(ref t);
        return IsClicked;
    }

    public override void Draw(ref Thread thread, string placeholder = "~~ph~~")
    {
        int left = _position.Left;
        int top = _position.Top;
        string label = _label.Length > _width - 2
            ? _label[..(_width - 2)]
            : _label.PadLeft((_width - 2 + _label.Length) / 2).PadRight(_width - 2);

        string foreground = _pressed ? "37" : "30";
        string background = _hovered ? "104" : _pressed ? "44" : "47";

        Console.SetCursorPosition(left, top);
        Console.Write($"{Escape}[{foreground};{background}m[{label}]{Escape}[0m");
    }
}
