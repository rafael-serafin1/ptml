using complexs.components;
namespace complexs.components.domain;

public class Signal : Component
{
    public enum SignalType
    {
        Hover       = 0,
        SingClick   = 1,
        DoubleClick = 2,
        HoldClick   = 3
    }

    private readonly SignalType _signal;

    public Signal(Coordinates coordinates, int width, int height, int signal) : base(coordinates, width, height)
    {
        _signal = (SignalType) signal;
    }
}