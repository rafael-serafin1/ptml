// AQUI FICARÁ A CLASSE "MouseSignalEvent" QUE SERÁ RESPONSÁVEL POR REALIZAR A OBSERVAÇÃO DE SINAIS LANÇADOS PELO TERMINAL DE ORIGEM DO MOUSE, FAZER O PARSING DO EVENTO E CLASSIFICAR ENTRE LMB/MMB/RMB

namespace complexs.components.events;

public class MouseSignalEvent
{
    public enum MouseEventTypes
    {
        None        = -1,
        Hover       = 0,
        SingleClick = 1,
        DoubleClick = 2,
        HoldClick   = 3
    }

    private MouseEventTypes expectedEvent;
    private MouseEventTypes capturedEvent = MouseEventTypes.None;

    public MouseEventTypes ExpectedEvent { get => expectedEvent; }
    public MouseEventTypes CapturedEvent { get => capturedEvent; }

    public MouseSignalEvent(MouseEventTypes _expected = MouseEventTypes.SingleClick)
    {
        expectedEvent = _expected;
    }

    public static bool TryParseSgr(string sequence, out MouseEventTypes eventType, out Coordinates position)
    {
        eventType = MouseEventTypes.None;
        position = new Coordinates();

        if (string.IsNullOrEmpty(sequence))
            return false;

        const string Prefix = "\x1b[<";
        if (!sequence.StartsWith(Prefix, StringComparison.Ordinal))
            return false;

        int finalIndex = sequence.Length - 1;
        char final = sequence[finalIndex];
        if (final != 'M' && final != 'm')
            return false;

        string payload = sequence[Prefix.Length..finalIndex];
        string[] parts = payload.Split(';');
        if (parts.Length != 3 ||
            !int.TryParse(parts[0], out int button) ||
            !int.TryParse(parts[1], out int x) ||
            !int.TryParse(parts[2], out int y) ||
            x < 1 ||
            y < 1)
            return false;

        position = new Coordinates(x - 1, y - 1);
        eventType = final == 'm'
            ? MouseEventTypes.None
            : button switch
            {
                0 => MouseEventTypes.SingleClick,
                1 => MouseEventTypes.HoldClick,
                35 => MouseEventTypes.Hover,
                _ => MouseEventTypes.None
            };

        return true;
    }
}