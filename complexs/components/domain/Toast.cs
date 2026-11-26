using System.Diagnostics;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.Text;
using complexs.components;
using complexs.components.structure;

namespace complexs.components.domain;

public class Toast : TerminalComponent {
    private string _message = string.Empty;
    private string _previous_output = string.Empty;

    private int _duration = 3000;          // Duration in milliseconds
    private bool shouldLI = true;

    // sem necessidade de verificar se é menor ou igual a zero, pois o código F# fara isso ja.
    public Toast(Coordinates coordinates, int width, int height, int duration) : base(coordinates, width, height)
    {
        _duration = duration;
    }

    private void TryLowerIntensity(long time)
    {
        if (time > (_duration / 2) && shouldLI)
        {
            GotoCoordinates();
            Terminal.Dim(_message);
            shouldLI = false;
        }
    }

    public void Display(string message)
    {
        var (l, t) = Console.GetCursorPosition();
        _previous_position = new Coordinates{ Left = l, Top = t };

        _message = message;
        _width   = _message.Length;

        GotoCoordinates();
        _previous_output = Terminal.Read(_position.Left, _position.Top, _width);

        GotoCoordinates();
        Console.Write(message);

        Stopwatch timer = new();
        timer.Start();

        while (timer.ElapsedMilliseconds <= _duration) 
            TryLowerIntensity(timer.ElapsedMilliseconds);

        timer.Stop();

        GotoCoordinates();
        Console.Write(new string(' ', _width));

        GotoCoordinates();
        Console.Write(_previous_output);

        Console.SetCursorPosition(_previous_position.Left, _previous_position.Top);
    }

    public void Display(string message, int delay)
    {
        Stopwatch wait = new();

        wait.Start();
        while (wait.ElapsedMilliseconds <= delay);
        wait.Stop();

        Display(message);
    }
}